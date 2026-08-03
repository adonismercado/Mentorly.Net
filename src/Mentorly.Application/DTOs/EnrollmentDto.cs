using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record EnrollmentDto(
    Guid Id,
    Guid StudentId,
    Guid CourseId,
    int AttemptNumber,
    DateTime StartedAt,
    DateTime ExpiresAt,
    EnrollmentStatus Status,
    string? CertificateUrl);
