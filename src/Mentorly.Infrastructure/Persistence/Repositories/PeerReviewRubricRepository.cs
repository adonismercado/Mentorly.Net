using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Mentorly.Infrastructure.Persistence.Repositories;
public sealed class PeerReviewRubricRepository(MentorlyDbContext dbContext) : IPeerReviewRubricRepository
{
    public Task<PeerReviewRubricCriterion[]> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default) => dbContext.PeerReviewRubricCriteria.AsNoTracking().Where(x => x.ActivityId == activityId).OrderBy(x => x.OrderIndex).ToArrayAsync(cancellationToken);
    public Task<PeerReviewRubricCriterion?> GetByIdAsync(Guid criterionId, CancellationToken cancellationToken = default) => dbContext.PeerReviewRubricCriteria.FirstOrDefaultAsync(x => x.Id == criterionId, cancellationToken);
    public Task AddAsync(PeerReviewRubricCriterion criterion, CancellationToken cancellationToken = default) => dbContext.PeerReviewRubricCriteria.AddAsync(criterion, cancellationToken).AsTask();
    public void Update(PeerReviewRubricCriterion criterion) => dbContext.PeerReviewRubricCriteria.Update(criterion);
    public void Delete(PeerReviewRubricCriterion criterion) => dbContext.PeerReviewRubricCriteria.Remove(criterion);
}
