namespace Mentorly.Application.DTOs;

public sealed record CreatePeerReviewRequestDto(
    Guid SubmissionId,
    Guid ReviewerStudentId,
    bool IsApproved,
    string FeedbackComment,
    DateTime CreatedAtUtc
);
