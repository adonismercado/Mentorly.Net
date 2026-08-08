using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;

namespace Mentorly.Application.Services;

public sealed class CourseImageService(ICourseRepository courseRepository, ICourseImageRepository imageRepository, IUnitOfWork unitOfWork) : ICourseImageService
{
    public async Task<IReadOnlyList<CourseImageDto>> GetByCourseAsync(Guid courseId, CancellationToken c = default) => (await imageRepository.GetByCourseIdAsync(courseId, c)).Select(Map).ToList();
    public async Task<CourseImageDto?> CreateAsync(Guid courseId, CreateCourseImageDto dto, CancellationToken c = default) { if (await courseRepository.GetByIdAsync(courseId, c) is null) return null; var image = new CourseImage(Guid.NewGuid(), courseId, dto.ImageUrl, dto.AltText, dto.IsCover, dto.OrderIndex); imageRepository.Add(image); await unitOfWork.SaveChangesAsync(c); return Map(image); }
    public async Task<bool> UpdateAsync(Guid courseId, Guid imageId, UpdateCourseImageDto dto, CancellationToken c = default) { var image = await imageRepository.GetByIdAsync(imageId, c); if (image is null || image.CourseId != courseId) return false; image.Update(dto.ImageUrl, dto.AltText, dto.IsCover, dto.OrderIndex); imageRepository.Update(image); await unitOfWork.SaveChangesAsync(c); return true; }
    public async Task<bool> DeleteAsync(Guid courseId, Guid imageId, CancellationToken c = default) { var image = await imageRepository.GetByIdAsync(imageId, c); if (image is null || image.CourseId != courseId) return false; imageRepository.Delete(image); await unitOfWork.SaveChangesAsync(c); return true; }
    private static CourseImageDto Map(CourseImage x) => new(x.Id, x.CourseId, x.ImageUrl, x.AltText, x.IsCover, x.OrderIndex);
}

public sealed class UnitService(ICourseRepository courseRepository, IUnitRepository unitRepository, IStudentRepository studentRepository, IUnitOfWork unitOfWork) : IUnitService
{
    public async Task<IReadOnlyList<UnitDto>> GetByCourseAsync(Guid courseId, CancellationToken c = default) => (await unitRepository.GetByCourseIdAsync(courseId, c)).Select(Map).ToList();
    public async Task<UnitDto?> CreateAsync(Guid adminId, Guid courseId, CreateUnitDto dto, CancellationToken c = default) { if (!await IsAdminAsync(adminId, c) || await courseRepository.GetByIdAsync(courseId, c) is null) return null; var unit = new Unit(Guid.NewGuid(), courseId, dto.Title, dto.OrderIndex); unitRepository.Add(unit); await unitOfWork.SaveChangesAsync(c); return Map(unit); }
    public async Task<bool> UpdateAsync(Guid adminId, Guid unitId, UpdateUnitDto dto, CancellationToken c = default) { if (!await IsAdminAsync(adminId, c)) return false; var unit = await unitRepository.GetByIdAsync(unitId, c); if (unit is null) return false; unit.Rename(dto.Title); unit.ChangeOrder(dto.OrderIndex); unitRepository.Update(unit); await unitOfWork.SaveChangesAsync(c); return true; }
    public async Task<bool> DeleteAsync(Guid adminId, Guid unitId, CancellationToken c = default) { if (!await IsAdminAsync(adminId, c)) return false; var unit = await unitRepository.GetByIdAsync(unitId, c); if (unit is null) return false; if (await unitRepository.HasThemesAsync(unitId, c)) throw new InvalidOperationException("A unit with themes cannot be deleted."); unitRepository.Delete(unit); await unitOfWork.SaveChangesAsync(c); return true; }
    public async Task<bool> ReorderAsync(Guid adminId, Guid courseId, ReorderItemsDto dto, CancellationToken c = default) { if (!await IsAdminAsync(adminId, c)) return false; var units = await unitRepository.GetByCourseIdAsync(courseId, c); if (units.Count == 0 && await courseRepository.GetByIdAsync(courseId, c) is null) return false; ContentOrder.Reorder(units, dto.ItemIds, x => x.Id, (x, i) => x.ChangeOrder(i)); foreach (var unit in units) unitRepository.Update(unit); await unitOfWork.SaveChangesAsync(c); return true; }
    private static UnitDto Map(Unit x) => new(x.Id, x.CourseId, x.Title, x.OrderIndex);
    private async Task<bool> IsAdminAsync(Guid adminId, CancellationToken c) => (await studentRepository.GetByIdAsync(adminId, c))?.Role == StudentRole.Admin;
}

public sealed class ThemeService(IUnitRepository unitRepository, IThemeRepository themeRepository, IStudentRepository studentRepository, IUnitOfWork unitOfWork) : IThemeService
{
    public async Task<IReadOnlyList<ThemeDto>> GetByUnitAsync(Guid unitId, CancellationToken c = default) => (await themeRepository.GetByUnitIdAsync(unitId, c)).Select(Map).ToList();
    public async Task<ThemeDto?> CreateAsync(Guid adminId, Guid unitId, CreateThemeDto dto, CancellationToken c = default) { if (!await IsAdminAsync(adminId, c) || await unitRepository.GetByIdAsync(unitId, c) is null) return null; var theme = new Theme(Guid.NewGuid(), unitId, dto.Title, dto.ContentText, dto.OrderIndex); themeRepository.Add(theme); await unitOfWork.SaveChangesAsync(c); return Map(theme); }
    public async Task<bool> UpdateAsync(Guid adminId, Guid themeId, UpdateThemeDto dto, CancellationToken c = default) { if (!await IsAdminAsync(adminId, c)) return false; var theme = await themeRepository.GetByIdAsync(themeId, c); if (theme is null) return false; theme.Update(dto.Title, dto.ContentText, dto.OrderIndex); themeRepository.Update(theme); await unitOfWork.SaveChangesAsync(c); return true; }
    public async Task<bool> DeleteAsync(Guid adminId, Guid themeId, CancellationToken c = default) { if (!await IsAdminAsync(adminId, c)) return false; var theme = await themeRepository.GetByIdAsync(themeId, c); if (theme is null) return false; if (await themeRepository.HasActivitiesAsync(themeId, c)) throw new InvalidOperationException("A theme with activities cannot be deleted."); themeRepository.Delete(theme); await unitOfWork.SaveChangesAsync(c); return true; }
    public async Task<bool> ReorderAsync(Guid adminId, Guid unitId, ReorderItemsDto dto, CancellationToken c = default) { if (!await IsAdminAsync(adminId, c)) return false; var themes = await themeRepository.GetByUnitIdAsync(unitId, c); if (themes.Count == 0 && await unitRepository.GetByIdAsync(unitId, c) is null) return false; ContentOrder.Reorder(themes, dto.ItemIds, x => x.Id, (x, i) => x.ChangeOrder(i)); foreach (var theme in themes) themeRepository.Update(theme); await unitOfWork.SaveChangesAsync(c); return true; }
    private static ThemeDto Map(Theme x) => new(x.Id, x.UnitId, x.Title, x.ContentText, x.OrderIndex);
    private async Task<bool> IsAdminAsync(Guid adminId, CancellationToken c) => (await studentRepository.GetByIdAsync(adminId, c))?.Role == StudentRole.Admin;
}

public sealed class ActivityService(IThemeRepository themeRepository, IActivityRepository activityRepository, ISubmissionRepository submissionRepository, IStudentRepository studentRepository, IUnitOfWork unitOfWork) : IActivityService
{
    public async Task<IReadOnlyList<ActivityDto>> GetByThemeAsync(Guid themeId, CancellationToken c = default) => (await activityRepository.GetByThemeIdAsync(themeId, c)).Select(Map).ToList();
    public async Task<ActivityDto?> CreateAsync(Guid adminId, Guid themeId, CreateActivityDto dto, CancellationToken c = default) { if (!await IsAdminAsync(adminId, c) || await themeRepository.GetByIdAsync(themeId, c) is null) return null; var activity = new Activity(Guid.NewGuid(), themeId, dto.Title, dto.Type, dto.IsMandatory, dto.ApprovalStrategy, dto.OrderIndex); activityRepository.Add(activity); await unitOfWork.SaveChangesAsync(c); return Map(activity); }
    public async Task<bool> UpdateAsync(Guid adminId, Guid activityId, UpdateActivityDto dto, CancellationToken c = default) { if (!await IsAdminAsync(adminId, c)) return false; var activity = await activityRepository.GetByIdAsync(activityId, c); if (activity is null) return false; activity.Update(dto.Title, dto.Type, dto.IsMandatory, dto.ApprovalStrategy, dto.OrderIndex); activityRepository.Update(activity); await unitOfWork.SaveChangesAsync(c); return true; }
    public async Task<bool> DeleteAsync(Guid adminId, Guid activityId, CancellationToken c = default) { if (!await IsAdminAsync(adminId, c)) return false; var activity = await activityRepository.GetByIdAsync(activityId, c); if (activity is null) return false; if (await submissionRepository.HasSubmissionsForActivityAsync(activityId, c)) throw new InvalidOperationException("An activity with submissions cannot be deleted."); activityRepository.Delete(activity); await unitOfWork.SaveChangesAsync(c); return true; }
    public async Task<bool> ReorderAsync(Guid adminId, Guid themeId, ReorderItemsDto dto, CancellationToken c = default) { if (!await IsAdminAsync(adminId, c)) return false; var activities = await activityRepository.GetByThemeIdAsync(themeId, c); if (activities.Count == 0 && await themeRepository.GetByIdAsync(themeId, c) is null) return false; ContentOrder.Reorder(activities, dto.ItemIds, x => x.Id, (x, i) => x.ChangeOrder(i)); foreach (var activity in activities) activityRepository.Update(activity); await unitOfWork.SaveChangesAsync(c); return true; }
    private static ActivityDto Map(Activity x) => new(x.Id, x.ThemeId, x.Title, x.Type, x.IsMandatory, x.ApprovalStrategy, x.OrderIndex);
    private async Task<bool> IsAdminAsync(Guid adminId, CancellationToken c) => (await studentRepository.GetByIdAsync(adminId, c))?.Role == StudentRole.Admin;
}

file static class ContentOrder
{
    public static void Reorder<T>(IReadOnlyList<T> items, IReadOnlyList<Guid> ids, Func<T, Guid> id, Action<T, int> setOrder)
    {
        if (ids is null || ids.Count != items.Count || ids.Distinct().Count() != ids.Count || !items.Select(id).OrderBy(x => x).SequenceEqual(ids.OrderBy(x => x))) throw new ArgumentException("The ordered ids must contain each child exactly once.", nameof(ids));
        var positions = ids.Select((value, index) => (value, index)).ToDictionary(x => x.value, x => x.index + 1);
        foreach (var item in items) setOrder(item, positions[id(item)]);
    }
}
