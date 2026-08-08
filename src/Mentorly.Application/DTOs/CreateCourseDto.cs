namespace Mentorly.Application.DTOs;

public sealed record CreateCourseDto(
    string Title,
    string Description,
    int RequiredPeerReviews,
    string? ImageUrl);
