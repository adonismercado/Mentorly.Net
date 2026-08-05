namespace Mentorly.Application.Abstractions.Persistence;

public interface IEnrollmentProgressRepository
{
    Task<bool> ThemeBelongsToCourseAsync(Guid themeId, Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetThemeIdsAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetMandatoryActivityIdsAsync(Guid courseId, CancellationToken cancellationToken = default);
}
