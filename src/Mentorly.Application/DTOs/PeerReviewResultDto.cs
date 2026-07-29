using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record PeerReviewResultDto(
    Guid PeerReviewId,
    Guid SubmissionId,
    Guid ReviewerStudentId,
    bool IsApproved,
    string FeedbackComment,
    DateTime CreatedAtUtc,
    int PositiveReviews,
    int RequiredPositiveReviews,
    SubmissionStatus SubmissionStatus
);
