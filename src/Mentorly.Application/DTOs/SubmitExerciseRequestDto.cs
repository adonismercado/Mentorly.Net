namespace Mentorly.Application.DTOs;

public sealed record SubmitExerciseRequestDto(
    Guid EnrollmentId,
    Guid ActivityId,
    string EvidenceUrl,
    DateTime SubmittedAtUtc
);
