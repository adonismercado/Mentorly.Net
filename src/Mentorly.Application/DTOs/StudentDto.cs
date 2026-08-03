namespace Mentorly.Application.DTOs;

public sealed record StudentDto(
    Guid Id,
    string GoogleUserId,
    string Email,
    string DisplayName);
