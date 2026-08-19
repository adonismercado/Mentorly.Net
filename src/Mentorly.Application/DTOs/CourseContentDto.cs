using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record CourseContentDto(
    Guid Id,
    string Title,
    string Description,
    string? ImageUrl,
    bool IsPublished,
    int RequiredPeerReviews,
    CourseUnitContentDto[] Units);

public sealed record CourseUnitContentDto(
    Guid Id,
    string Title,
    int OrderIndex,
    CourseThemeContentDto[] Themes);

public sealed record CourseThemeContentDto(
    Guid Id,
    string Title,
    string ContentText,
    int OrderIndex,
    CourseActivityContentDto[] Activities);

public sealed record CourseActivityContentDto(
    Guid Id,
    string Title,
    string Description,
    ActivityType Type,
    bool IsMandatory,
    ApprovalStrategy ApprovalStrategy,
    int OrderIndex);
