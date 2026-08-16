namespace Mentorly.Application.DTOs;

public sealed record AnonymousSubmissionDto(Guid SubmissionId, Guid ActivityId, string ActivityTitle, Mentorly.Domain.Enums.EvidenceType EvidenceType, string EvidenceContent, DateTime SubmittedAtUtc);
