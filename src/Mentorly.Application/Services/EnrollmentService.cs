using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Domain.Entities;

namespace Mentorly.Application.Services;

public sealed class EnrollmentService(
    IEnrollmentRepository enrollmentRepository,
    IUnitOfWork unitOfWork) : IEnrollmentService
{
    public async Task<EnrollmentDto[]> GetAllEnrollmentsAsync(CancellationToken cancellationToken = default)
    {
        var enrollments = await enrollmentRepository.GetAllAsync(cancellationToken);

        return enrollments.Select(e => new EnrollmentDto(
            e.Id,
            e.StudentId,
            e.CourseId,
            e.AttemptNumber,
            e.StartedAt,
            e.ExpiresAt,
            e.Status,
            e.CertificateUrl))
            .ToArray();
    }

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
            enrollment.AttemptNumber,
            enrollment.StartedAt,
            enrollment.ExpiresAt,
            enrollment.Status,
            enrollment.CertificateUrl);
    }

    public async Task<EnrollmentDto> CreateEnrollmentAsync(CreateEnrollmentDto dto, CancellationToken cancellationToken = default)
    {
        var enrollment = Enrollment.CreateNew(
            dto.StudentId,
            dto.CourseId,
            dto.AttemptNumber,
            DateTime.UtcNow);

        await enrollmentRepository.AddAsync(enrollment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new EnrollmentDto(
            enrollment.Id,
            enrollment.StudentId,
            enrollment.CourseId,
            enrollment.AttemptNumber,
            enrollment.StartedAt,
            enrollment.ExpiresAt,
            enrollment.Status,
            enrollment.CertificateUrl);
    }
}
