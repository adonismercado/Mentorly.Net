using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record EnrollmentProgressDto(
    Guid EnrollmentId,
    EnrollmentStatus Status,
    DateTime StartedAtUtc,
    DateTime ExpiresAtUtc,
    int TotalThemes,
    int CompletedThemes,
    int TotalMandatoryExercises,
    int ApprovedMandatoryExercises,
    int Percentage,
    bool IsCompleted,
    string? CertificateUrl);
