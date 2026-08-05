namespace Mentorly.Application.DTOs;

public sealed record AnonymousSubmissionDto(Guid SubmissionId, Guid ActivityId, string ActivityTitle, string EvidenceUrl, DateTime SubmittedAtUtc);
