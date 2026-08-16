namespace Mentorly.Application.DTOs;

public sealed record UpdateSubmissionDto(
    Mentorly.Domain.Enums.EvidenceType EvidenceType,
    string EvidenceContent);
