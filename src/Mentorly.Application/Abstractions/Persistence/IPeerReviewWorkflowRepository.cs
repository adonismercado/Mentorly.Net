using Mentorly.Domain.Enums;

namespace Mentorly.Application.Abstractions.Persistence;

public sealed record ActivityWorkflowData(Guid ActivityId, Guid CourseId, Guid UnitId, int UnitOrderIndex, ActivityType Type, bool IsMandatory, ApprovalStrategy ApprovalStrategy, int RequiredPeerReviews);
public sealed record ReviewQueueItemData(Guid SubmissionId, Guid ActivityId, string ActivityTitle, string EvidenceUrl, DateTime SubmittedAtUtc);
public sealed record ReviewAuditData(Guid PeerReviewId, Guid SubmissionId, Guid AuthorStudentId, Guid ReviewerStudentId, bool IsApproved, string FeedbackComment, DateTime CreatedAtUtc, string EvidenceUrl);
public sealed record AnonymousSubmissionData(Guid SubmissionId, Guid ActivityId, string ActivityTitle, string EvidenceUrl, DateTime SubmittedAtUtc);

public interface IPeerReviewWorkflowRepository
{
    Task<ActivityWorkflowData?> GetActivityAsync(Guid activityId, CancellationToken cancellationToken = default);
    Task<bool> CanSubmitMandatoryActivityAsync(Guid enrollmentId, Guid activityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReviewQueueItemData>> GetEligibleQueueAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default);
    Task<ReviewAuditData?> GetAuditAsync(Guid peerReviewId, CancellationToken cancellationToken = default);
    Task<AnonymousSubmissionData?> GetAnonymousSubmissionAsync(Guid submissionId, Guid reviewerStudentId, CancellationToken cancellationToken = default);
}
