using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record StudentDto(
    Guid Id,
    string DisplayName,
    StudentRole Role,
    bool IsLeaderboardPublic,
    int TotalPoints);
