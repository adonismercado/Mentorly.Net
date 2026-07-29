using Mentorly.Domain.Entities;

namespace Mentorly.Application.Abstractions.Persistence;

public interface ISubmissionRepository
{
    Task<Submission?> GetByIdAsync(Guid submissionId, CancellationToken cancellationToken = default);

    Task<Submission?> GetByIdWithContextAsync(Guid submissionId, CancellationToken cancellationToken = default);

    Task<Submission?> GetByEnrollmentAndActivityAsync(Guid enrollmentId, Guid activityId, CancellationToken cancellationToken = default);

    Task<bool> HasStudentSubmittedActivityAsync(Guid studentId, Guid activityId, CancellationToken cancellationToken = default);

    Task AddAsync(Submission submission, CancellationToken cancellationToken = default);
}
