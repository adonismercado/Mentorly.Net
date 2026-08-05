using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;
public sealed class ActivityRepository(MentorlyDbContext dbContext) : IActivityRepository
{ public async Task<IReadOnlyList<Activity>> GetByThemeIdAsync(Guid themeId, CancellationToken c = default) => await dbContext.Activities.Where(x => x.ThemeId == themeId).OrderBy(x => x.OrderIndex).ToListAsync(c); public Task<Activity?> GetByIdAsync(Guid id, CancellationToken c = default) => dbContext.Activities.FirstOrDefaultAsync(x => x.Id == id, c); public void Add(Activity activity) => dbContext.Activities.Add(activity); public void Update(Activity activity) => dbContext.Activities.Update(activity); public void Delete(Activity activity) => dbContext.Activities.Remove(activity); }
