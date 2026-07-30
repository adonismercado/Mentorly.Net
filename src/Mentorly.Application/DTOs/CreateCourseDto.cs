namespace Mentorly.Application.DTOs;

public sealed record CreateCourseDto(
    string Title,
    string Description,
    Guid CreatedByAdminId,
    int RequiredPeerReviews);
