using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.Services;

namespace Mentorly.Tests.Application;

public sealed class CourseCommunityServiceTests
{
    [Fact]
    public async Task GetLeaderboardAsync_HidesPrivateStudents_ForStudents()
    {
        var courseId = Guid.NewGuid();
        var repository = new FakeCommunityRepository(courseId, [
            new CourseCommunityStudentData(Guid.NewGuid(), "Visible", 10, true),
            new CourseCommunityStudentData(Guid.NewGuid(), "Private", 100, false)]);
        var service = new CourseCommunityService(repository);

        var leaderboard = await service.GetLeaderboardAsync(courseId, includePrivate: false);

        var entry = Assert.Single(leaderboard!);
        Assert.Equal("Visible", entry.DisplayName);
    }

    [Fact]
    public async Task GetLeaderboardAsync_OrdersByPoints_ForAdmins()
    {
        var courseId = Guid.NewGuid();
        var repository = new FakeCommunityRepository(courseId, [
            new CourseCommunityStudentData(Guid.NewGuid(), "Second", 20, true),
            new CourseCommunityStudentData(Guid.NewGuid(), "First", 50, false)]);
        var service = new CourseCommunityService(repository);

        var leaderboard = await service.GetLeaderboardAsync(courseId, includePrivate: true);

        Assert.Equal("First", leaderboard![0].DisplayName);
        Assert.Equal(1, leaderboard[0].Position);
    }

    [Fact]
    public async Task GetOwnPositionAsync_ReturnsPrivateStudentPosition()
    {
        var courseId = Guid.NewGuid();
        var privateStudentId = Guid.NewGuid();
        var repository = new FakeCommunityRepository(courseId, [
            new CourseCommunityStudentData(Guid.NewGuid(), "Top", 100, true),
            new CourseCommunityStudentData(privateStudentId, "Private", 50, false)]);
        var service = new CourseCommunityService(repository);

        var position = await service.GetOwnPositionAsync(courseId, privateStudentId);

        Assert.NotNull(position);
        Assert.Equal(2, position.Position);
        Assert.False(position.IsLeaderboardPublic);
    }

    private sealed class FakeCommunityRepository(Guid courseId, IReadOnlyList<CourseCommunityStudentData> students) : ICourseCommunityRepository
    {
        public Task<bool> CourseExistsAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(id == courseId);
        public Task<bool> IsStudentEnrolledAsync(Guid requestedCourseId, Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult(requestedCourseId == courseId && students.Any(x => x.StudentId == studentId));
        public Task<IReadOnlyList<CourseCommunityStudentData>> GetStudentsAsync(Guid requestedCourseId, bool includePrivate, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<CourseCommunityStudentData>>(students.Where(x => includePrivate || x.IsLeaderboardPublic).ToList());
        public Task<IReadOnlyList<CourseCommunityStudentData>> GetAllStudentsAsync(Guid requestedCourseId, CancellationToken cancellationToken = default) => Task.FromResult(students);
    }
}
