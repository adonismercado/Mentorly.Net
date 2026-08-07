using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;

namespace Mentorly.Tests.Application;

public sealed class FakePeerReviewRepository(int existingApprovalCount, bool alreadyReviewed) : IPeerReviewRepository
{
    public PeerReview? LastAdded { get; private set; }

    public Task<IReadOnlyList<PeerReview>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PeerReview?> GetByIdAsync(Guid peerReviewId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<PeerReview>> GetBySubmissionIdAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<PeerReview>> GetByReviewerStudentIdAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasReviewerAlreadyReviewedAsync(Guid submissionId, Guid reviewerStudentId, CancellationToken cancellationToken = default)
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

    public Task<IReadOnlyList<PeerReview>> GetBySubmissionIdAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<PeerReview>> GetByReviewerStudentIdAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}