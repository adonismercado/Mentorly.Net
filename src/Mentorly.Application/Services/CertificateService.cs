namespace Mentorly.Application.Services;

public sealed class CertificateService : ICertificateService
{
    public string CreateCertificateUrl(Guid enrollmentId) => $"/api/enrollments/{enrollmentId}/certificate";
}
