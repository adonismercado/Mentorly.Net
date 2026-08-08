using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface ICourseCommunityService
{
    Task<IReadOnlyList<CourseMemberDto>?> GetMembersAsync(Guid courseId, Guid viewerStudentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaderboardEntryDto>?> GetLeaderboardAsync(Guid courseId, Guid viewerStudentId, CancellationToken cancellationToken = default);
    Task<LeaderboardEntryDto?> GetOwnPositionAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaderboardEntryDto>?> GetAdminLeaderboardAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default);
}

public sealed class CourseCommunityService(
    ICourseCommunityRepository communityRepository,
    IStudentRepository studentRepository) : ICourseCommunityService
{
    public async Task<IReadOnlyList<CourseMemberDto>?> GetMembersAsync(Guid courseId, Guid viewerStudentId, CancellationToken cancellationToken = default)
    {
        if (!await communityRepository.IsStudentEnrolledAsync(courseId, viewerStudentId, cancellationToken)) return null;
        return (await communityRepository.GetVisibleStudentsAsync(courseId, cancellationToken)).OrderBy(x => x.DisplayName).Select(x => new CourseMemberDto(x.StudentId, x.DisplayName, x.IsLeaderboardPublic)).ToList();
    }

    public async Task<IReadOnlyList<LeaderboardEntryDto>?> GetLeaderboardAsync(Guid courseId, Guid viewerStudentId, CancellationToken cancellationToken = default)
    {
        if (!await communityRepository.IsStudentEnrolledAsync(courseId, viewerStudentId, cancellationToken)) return null;
        var students = await communityRepository.GetVisibleStudentsAsync(courseId, cancellationToken);
        return Rank(students);
    }

    public async Task<LeaderboardEntryDto?> GetOwnPositionAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default)
    {
        if (!await communityRepository.IsStudentEnrolledAsync(courseId, studentId, cancellationToken)) return null;
        return Rank(await communityRepository.GetAllStudentsAsync(courseId, cancellationToken)).FirstOrDefault(x => x.StudentId == studentId);
    }

    public async Task<IReadOnlyList<LeaderboardEntryDto>?> GetAdminLeaderboardAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var admin = await studentRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin?.Role != Domain.Enums.StudentRole.Admin)
        {
            throw new InvalidOperationException("Only an administrator can view the complete leaderboard.");
        }

        if (!await communityRepository.CourseExistsAsync(courseId, cancellationToken)) return null;
        return Rank(await communityRepository.GetAllStudentsAsync(courseId, cancellationToken));
    }

    private static IReadOnlyList<LeaderboardEntryDto> Rank(IReadOnlyList<CourseCommunityStudentData> students) => students.OrderByDescending(x => x.TotalPoints).ThenBy(x => x.DisplayName).Select((x, index) => new LeaderboardEntryDto(index + 1, x.StudentId, x.DisplayName, x.TotalPoints, x.IsLeaderboardPublic)).ToList();
}
