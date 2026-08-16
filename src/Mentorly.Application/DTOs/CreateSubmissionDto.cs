namespace Mentorly.Application.DTOs;

public sealed record CreateSubmissionDto(
    Mentorly.Domain.Enums.EvidenceType EvidenceType,
    string EvidenceContent);
