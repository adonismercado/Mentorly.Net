namespace Mentorly.Application.DTOs;

public sealed record CourseDetailDto(
    Guid Id,
    string Title,
    string Description,
    string? ImageUrl,
    bool IsPublished,
    int RequiredPeerReviews,
    UnitDto[] Units);
