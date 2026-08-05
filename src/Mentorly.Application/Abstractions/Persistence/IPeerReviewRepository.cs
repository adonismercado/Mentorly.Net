using Mentorly.Domain.Entities;

namespace Mentorly.Application.Abstractions.Persistence;

public interface IPeerReviewRepository
{
    Task<IReadOnlyList<PeerReview>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PeerReview?> GetByIdAsync(Guid peerReviewId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PeerReview>> GetBySubmissionIdAsync(Guid submissionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PeerReview>> GetByReviewerStudentIdAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default);
    Task<bool> HasReviewerAlreadyReviewedAsync(Guid submissionId, Guid reviewerStudentId, CancellationToken cancellationToken = default);

    Task<int> CountApprovalsForSubmissionAsync(Guid submissionId, CancellationToken cancellationToken = default);

    Task AddAsync(PeerReview review, CancellationToken cancellationToken = default);
    void Update(PeerReview review);
    void Delete(PeerReview review);
}
