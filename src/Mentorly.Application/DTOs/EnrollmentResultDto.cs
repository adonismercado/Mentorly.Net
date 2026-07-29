using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record EnrollmentResultDto(
    Guid EnrollmentId,
    Guid StudentId,
    Guid CourseId,
    int AttemptNumber,
    DateTime StartedAtUtc,
    DateTime ExpiresAtUtc,
    EnrollmentStatus Status
);
