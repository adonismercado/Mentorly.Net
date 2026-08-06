using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class ThemeCompletionRepository(MentorlyDbContext dbContext) : IThemeCompletionRepository
{
    public async Task<IReadOnlyList<ThemeCompletion>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default) => await dbContext.ThemeCompletions.Where(x => x.EnrollmentId == enrollmentId).ToListAsync(cancellationToken);
    public Task<bool> ExistsAsync(Guid enrollmentId, Guid themeId, CancellationToken cancellationToken = default) => dbContext.ThemeCompletions.AnyAsync(x => x.EnrollmentId == enrollmentId && x.ThemeId == themeId, cancellationToken);
    public void Add(ThemeCompletion completion) => dbContext.ThemeCompletions.Add(completion);
}
