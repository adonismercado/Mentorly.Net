using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class SubmissionRepository(MentorlyDbContext dbContext) : ISubmissionRepository
{
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

    public Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        dbContext.Submissions.Update(submission);
        return Task.CompletedTask;
    }

    public Task<Submission[]> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
