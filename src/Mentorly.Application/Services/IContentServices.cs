using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface IUnitService
{
    Task<IReadOnlyList<UnitDto>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<UnitDto?> CreateAsync(Guid adminId, Guid courseId, CreateUnitDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid adminId, Guid unitId, UpdateUnitDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid adminId, Guid unitId, CancellationToken cancellationToken = default);
    Task<bool> ReorderAsync(Guid adminId, Guid courseId, ReorderItemsDto dto, CancellationToken cancellationToken = default);
}

public interface IThemeService
{
    Task<IReadOnlyList<ThemeDto>> GetByUnitAsync(Guid unitId, CancellationToken cancellationToken = default);
    Task<ThemeDto?> CreateAsync(Guid adminId, Guid unitId, CreateThemeDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid adminId, Guid themeId, UpdateThemeDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid adminId, Guid themeId, CancellationToken cancellationToken = default);
    Task<bool> ReorderAsync(Guid adminId, Guid unitId, ReorderItemsDto dto, CancellationToken cancellationToken = default);
}

public interface IActivityService
{
    Task<IReadOnlyList<ActivityDto>> GetByThemeAsync(Guid themeId, CancellationToken cancellationToken = default);
    Task<ActivityDto?> CreateAsync(Guid adminId, Guid themeId, CreateActivityDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid adminId, Guid activityId, UpdateActivityDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid adminId, Guid activityId, CancellationToken cancellationToken = default);
    Task<bool> ReorderAsync(Guid adminId, Guid themeId, ReorderItemsDto dto, CancellationToken cancellationToken = default);
}