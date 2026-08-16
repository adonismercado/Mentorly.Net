namespace Mentorly.Application.DTOs;

public sealed record PeerReviewAuditDto(Guid PeerReviewId, Guid SubmissionId, Guid AuthorStudentId, Guid ReviewerStudentId, bool IsApproved, string FeedbackComment, PeerReviewCriterionScoreDto[] CriterionScores, DateTime CreatedAtUtc, Mentorly.Domain.Enums.EvidenceType EvidenceType, string EvidenceContent);
