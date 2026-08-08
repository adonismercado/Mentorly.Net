using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface IEnrollmentService
{
    Task<EnrollmentDto?> GetEnrollmentByIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
}
