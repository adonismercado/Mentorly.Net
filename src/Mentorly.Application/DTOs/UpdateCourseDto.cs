namespace Mentorly.Application.DTOs;

public sealed record UpdateCourseDto(
    string Title,
    string Description,
    int RequiredPeerReviews);
