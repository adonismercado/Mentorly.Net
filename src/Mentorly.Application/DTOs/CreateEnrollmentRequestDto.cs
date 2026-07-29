namespace Mentorly.Application.DTOs;

public sealed record CreateEnrollmentRequestDto(
    Guid StudentId,
    Guid CourseId,
    DateTime StartedAtUtc
);
