using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class EnrollmentProgressRepository(MentorlyDbContext dbContext) : IEnrollmentProgressRepository
{
    public Task<bool> ThemeBelongsToCourseAsync(Guid themeId, Guid courseId, CancellationToken cancellationToken = default) => dbContext.Themes.AnyAsync(x => x.Id == themeId && x.Unit.CourseId == courseId, cancellationToken);
    public async Task<IReadOnlyList<Guid>> GetThemeIdsAsync(Guid courseId, CancellationToken cancellationToken = default) => await dbContext.Themes.Where(x => x.Unit.CourseId == courseId).Select(x => x.Id).ToListAsync(cancellationToken);
    public async Task<IReadOnlyList<Guid>> GetMandatoryActivityIdsAsync(Guid courseId, CancellationToken cancellationToken = default) => await dbContext.Activities.Where(x => x.Theme.Unit.CourseId == courseId && x.IsMandatory).Select(x => x.Id).ToListAsync(cancellationToken);
}
