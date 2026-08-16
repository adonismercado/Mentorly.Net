using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record SubmissionDto(
    Guid Id,
    Guid EnrollmentId,
    Guid ActivityId,
    string ActivityTitle,
    EvidenceType EvidenceType,
    string EvidenceContent,
    SubmissionStatus Status,
    DateTime SubmittedAt,
    DateTime? ReviewedAt);
