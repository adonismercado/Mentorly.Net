using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record EnrollmentStatusDto(
    Guid EnrollmentId,
    EnrollmentStatus Status,
    DateTime StartedAtUtc,
    DateTime ExpiresAtUtc,
    bool CanSubmit
);
