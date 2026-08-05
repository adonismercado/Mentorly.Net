using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record CourseImageDto(Guid Id, Guid CourseId, string ImageUrl, string AltText, bool IsCover, int OrderIndex);
public sealed record CreateCourseImageDto(string ImageUrl, string AltText, bool IsCover, int OrderIndex);
public sealed record UpdateCourseImageDto(string ImageUrl, string AltText, bool IsCover, int OrderIndex);
public sealed record UnitDto(Guid Id, Guid CourseId, string Title, int OrderIndex);
public sealed record CreateUnitDto(string Title, int OrderIndex);
public sealed record UpdateUnitDto(string Title, int OrderIndex);
public sealed record ThemeDto(Guid Id, Guid UnitId, string Title, string ContentText, int OrderIndex);
public sealed record CreateThemeDto(string Title, string ContentText, int OrderIndex);
public sealed record UpdateThemeDto(string Title, string ContentText, int OrderIndex);
public sealed record ActivityDto(Guid Id, Guid ThemeId, string Title, ActivityType Type, bool IsMandatory, ApprovalStrategy ApprovalStrategy, int OrderIndex);
public sealed record CreateActivityDto(string Title, ActivityType Type, bool IsMandatory, ApprovalStrategy ApprovalStrategy, int OrderIndex);
public sealed record UpdateActivityDto(string Title, ActivityType Type, bool IsMandatory, ApprovalStrategy ApprovalStrategy, int OrderIndex);
public sealed record ReorderItemsDto(IReadOnlyList<Guid> ItemIds);
