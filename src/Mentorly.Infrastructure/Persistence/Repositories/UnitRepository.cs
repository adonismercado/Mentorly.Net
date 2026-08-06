using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;
public sealed class UnitRepository(MentorlyDbContext dbContext) : IUnitRepository
{ public async Task<IReadOnlyList<Unit>> GetByCourseIdAsync(Guid courseId, CancellationToken c = default) => await dbContext.Units.Where(x => x.CourseId == courseId).OrderBy(x => x.OrderIndex).ToListAsync(c); public Task<Unit?> GetByIdAsync(Guid id, CancellationToken c = default) => dbContext.Units.FirstOrDefaultAsync(x => x.Id == id, c); public Task<bool> HasThemesAsync(Guid id, CancellationToken c = default) => dbContext.Themes.AnyAsync(x => x.UnitId == id, c); public void Add(Unit unit) => dbContext.Units.Add(unit); public void Update(Unit unit) => dbContext.Units.Update(unit); public void Delete(Unit unit) => dbContext.Units.Remove(unit); }
