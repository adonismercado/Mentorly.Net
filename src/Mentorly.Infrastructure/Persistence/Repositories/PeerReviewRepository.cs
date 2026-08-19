using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class PeerReviewRepository(MentorlyDbContext dbContext) : IPeerReviewRepository
{
    public async Task<PeerReview[]> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.PeerReviews
            .AsNoTracking()
            .Include(review => review.CriterionScores)
            .ToArrayAsync(cancellationToken);
    }

    public Task<PeerReview?> GetByIdAsync(Guid peerReviewId, CancellationToken cancellationToken = default)
    {
        return dbContext.PeerReviews
            .Include(review => review.CriterionScores)
            .FirstOrDefaultAsync(x => x.Id == peerReviewId, cancellationToken);
    }

    public async Task<PeerReview[]> GetBySubmissionIdAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        return await dbContext.PeerReviews.Include(review => review.CriterionScores).Where(x => x.SubmissionId == submissionId).OrderBy(x => x.CreatedAt).ToArrayAsync(cancellationToken);
    }

    public Task<PeerReview?> GetBySubmissionAndReviewerAsync(Guid submissionId, Guid reviewerStudentId, CancellationToken cancellationToken = default)
    {
        return dbContext.PeerReviews
            .Include(review => review.CriterionScores)
            .FirstOrDefaultAsync(x => x.SubmissionId == submissionId && x.ReviewerStudentId == reviewerStudentId, cancellationToken);
    }

    public async Task<PeerReview[]> GetByReviewerStudentIdAsync(Guid reviewerStudentId, CancellationToken cancellationToken = default)
    {
        return await dbContext.PeerReviews.Include(review => review.CriterionScores).Where(x => x.ReviewerStudentId == reviewerStudentId).OrderByDescending(x => x.CreatedAt).ToArrayAsync(cancellationToken);
    }

    public Task<bool> HasReviewerAlreadyReviewedAsync(Guid submissionId, Guid reviewerStudentId, DateTime? submissionSubmittedAtUtc = null, CancellationToken cancellationToken = default)
    {
        if (submissionSubmittedAtUtc is null)
        {
            return dbContext.PeerReviews
                .AnyAsync(x => x.SubmissionId == submissionId && x.ReviewerStudentId == reviewerStudentId, cancellationToken);
        }

        return dbContext.PeerReviews
            .AnyAsync(x => x.SubmissionId == submissionId && x.ReviewerStudentId == reviewerStudentId && (x.IsApproved || x.CreatedAt >= submissionSubmittedAtUtc.Value), cancellationToken);
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
