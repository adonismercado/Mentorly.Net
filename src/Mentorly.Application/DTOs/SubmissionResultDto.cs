using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record SubmissionResultDto(
    Guid SubmissionId,
    Guid EnrollmentId,
    Guid ActivityId,
    EvidenceType EvidenceType,
    string EvidenceContent,
    DateTime SubmittedAtUtc,
    SubmissionStatus Status
);
