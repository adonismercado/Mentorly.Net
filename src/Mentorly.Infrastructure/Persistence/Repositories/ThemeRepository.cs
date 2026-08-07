using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;
public sealed class ThemeRepository(MentorlyDbContext dbContext) : IThemeRepository
{ public async Task<IReadOnlyList<Theme>> GetByUnitIdAsync(Guid unitId, CancellationToken c = default) => await dbContext.Themes.Where(x => x.UnitId == unitId).OrderBy(x => x.OrderIndex).ToListAsync(c); public Task<Theme?> GetByIdAsync(Guid id, CancellationToken c = default) => dbContext.Themes.FirstOrDefaultAsync(x => x.Id == id, c); public Task<bool> HasActivitiesAsync(Guid id, CancellationToken c = default) => dbContext.Activities.AnyAsync(x => x.ThemeId == id, c); public void Add(Theme theme) => dbContext.Themes.Add(theme); public void Update(Theme theme) => dbContext.Themes.Update(theme); public void Delete(Theme theme) => dbContext.Themes.Remove(theme); }
