namespace Mentorly.Application.DTOs;

public sealed record CreateSubmissionDto(
    Guid EnrollmentId,
    Guid ActivityId,
    string EvidenceUrl);
