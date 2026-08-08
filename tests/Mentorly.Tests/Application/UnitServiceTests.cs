using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Mentorly.Domain.Entities;

namespace Mentorly.Tests.Application;

public sealed class UnitServiceTests
{
    [Fact]
    public async Task ReorderAsync_UpdatesEveryUnitPosition()
    {
        var courseId = Guid.NewGuid();
        var first = new Unit(Guid.NewGuid(), courseId, "First", 1);
        var second = new Unit(Guid.NewGuid(), courseId, "Second", 2);
        var repository = new FakeUnitRepository([first, second]);
        var admin = CreateAdmin();
        var service = new UnitService(new FakeCourseRepository(courseId), repository, new FakeStudentRepository(admin), new FakeUnitOfWork());

        var reordered = await service.ReorderAsync(admin.Id, courseId, new ReorderItemsDto([second.Id, first.Id]));

        Assert.True(reordered);
        Assert.Equal(2, first.OrderIndex);
        Assert.Equal(1, second.OrderIndex);
    }

    [Fact]
    public async Task DeleteAsync_Throws_WhenUnitHasThemes()
    {
        var courseId = Guid.NewGuid();
        var unit = new Unit(Guid.NewGuid(), courseId, "Unit", 1);
        var admin = CreateAdmin();
        var service = new UnitService(new FakeCourseRepository(courseId), new FakeUnitRepository([unit], hasThemes: true), new FakeStudentRepository(admin), new FakeUnitOfWork());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(admin.Id, unit.Id));
    }

    private sealed class FakeCourseRepository(Guid courseId) : ICourseRepository
    {
        private readonly Course _course = new(courseId, "Course", "Description", Guid.NewGuid(), 1);
        public Task<Course[]> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<Course[]>([_course]);
        public Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Course?>(id == courseId ? _course : null);
        public void Add(Course course) { }
        public void Update(Course course) { }
        public void Delete(Course course) { }
    }

    private sealed class FakeUnitRepository(IReadOnlyList<Unit> units, bool hasThemes = false) : IUnitRepository
    {
        public Task<IReadOnlyList<Unit>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default) => Task.FromResult(units);
        public Task<Unit?> GetByIdAsync(Guid unitId, CancellationToken cancellationToken = default) => Task.FromResult(units.SingleOrDefault(x => x.Id == unitId));
        public Task<bool> HasThemesAsync(Guid unitId, CancellationToken cancellationToken = default) => Task.FromResult(hasThemes);
        public void Add(Unit unit) { }
        public void Update(Unit unit) { }
        public void Delete(Unit unit) { }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }

    private static Student CreateAdmin()
    {
        var admin = new Student(Guid.NewGuid(), "admin", "admin@mentorly.dev", "Admin");
        admin.PromoteToAdmin();
        return admin;
    }

    private sealed class FakeStudentRepository(Student admin) : IStudentRepository
    {
        public Task<Student?> GetByIdWithBadgesAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult<Student?>(studentId == admin.Id ? admin : null);
        public Task<bool> ExistsAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult(studentId == admin.Id);
        public Task<Student?> GetByIdAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult<Student?>(studentId == admin.Id ? admin : null);
        public Task<Student[]> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<Student[]>([admin]);
        public void Add(Student student) { }
        public void Update(Student student) { }
        public void Delete(Student student) { }
    }
}
