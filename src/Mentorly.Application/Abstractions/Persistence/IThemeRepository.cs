using Mentorly.Domain.Entities;

namespace Mentorly.Application.Abstractions.Persistence;
public interface IThemeRepository { Task<IReadOnlyList<Theme>> GetByUnitIdAsync(Guid unitId, CancellationToken cancellationToken = default); Task<Theme?> GetByIdAsync(Guid themeId, CancellationToken cancellationToken = default); Task<bool> HasActivitiesAsync(Guid themeId, CancellationToken cancellationToken = default); void Add(Theme theme); void Update(Theme theme); void Delete(Theme theme); }
