using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class SubmissionRepository(MentorlyDbContext dbContext) : ISubmissionRepository
{
    public async Task<IReadOnlyList<Submission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Submissions
            .AsNoTracking()
            .ToListAsync(cancellationToken);
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

    public Task AddAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        return dbContext.Submissions.AddAsync(submission, cancellationToken).AsTask();
    }

    public async Task<IReadOnlyList<Submission>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Submissions.Where(x => x.Enrollment.StudentId == studentId).OrderByDescending(x => x.SubmittedAt).ToListAsync(cancellationToken);
    }

    public Task<bool> HasSubmissionsForActivityAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        return dbContext.Submissions
            .AnyAsync(x => x.ActivityId == activityId, cancellationToken);
    }

    public async Task<IReadOnlySet<Guid>> GetApprovedActivityIdsAsync(Guid enrollmentId, IReadOnlyCollection<Guid> activityIds, CancellationToken cancellationToken = default)
    {
        if (activityIds.Count == 0)
        {
            return new HashSet<Guid>();
        }

        return await dbContext.Submissions
            .Where(x => x.EnrollmentId == enrollmentId && activityIds.Contains(x.ActivityId) && x.Status == SubmissionStatus.Approved)
            .Select(x => x.ActivityId)
            .ToHashSetAsync(cancellationToken);
    }

    public void Add(Submission submission)
    {
        dbContext.Submissions.Add(submission);
    }

    public void Update(Submission submission)
    {
        dbContext.Submissions.Update(submission);
    }

    public void Delete(Submission submission)
    {
        dbContext.Submissions.Remove(submission);
    }
}
