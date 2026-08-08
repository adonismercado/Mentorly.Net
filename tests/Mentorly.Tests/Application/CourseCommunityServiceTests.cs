using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.Services;
using Mentorly.Domain.Entities;

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
        var service = new CourseCommunityService(repository, new FakeStudentRepository());

        var leaderboard = await service.GetLeaderboardAsync(courseId, repository.VisibleStudentId);

        var entry = Assert.Single(leaderboard!);
        Assert.Equal("Visible", entry.DisplayName);
    }

    [Fact]
    public async Task GetAdminLeaderboardAsync_OrdersByPoints_ForAdmins()
    {
        var courseId = Guid.NewGuid();
        var repository = new FakeCommunityRepository(courseId, [
            new CourseCommunityStudentData(Guid.NewGuid(), "Second", 20, true),
            new CourseCommunityStudentData(Guid.NewGuid(), "First", 50, false)]);
        var admin = CreateAdmin();
        var service = new CourseCommunityService(repository, new FakeStudentRepository(admin));

        var leaderboard = await service.GetAdminLeaderboardAsync(admin.Id, courseId);

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
        var service = new CourseCommunityService(repository, new FakeStudentRepository());

        var position = await service.GetOwnPositionAsync(courseId, privateStudentId);

        Assert.NotNull(position);
        Assert.Equal(2, position.Position);
        Assert.False(position.IsLeaderboardPublic);
    }

    private static Student CreateAdmin()
    {
        var admin = new Student(Guid.NewGuid(), "admin", "admin@mentorly.dev", "Admin");
        admin.PromoteToAdmin();
        return admin;
    }

    private sealed class FakeCommunityRepository(Guid courseId, IReadOnlyList<CourseCommunityStudentData> students) : ICourseCommunityRepository
    {
        public Guid VisibleStudentId => students.First(x => x.IsLeaderboardPublic).StudentId;
        public Task<bool> CourseExistsAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(id == courseId);
        public Task<bool> IsStudentEnrolledAsync(Guid requestedCourseId, Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult(requestedCourseId == courseId && students.Any(x => x.StudentId == studentId));
        public Task<IReadOnlyList<CourseCommunityStudentData>> GetVisibleStudentsAsync(Guid requestedCourseId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<CourseCommunityStudentData>>(students.Where(x => x.IsLeaderboardPublic).ToList());
        public Task<IReadOnlyList<CourseCommunityStudentData>> GetAllStudentsAsync(Guid requestedCourseId, CancellationToken cancellationToken = default) => Task.FromResult(students);
    }

    private sealed class FakeStudentRepository(params Student[] students) : IStudentRepository
    {
        public Task<Student?> GetByIdWithBadgesAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult(students.FirstOrDefault(student => student.Id == studentId));
        public Task<bool> ExistsAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult(students.Any(student => student.Id == studentId));
        public Task<Student?> GetByIdAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult(students.FirstOrDefault(student => student.Id == studentId));
        public Task<Student[]> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult(students);
        public void Add(Student student) { }
        public void Update(Student student) { }
        public void Delete(Student student) { }
    }
}
