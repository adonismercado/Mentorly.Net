namespace Mentorly.Application.DTOs;

public sealed record CreateEnrollmentDto(
    Guid StudentId,
    Guid CourseId,
    int AttemptNumber);
