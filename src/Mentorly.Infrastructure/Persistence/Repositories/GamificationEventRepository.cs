using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class GamificationEventRepository(MentorlyDbContext dbContext) : IGamificationEventRepository
{
    public Task<bool> ExistsAsync(Guid studentId, GamificationEventType type, Guid referenceId, CancellationToken cancellationToken = default) => dbContext.GamificationEvents.AnyAsync(x => x.StudentId == studentId && x.Type == type && x.ReferenceId == referenceId, cancellationToken);
    public void Add(GamificationEvent gamificationEvent) => dbContext.GamificationEvents.Add(gamificationEvent);
}
