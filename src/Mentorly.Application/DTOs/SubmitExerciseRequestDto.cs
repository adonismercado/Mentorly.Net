using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record SubmitExerciseRequestDto(
    Guid EnrollmentId,
    Guid ActivityId,
    EvidenceType EvidenceType,
    string EvidenceContent,
    DateTime SubmittedAtUtc
);
