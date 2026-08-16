namespace Mentorly.Application.DTOs;
public sealed record PeerReviewRubricCriterionDto(Guid Id, Guid ActivityId, string Title, string Description, int MaxScore, int OrderIndex);
public sealed record CreatePeerReviewRubricCriterionDto(string Title, string Description, int MaxScore, int OrderIndex);
public sealed record UpdatePeerReviewRubricCriterionDto(string Title, string Description, int MaxScore, int OrderIndex);
public sealed record PeerReviewCriterionScoreDto(Guid RubricCriterionId, int Score);
