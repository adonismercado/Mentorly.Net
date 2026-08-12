namespace Mentorly.Application.DTOs;

public sealed record AdminPeerReviewAuditItemDto(
    Guid PeerReviewId,
    Guid ReviewerStudentId,
    string ReviewerDisplayName,
    string ReviewerEmail,
    bool IsApproved,
    string FeedbackComment,
    DateTime CreatedAtUtc);
