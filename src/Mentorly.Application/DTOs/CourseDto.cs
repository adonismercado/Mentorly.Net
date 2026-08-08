namespace Mentorly.Application.DTOs;

public sealed record CourseDto(
    Guid Id,
    string Title,
    string Description,
    string? ImageUrl,
    bool IsPublished,
    int RequiredPeerReviews);
