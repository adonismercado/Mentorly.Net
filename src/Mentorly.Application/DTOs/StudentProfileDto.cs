using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record StudentProfileDto(
    Guid Id,
    string Email,
    string DisplayName,
    StudentRole Role,
    bool IsLeaderboardPublic,
    int TotalPoints);
