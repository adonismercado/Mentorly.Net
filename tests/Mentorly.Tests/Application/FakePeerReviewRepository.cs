using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;

namespace Mentorly.Tests.Application;

public sealed class FakePeerReviewRepository(int existingApprovalCount, bool alreadyReviewed) : IPeerReviewRepository
{
    public PeerReview? LastAdded { get; private set; }

    public Task<PeerReview[]> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Array.Empty<PeerReview>());

    public Task<PeerReview?> GetByIdAsync(Guid peerReviewId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PeerReview[]> GetBySubmissionIdAsync(Guid submissionId, CancellationToken cancellationToken = default)
        => Task.FromResult(Array.Empty<PeerReview>());

    public Task<PeerReview[]> GetByReviewerStudentIdAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default)
        => Task.FromResult(Array.Empty<PeerReview>());

    public Task<PeerReview?> GetBySubmissionAndReviewerAsync(Guid submissionId, Guid reviewerStudentId, CancellationToken cancellationToken = default)
        => Task.FromResult(alreadyReviewed ? PeerReview.Create(submissionId, reviewerStudentId, true, "Already reviewed", DateTime.UtcNow) : null);

    public Task<bool> HasReviewerAlreadyReviewedAsync(Guid submissionId, Guid reviewerStudentId, DateTime? submissionSubmittedAtUtc = null, CancellationToken cancellationToken = default)
        => Task.FromResult(alreadyReviewed);

    public Task<int> CountApprovalsForSubmissionAsync(Guid submissionId, CancellationToken cancellationToken = default)
        => Task.FromResult(existingApprovalCount);

    public Task AddAsync(PeerReview review, CancellationToken cancellationToken = default)
    {
        LastAdded = review;
        return Task.CompletedTask;
    }

    public void Update(PeerReview review)
    {
        throw new NotImplementedException();
    }

    public void Delete(PeerReview review)
    {
        throw new NotImplementedException();
    }

}
