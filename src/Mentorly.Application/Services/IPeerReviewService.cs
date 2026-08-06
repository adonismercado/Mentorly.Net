using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface IPeerReviewService
{
    Task<IReadOnlyList<PeerReviewDto>> GetAllPeerReviewsAsync(CancellationToken cancellationToken = default);
    Task<PeerReviewDto?> GetPeerReviewByIdAsync(Guid peerReviewId, CancellationToken cancellationToken = default);
    Task<PeerReviewResultDto> SubmitReviewAsync(CreatePeerReviewRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> UpdatePeerReviewAsync(Guid peerReviewId, UpdatePeerReviewDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeletePeerReviewAsync(Guid peerReviewId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReviewQueueItemDto>> GetEligibleQueueAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default);
    Task<PeerReviewAuditDto?> GetAuditAsync(Guid peerReviewId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PeerReviewDto>> GetMyPeerReviewsAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default);
    Task<AnonymousSubmissionDto?> GetAnonymousSubmissionAsync(Guid peerReviewId, Guid reviewerStudentId, CancellationToken cancellationToken = default);
}
