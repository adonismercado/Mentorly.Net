using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class PeerReviewRepository(MentorlyDbContext dbContext) : IPeerReviewRepository
{
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
}
