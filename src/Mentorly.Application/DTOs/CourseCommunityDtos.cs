namespace Mentorly.Application.DTOs;

public sealed record CourseMemberDto(Guid StudentId, string DisplayName, bool IsLeaderboardPublic);
public sealed record LeaderboardEntryDto(int Position, Guid StudentId, string DisplayName, int TotalPoints, bool IsLeaderboardPublic);
