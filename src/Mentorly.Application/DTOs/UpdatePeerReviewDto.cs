namespace Mentorly.Application.DTOs;

public sealed record UpdatePeerReviewDto(
    bool IsApproved,
    string FeedbackComment);
