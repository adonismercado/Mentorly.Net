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

    public Task<Submission[]> GetEscalatedAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Submissions
            .AsNoTracking()
            .Where(submission => submission.Status == SubmissionStatus.Escalated)
            .OrderBy(submission => submission.ReviewedAt ?? submission.SubmittedAt)
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
