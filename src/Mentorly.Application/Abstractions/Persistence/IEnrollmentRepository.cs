using Mentorly.Domain.Entities;

namespace Mentorly.Application.Abstractions.Persistence;

public interface IEnrollmentRepository
{
    Task<Enrollment[]> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Enrollment?> GetByIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
    Task<Enrollment[]> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetLatestByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);

    Task<bool> HasActiveEnrollmentAsync(Guid studentId, Guid courseId, DateTime utcNow, CancellationToken cancellationToken = default);

    Task<int> GetNextAttemptNumberAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);

    Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    void Add(Enrollment enrollment);
}
