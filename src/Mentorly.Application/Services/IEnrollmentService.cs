using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface IEnrollmentService
{
    Task<IReadOnlyList<EnrollmentDto>> GetAllEnrollmentsAsync(CancellationToken cancellationToken = default);
    Task<EnrollmentDto?> GetEnrollmentByIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
    Task<EnrollmentDto> CreateEnrollmentAsync(CreateEnrollmentDto dto, CancellationToken cancellationToken = default);
}
