using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface IEnrollmentProgressService
{
    Task<IReadOnlyList<EnrollmentDto>> GetStudentEnrollmentsAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EnrollmentDto>> GetStudentEnrollmentsAsAdminAsync(Guid adminId, Guid studentId, CancellationToken cancellationToken = default);
    Task<EnrollmentDto?> RestartAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
    Task<EnrollmentProgressDto?> GetProgressAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
    Task<EnrollmentProgressDto?> GetProgressAsAdminAsync(Guid adminId, Guid enrollmentId, CancellationToken cancellationToken = default);
    Task<EnrollmentProgressDto?> CompleteThemeAsync(Guid enrollmentId, Guid themeId, CancellationToken cancellationToken = default);
    Task<EnrollmentStatusDto?> GetStatusAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
    Task<CertificateDto?> GetCertificateAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
}

public interface ICourseCompletionService
{
    Task<EnrollmentProgressDto?> EvaluateAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
}

public interface ICertificateService
{
    string CreateCertificateUrl(Guid enrollmentId);
}
