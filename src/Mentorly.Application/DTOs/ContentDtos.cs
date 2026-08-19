using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record UnitDto(Guid Id, Guid CourseId, string Title, int OrderIndex);
public sealed record CreateUnitDto(string Title, int OrderIndex);
public sealed record UpdateUnitDto(string Title, int OrderIndex);
public sealed record ThemeDto(Guid Id, Guid UnitId, string Title, string ContentText, int OrderIndex);
public sealed record CreateThemeDto(string Title, string ContentText, int OrderIndex);
public sealed record UpdateThemeDto(string Title, string ContentText, int OrderIndex);
public sealed record ActivityDto(Guid Id, Guid ThemeId, string Title, string Description, ActivityType Type, bool IsMandatory, ApprovalStrategy ApprovalStrategy, int OrderIndex);
public sealed record CreateActivityDto(string Title, string Description, ActivityType Type, bool IsMandatory, ApprovalStrategy ApprovalStrategy, int OrderIndex);
public sealed record UpdateActivityDto(string Title, string Description, ActivityType Type, bool IsMandatory, ApprovalStrategy ApprovalStrategy, int OrderIndex);
public sealed record ReorderItemsDto(IReadOnlyList<Guid> ItemIds);
