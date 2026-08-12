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
                submission.EvidenceUrl,
                submission.SubmittedAt,
                submission.ReviewedAt ?? submission.SubmittedAt,
                dbContext.PeerReviews.Count(review => review.SubmissionId == submission.Id && review.IsApproved),
                dbContext.PeerReviews.Count(review => review.SubmissionId == submission.Id && !review.IsApproved)))
            .ToArrayAsync(cancellationToken);
    }

    public Task<Submission?> GetByIdAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        return dbContext.Submissions
            .FirstOrDefaultAsync(x => x.Id == submissionId, cancellationToken);
    }

    public Task<Submission?> GetByIdWithContextAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        return dbContext.Submissions
            .Include(x => x.Enrollment)
            .ThenInclude(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == submissionId, cancellationToken);
    }

    public Task<Submission?> GetByEnrollmentAndActivityAsync(Guid enrollmentId, Guid activityId, CancellationToken cancellationToken = default)
    {
        return dbContext.Submissions
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
