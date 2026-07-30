namespace Mentorly.Application.DTOs;

public sealed record CourseDto(
    Guid Id,
    string Title,
    string Description,
    Guid CreatedByAdminId,
    bool IsPublished,
    int RequiredPeerReviews,
    DateTime CreatedAt);
