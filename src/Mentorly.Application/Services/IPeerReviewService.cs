using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface IPeerReviewService
{
    Task<PeerReviewDto[]> GetAllPeerReviewsAsync(Guid adminId, CancellationToken cancellationToken = default);
    Task<PeerReviewResultDto> SubmitReviewAsync(Guid reviewerStudentId, CreatePeerReviewRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> UpdatePeerReviewAsync(Guid peerReviewId, UpdatePeerReviewDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeletePeerReviewAsync(Guid peerReviewId, CancellationToken cancellationToken = default);
    Task<ReviewQueueItemDto[]> GetEligibleQueueAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default);
    Task<PeerReviewAuditDto?> GetAuditAsync(Guid adminId, Guid peerReviewId, CancellationToken cancellationToken = default);
    Task<PeerReviewDto[]> GetMyPeerReviewsAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default);
    Task<AnonymousSubmissionDto?> GetAnonymousSubmissionAsync(Guid submissionId, Guid reviewerStudentId, CancellationToken cancellationToken = default);
    Task<PeerReviewRubricCriterionDto[]> GetRubricAsync(Guid activityId, CancellationToken cancellationToken = default);
    Task<PeerReviewRubricCriterionDto?> CreateRubricCriterionAsync(Guid adminId, Guid activityId, CreatePeerReviewRubricCriterionDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateRubricCriterionAsync(Guid adminId, Guid criterionId, UpdatePeerReviewRubricCriterionDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteRubricCriterionAsync(Guid adminId, Guid criterionId, CancellationToken cancellationToken = default);
}
