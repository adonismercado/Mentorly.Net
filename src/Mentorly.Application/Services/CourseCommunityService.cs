using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface ICourseCommunityService
{
    Task<IReadOnlyList<CourseMemberDto>?> GetMembersAsync(Guid courseId, bool includePrivate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaderboardEntryDto>?> GetLeaderboardAsync(Guid courseId, bool includePrivate, CancellationToken cancellationToken = default);
    Task<LeaderboardEntryDto?> GetOwnPositionAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default);
}

public sealed class CourseCommunityService(ICourseCommunityRepository communityRepository) : ICourseCommunityService
{
    public async Task<IReadOnlyList<CourseMemberDto>?> GetMembersAsync(Guid courseId, bool includePrivate, CancellationToken cancellationToken = default)
    {
        if (!await communityRepository.CourseExistsAsync(courseId, cancellationToken)) return null;
        return (await communityRepository.GetStudentsAsync(courseId, includePrivate, cancellationToken)).OrderBy(x => x.DisplayName).Select(x => new CourseMemberDto(x.StudentId, x.DisplayName, x.IsLeaderboardPublic)).ToList();
    }

    public async Task<IReadOnlyList<LeaderboardEntryDto>?> GetLeaderboardAsync(Guid courseId, bool includePrivate, CancellationToken cancellationToken = default)
    {
        if (!await communityRepository.CourseExistsAsync(courseId, cancellationToken)) return null;
        var students = await communityRepository.GetStudentsAsync(courseId, includePrivate, cancellationToken);
        return Rank(students);
    }

    public async Task<LeaderboardEntryDto?> GetOwnPositionAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default)
    {
        if (!await communityRepository.IsStudentEnrolledAsync(courseId, studentId, cancellationToken)) return null;
        return Rank(await communityRepository.GetAllStudentsAsync(courseId, cancellationToken)).FirstOrDefault(x => x.StudentId == studentId);
    }

    private static IReadOnlyList<LeaderboardEntryDto> Rank(IReadOnlyList<CourseCommunityStudentData> students) => students.OrderByDescending(x => x.TotalPoints).ThenBy(x => x.DisplayName).Select((x, index) => new LeaderboardEntryDto(index + 1, x.StudentId, x.DisplayName, x.TotalPoints, x.IsLeaderboardPublic)).ToList();
}
