using Mentorly.Domain.Entities;

namespace Mentorly.Application.Abstractions.Persistence;
public interface IUnitRepository { Task<IReadOnlyList<Unit>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default); Task<Unit?> GetByIdAsync(Guid unitId, CancellationToken cancellationToken = default); Task<bool> HasThemesAsync(Guid unitId, CancellationToken cancellationToken = default); void Add(Unit unit); void Update(Unit unit); void Delete(Unit unit); }
