namespace Mentorly.Application.DTOs;

public sealed record ReviewQueueItemDto(Guid SubmissionId, Guid ActivityId, string ActivityTitle, Mentorly.Domain.Enums.EvidenceType EvidenceType, string EvidenceContent, DateTime SubmittedAtUtc);
