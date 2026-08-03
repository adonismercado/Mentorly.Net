using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record SubmissionDto(
    Guid Id,
    Guid EnrollmentId,
    Guid ActivityId,
    string EvidenceUrl,
    SubmissionStatus Status,
    DateTime SubmittedAt,
    DateTime? ReviewedAt);
