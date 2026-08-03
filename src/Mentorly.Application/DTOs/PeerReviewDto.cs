namespace Mentorly.Application.DTOs;

public sealed record PeerReviewDto(
    Guid Id,
    Guid SubmissionId,
    Guid ReviewerStudentId,
    bool IsApproved,
    string FeedbackComment,
    DateTime CreatedAt);
