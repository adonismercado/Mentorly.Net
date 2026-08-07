using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record StudentStatisticsDto(
    Guid StudentId,
    StudentRole Role,
    bool IsLeaderboardPublic,
    int TotalPoints,
    IReadOnlyList<BadgeDto> Badges);
