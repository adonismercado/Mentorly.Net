using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class SubmissionRepository(MentorlyDbContext dbContext) : ISubmissionRepository
{
    public Task<Submission[]> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Submissions
            .AsNoTracking()
            .Include(submission => submission.Activity)
            .OrderByDescending(submission => submission.SubmittedAt)
            .ToArrayAsync(cancellationToken);
    }

    public Task<AdminEscalatedSubmissionData[]> GetEscalatedForAdminAsync(CancellationToken cancellationToken = default)
    {
        return (
            from submission in dbContext.Submissions.AsNoTracking()
            join enrollment in dbContext.Enrollments on submission.EnrollmentId equals enrollment.Id
            join author in dbContext.Students on enrollment.StudentId equals author.Id
            join course in dbContext.Courses on enrollment.CourseId equals course.Id
            join activity in dbContext.Activities on submission.ActivityId equals activity.Id
            where submission.Status == SubmissionStatus.Escalated
               || (activity.ApprovalStrategy == ApprovalStrategy.Admin && submission.Status == SubmissionStatus.Pending)
            orderby submission.ReviewedAt ?? submission.SubmittedAt
            select new AdminEscalatedSubmissionData(
                submission.Id,
                enrollment.Id,
                author.Id,
                author.DisplayName,
                course.Id,
                course.Title,
                activity.Id,
                activity.Title,
                submission.EvidenceType,
                submission.EvidenceContent,
                submission.SubmittedAt,
                submission.ReviewedAt ?? submission.SubmittedAt,
                dbContext.PeerReviews.Count(review => review.SubmissionId == submission.Id && review.IsApproved),
                dbContext.PeerReviews.Count(review => review.SubmissionId == submission.Id && !review.IsApproved)))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<AdminSubmissionAuditData?> GetEscalatedAuditAsync(
        Guid submissionId,
        CancellationToken cancellationToken = default)
    {
        var submission = await dbContext.Submissions
            .AsNoTracking()
            .Include(item => item.Enrollment)
            .ThenInclude(enrollment => enrollment.Student)
            .Include(item => item.Enrollment)
            .ThenInclude(enrollment => enrollment.Course)
            .FirstOrDefaultAsync(
                item => item.Id == submissionId && (item.Status == SubmissionStatus.Escalated || item.Status == SubmissionStatus.Pending),
                cancellationToken);

        if (submission is null)
        {
            return null;
        }

        var activity = await dbContext.Activities
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == submission.ActivityId, cancellationToken);

        if (activity is null)
        {
            return null;
        }

        var reviews = await (
            from review in dbContext.PeerReviews.AsNoTracking()
            join reviewer in dbContext.Students on review.ReviewerStudentId equals reviewer.Id
            where review.SubmissionId == submission.Id
            orderby review.CreatedAt
            select new AdminPeerReviewAuditItemData(
                review.Id,
                reviewer.Id,
                reviewer.DisplayName,
                reviewer.Email,
                review.IsApproved,
                review.FeedbackComment,
                review.CreatedAt))
            .ToArrayAsync(cancellationToken);

        return new AdminSubmissionAuditData(
            submission.Id,
            submission.Enrollment.Id,
            submission.Enrollment.Student.Id,
            submission.Enrollment.Student.DisplayName,
            submission.Enrollment.Student.Email,
            submission.Enrollment.Course.Id,
            submission.Enrollment.Course.Title,
            activity.Id,
            activity.Title,
            submission.EvidenceType,
            submission.EvidenceContent,
            submission.Status,
            submission.SubmittedAt,
            submission.ReviewedAt,
            reviews);
    }

    public Task<Submission?> GetByIdAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        return dbContext.Submissions
            .Include(submission => submission.Activity)
            .FirstOrDefaultAsync(x => x.Id == submissionId, cancellationToken);
    }

    public Task<Submission?> GetByIdWithContextAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        return dbContext.Submissions
            .Include(x => x.Enrollment)
            .ThenInclude(x => x.Course)
            .Include(x => x.Activity)
            .FirstOrDefaultAsync(x => x.Id == submissionId, cancellationToken);
    }

    public Task<Submission?> GetByEnrollmentAndActivityAsync(Guid enrollmentId, Guid activityId, CancellationToken cancellationToken = default)
    {
        return dbContext.Submissions
            .Include(submission => submission.Activity)
            .FirstOrDefaultAsync(x => x.EnrollmentId == enrollmentId && x.ActivityId == activityId, cancellationToken);
    }

    public Task<bool> HasStudentSubmittedActivityAsync(Guid studentId, Guid activityId, CancellationToken cancellationToken = default)
    {
        return dbContext.Submissions
            .AnyAsync(x => x.ActivityId == activityId && x.Enrollment.StudentId == studentId, cancellationToken);
    }

    public Task<bool> HasSubmissionsForActivityAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        return dbContext.Submissions
            .AnyAsync(submission => submission.ActivityId == activityId, cancellationToken);
    }

    public async Task<IReadOnlySet<Guid>> GetApprovedActivityIdsAsync(
        Guid enrollmentId,
        IReadOnlyCollection<Guid> activityIds,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Submissions
            .Where(submission =>
                submission.EnrollmentId == enrollmentId &&
                activityIds.Contains(submission.ActivityId) &&
                submission.Status == SubmissionStatus.Approved)
            .Select(submission => submission.ActivityId)
            .ToHashSetAsync(cancellationToken);
    }

    public Task<Submission[]> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return dbContext.Submissions
            .AsNoTracking()
            .Include(submission => submission.Activity)
            .Where(submission => submission.Enrollment.StudentId == studentId)
            .OrderByDescending(submission => submission.SubmittedAt)
            .ToArrayAsync(cancellationToken);
    }

    public Task AddAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        return dbContext.Submissions.AddAsync(submission, cancellationToken).AsTask();
    }

    public Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        dbContext.Submissions.Update(submission);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        dbContext.Submissions.Remove(submission);
        return Task.CompletedTask;
    }
}
