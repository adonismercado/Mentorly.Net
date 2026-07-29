using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record SubmissionResultDto(
    Guid SubmissionId,
    Guid EnrollmentId,
    Guid ActivityId,
    string EvidenceUrl,
    DateTime SubmittedAtUtc,
    SubmissionStatus Status
);
