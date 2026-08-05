using Mentorly.Domain.Entities;

namespace Mentorly.Application.Abstractions.Persistence;
public interface IActivityRepository { Task<IReadOnlyList<Activity>> GetByThemeIdAsync(Guid themeId, CancellationToken cancellationToken = default); Task<Activity?> GetByIdAsync(Guid activityId, CancellationToken cancellationToken = default); void Add(Activity activity); void Update(Activity activity); void Delete(Activity activity); }
