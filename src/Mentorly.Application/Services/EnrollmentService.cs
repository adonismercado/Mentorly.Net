using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public sealed class EnrollmentService(
    IEnrollmentRepository enrollmentRepository) : IEnrollmentService
{
    public async Task<EnrollmentDto?> GetEnrollmentByIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken);

        if (enrollment is null)
        {
            return null;
        }

        return new EnrollmentDto(
            enrollment.Id,
            enrollment.StudentId,
            enrollment.CourseId,
            enrollment.Course?.Title ?? string.Empty,
            enrollment.AttemptNumber,
            enrollment.StartedAt,
            enrollment.ExpiresAt,
            enrollment.Status,
            enrollment.CertificateUrl);
    }

}
