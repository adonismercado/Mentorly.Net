namespace Mentorly.Application.Abstractions.Persistence;

public sealed record CourseCommunityStudentData(Guid StudentId, string DisplayName, int TotalPoints, bool IsLeaderboardPublic);

public interface ICourseCommunityRepository
{
    Task<bool> CourseExistsAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<bool> IsStudentEnrolledAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourseCommunityStudentData>> GetStudentsAsync(Guid courseId, bool includePrivate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CourseCommunityStudentData>> GetAllStudentsAsync(Guid courseId, CancellationToken cancellationToken = default);
}
