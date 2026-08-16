using Mentorly.Domain.Entities;
namespace Mentorly.Application.Abstractions.Persistence;
public interface IPeerReviewRubricRepository
{
    Task<PeerReviewRubricCriterion[]> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default);
    Task<PeerReviewRubricCriterion?> GetByIdAsync(Guid criterionId, CancellationToken cancellationToken = default);
    Task AddAsync(PeerReviewRubricCriterion criterion, CancellationToken cancellationToken = default);
    void Update(PeerReviewRubricCriterion criterion);
    void Delete(PeerReviewRubricCriterion criterion);
}
