namespace Mentorly.Application.DTOs;

public sealed record CertificateDto(Guid EnrollmentId, string CertificateUrl, DateTime CompletedAtUtc);
