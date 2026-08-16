namespace Mentorly.Application.DTOs;

public sealed record CreatePeerReviewRequestDto(
    Guid SubmissionId,
    bool IsApproved,
    string FeedbackComment,
    PeerReviewCriterionScoreDto[]? CriterionScores = null);
