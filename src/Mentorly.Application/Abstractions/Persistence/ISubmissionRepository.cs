using Mentorly.Domain.Entities;

namespace Mentorly.Application.Abstractions.Persistence;

public interface ISubmissionRepository
{
    Task<Submission[]> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Submission?> GetByIdAsync(Guid submissionId, CancellationToken cancellationToken = default);

    Task<Submission?> GetByIdWithContextAsync(Guid submissionId, CancellationToken cancellationToken = default);

    Task<Submission?> GetByEnrollmentAndActivityAsync(Guid enrollmentId, Guid activityId, CancellationToken cancellationToken = default);

    Task<bool> HasStudentSubmittedActivityAsync(Guid studentId, Guid activityId, CancellationToken cancellationToken = default);
    Task<bool> HasSubmissionsForActivityAsync(Guid activityId, CancellationToken cancellationToken = default);
    Task<IReadOnlySet<Guid>> GetApprovedActivityIdsAsync(Guid enrollmentId, IReadOnlyCollection<Guid> activityIds, CancellationToken cancellationToken = default);
    Task<Submission[]> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);

    Task AddAsync(Submission submission, CancellationToken cancellationToken = default);
    Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default);
    Task DeleteAsync(Submission submission, CancellationToken cancellationToken = default);
}
