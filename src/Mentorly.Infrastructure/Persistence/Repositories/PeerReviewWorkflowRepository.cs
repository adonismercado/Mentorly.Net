using Mentorly.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class PeerReviewWorkflowRepository(MentorlyDbContext dbContext) : IPeerReviewWorkflowRepository
{
    public Task<ActivityWorkflowData?> GetActivityAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        return (from activity in dbContext.Activities
                join theme in dbContext.Themes on activity.ThemeId equals theme.Id
                join unit in dbContext.Units on theme.UnitId equals unit.Id
                join course in dbContext.Courses on unit.CourseId equals course.Id
                where activity.Id == activityId
                select new ActivityWorkflowData(activity.Id, course.Id, unit.Id, unit.OrderIndex, activity.Type, activity.IsMandatory, activity.ApprovalStrategy, course.RequiredPeerReviews))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> CanSubmitMandatoryActivityAsync(Guid enrollmentId, Guid activityId, CancellationToken cancellationToken = default)
    {
        var activity = await GetActivityAsync(activityId, cancellationToken);
        if (activity is null || !activity.IsMandatory || activity.Type != Domain.Enums.ActivityType.Exercise || activity.UnitOrderIndex == 1)
        {
            return activity is not null;
        }

        var previousMandatoryActivities = await (from candidate in dbContext.Activities
                                                 join theme in dbContext.Themes on candidate.ThemeId equals theme.Id
                                                 join unit in dbContext.Units on theme.UnitId equals unit.Id
                                                 where unit.CourseId == activity.CourseId && unit.OrderIndex < activity.UnitOrderIndex && candidate.IsMandatory && candidate.Type == Domain.Enums.ActivityType.Exercise
                                                 select candidate.Id).ToListAsync(cancellationToken);

        var approvedCount = await dbContext.Submissions.CountAsync(x => x.EnrollmentId == enrollmentId && previousMandatoryActivities.Contains(x.ActivityId) && x.Status == Domain.Enums.SubmissionStatus.Approved, cancellationToken);
        if (approvedCount != previousMandatoryActivities.Count)
        {
            return false;
        }

        var enrollment = await dbContext.Enrollments.FirstOrDefaultAsync(x => x.Id == enrollmentId, cancellationToken);
        if (enrollment is null)
        {
            return false;
        }

        var reviewCount = await (from review in dbContext.PeerReviews
                                 join submission in dbContext.Submissions on review.SubmissionId equals submission.Id
                                 join reviewedEnrollment in dbContext.Enrollments on submission.EnrollmentId equals reviewedEnrollment.Id
                                 where review.ReviewerStudentId == enrollment.StudentId && reviewedEnrollment.CourseId == activity.CourseId
                                 select review.Id).CountAsync(cancellationToken);

        return reviewCount >= activity.RequiredPeerReviews;
    }

    public async Task<IReadOnlyList<ReviewQueueItemData>> GetEligibleQueueAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default)
    {
        return await (from submission in dbContext.Submissions
                      join enrollment in dbContext.Enrollments on submission.EnrollmentId equals enrollment.Id
                      join activity in dbContext.Activities on submission.ActivityId equals activity.Id
                      join theme in dbContext.Themes on activity.ThemeId equals theme.Id
                      join unit in dbContext.Units on theme.UnitId equals unit.Id
                      where submission.Status == Domain.Enums.SubmissionStatus.Pending
                          && activity.ApprovalStrategy == Domain.Enums.ApprovalStrategy.PeerReview
                          && enrollment.StudentId != reviewerStudentId
                          && dbContext.Enrollments.Any(reviewerEnrollment =>
                              reviewerEnrollment.StudentId == reviewerStudentId &&
                              reviewerEnrollment.CourseId == unit.CourseId &&
                              (reviewerEnrollment.Status == Domain.Enums.EnrollmentStatus.Active || reviewerEnrollment.Status == Domain.Enums.EnrollmentStatus.Completed) &&
                              reviewerEnrollment.ExpiresAt >= DateTime.UtcNow)
                          && dbContext.Submissions.Any(own => own.ActivityId == submission.ActivityId && own.Enrollment.StudentId == reviewerStudentId)
                          && !dbContext.PeerReviews.Any(review =>
                              review.SubmissionId == submission.Id &&
                              review.ReviewerStudentId == reviewerStudentId &&
                              (review.IsApproved || review.CreatedAt >= submission.SubmittedAt))
                      orderby submission.SubmittedAt
                      select new ReviewQueueItemData(submission.Id, submission.ActivityId, activity.Title, submission.EvidenceType, submission.EvidenceContent, submission.SubmittedAt))
            .ToListAsync(cancellationToken);
    }

    public Task<ReviewAuditData?> GetAuditAsync(Guid peerReviewId, CancellationToken cancellationToken = default)
    {
        return (from review in dbContext.PeerReviews
                join submission in dbContext.Submissions on review.SubmissionId equals submission.Id
                join enrollment in dbContext.Enrollments on submission.EnrollmentId equals enrollment.Id
                where review.Id == peerReviewId
                select new ReviewAuditData(review.Id, submission.Id, enrollment.StudentId, review.ReviewerStudentId, review.IsApproved, review.FeedbackComment, review.CreatedAt, submission.EvidenceType, submission.EvidenceContent))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<AnonymousSubmissionData?> GetAnonymousSubmissionAsync(Guid submissionId, Guid reviewerStudentId, CancellationToken cancellationToken = default)
    {
        return (from submission in dbContext.Submissions
                join enrollment in dbContext.Enrollments on submission.EnrollmentId equals enrollment.Id
                join activity in dbContext.Activities on submission.ActivityId equals activity.Id
                join theme in dbContext.Themes on activity.ThemeId equals theme.Id
                join unit in dbContext.Units on theme.UnitId equals unit.Id
                where submission.Id == submissionId
                    && submission.Status == Domain.Enums.SubmissionStatus.Pending
                    && activity.ApprovalStrategy == Domain.Enums.ApprovalStrategy.PeerReview
                    && enrollment.StudentId != reviewerStudentId
                    && dbContext.Enrollments.Any(reviewerEnrollment =>
                        reviewerEnrollment.StudentId == reviewerStudentId &&
                        reviewerEnrollment.CourseId == unit.CourseId &&
                        (reviewerEnrollment.Status == Domain.Enums.EnrollmentStatus.Active || reviewerEnrollment.Status == Domain.Enums.EnrollmentStatus.Completed) &&
                        reviewerEnrollment.ExpiresAt >= DateTime.UtcNow)
                    && dbContext.Submissions.Any(own => own.ActivityId == submission.ActivityId && own.Enrollment.StudentId == reviewerStudentId)
                    && !dbContext.PeerReviews.Any(review =>
                        review.SubmissionId == submission.Id &&
                        review.ReviewerStudentId == reviewerStudentId &&
                        (review.IsApproved || review.CreatedAt >= submission.SubmittedAt))
                select new AnonymousSubmissionData(submission.Id, activity.Id, activity.Title, submission.EvidenceType, submission.EvidenceContent, submission.SubmittedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
