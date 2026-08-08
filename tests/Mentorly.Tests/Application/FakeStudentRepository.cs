using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;

namespace Mentorly.Tests.Application;

public sealed class FakeStudentRepository(bool exists) : IStudentRepository
{
    public Task<bool> ExistsAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult(exists);

    public Task<Student?> GetByIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        => Task.FromResult<Student?>(null);

    public Task<Student?> GetByIdWithBadgesAsync(Guid studentId, CancellationToken cancellationToken = default)
        => Task.FromResult<Student?>(null);

    public Task<Student[]> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Array.Empty<Student>());

    public void Add(Student student) { }

    public void Update(Student student) { }

    public void Delete(Student student) { }
}
