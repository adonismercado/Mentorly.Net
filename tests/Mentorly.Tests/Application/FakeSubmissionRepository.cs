using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;

namespace Mentorly.Tests.Application;

public sealed class FakeSubmissionRepository(Submission submission, bool reviewerHasOwnSubmission) : ISubmissionRepository
{
    public Task<Submission[]> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Submission?> GetByIdAsync(Guid submissionId, CancellationToken cancellationToken = default)
        => Task.FromResult(submissionId == submission.Id ? submission : null);

    public Task<Submission?> GetByIdWithContextAsync(Guid submissionId, CancellationToken cancellationToken = default)
        => Task.FromResult(submissionId == submission.Id ? submission : null);

    public Task<Submission?> GetByEnrollmentAndActivityAsync(Guid enrollmentId, Guid activityId, CancellationToken cancellationToken = default)
        => Task.FromResult<Submission?>(null);

    public Task<bool> HasStudentSubmittedActivityAsync(Guid studentId, Guid activityId, CancellationToken cancellationToken = default)
        => Task.FromResult(reviewerHasOwnSubmission);

    public Task AddAsync(Submission submission, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public void Update(Submission submission)
    {
        throw new NotImplementedException();
    }

    public void Delete(Submission submission)
    {
        throw new NotImplementedException();
    }
}