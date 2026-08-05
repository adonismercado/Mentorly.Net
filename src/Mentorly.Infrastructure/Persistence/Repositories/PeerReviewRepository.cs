using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class PeerReviewRepository(MentorlyDbContext dbContext) : IPeerReviewRepository
{
    public async Task<IReadOnlyList<PeerReview>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.PeerReviews
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<PeerReview?> GetByIdAsync(Guid peerReviewId, CancellationToken cancellationToken = default)
    {
        return dbContext.PeerReviews
            .FirstOrDefaultAsync(x => x.Id == peerReviewId, cancellationToken);
    }

    public Task<bool> HasReviewerAlreadyReviewedAsync(Guid submissionId, Guid reviewerStudentId, CancellationToken cancellationToken = default)
    {
        return dbContext.PeerReviews
            .AnyAsync(x => x.SubmissionId == submissionId && x.ReviewerStudentId == reviewerStudentId, cancellationToken);
    }

    public Task<int> CountApprovalsForSubmissionAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        return dbContext.PeerReviews
            .CountAsync(x => x.SubmissionId == submissionId && x.IsApproved, cancellationToken);
    }

    public Task AddAsync(PeerReview review, CancellationToken cancellationToken = default)
    {
        return dbContext.PeerReviews.AddAsync(review, cancellationToken).AsTask();
    }

    public void Update(PeerReview review)
    {
        dbContext.PeerReviews.Update(review);
    }

    public void Delete(PeerReview review)
    {
        dbContext.PeerReviews.Remove(review);
    }
}