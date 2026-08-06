namespace Mentorly.Application.DTOs;

public sealed record PeerReviewFeedbackDto(Guid PeerReviewId, bool IsApproved, string FeedbackComment, DateTime CreatedAtUtc);
