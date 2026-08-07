namespace Mentorly.Application.DTOs;

public sealed record ReviewQueueItemDto(Guid SubmissionId, Guid ActivityId, string ActivityTitle, string EvidenceUrl, DateTime SubmittedAtUtc);
