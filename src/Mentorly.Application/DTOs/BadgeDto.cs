namespace Mentorly.Application.DTOs;

public sealed record BadgeDto(
    Guid Id,
    string Name,
    string Description,
    string? ImageUrl,
    DateTime GrantedAt);
