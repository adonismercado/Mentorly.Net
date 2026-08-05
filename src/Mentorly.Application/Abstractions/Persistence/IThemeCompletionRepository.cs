using Mentorly.Domain.Entities;

namespace Mentorly.Application.Abstractions.Persistence;

public interface IThemeCompletionRepository
{
    Task<IReadOnlyList<ThemeCompletion>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid enrollmentId, Guid themeId, CancellationToken cancellationToken = default);
    void Add(ThemeCompletion completion);
}
