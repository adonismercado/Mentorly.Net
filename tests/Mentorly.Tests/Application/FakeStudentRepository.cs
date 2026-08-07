using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;

namespace Mentorly.Tests.Application;

public sealed class FakeStudentRepository(bool exists) : IStudentRepository
{
    public Task<Student?> GetByIdWithBadgesAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult(exists);
    public Task<Student?> GetByIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    Task<Student[]> IStudentRepository.GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Student>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Add(Student student)
    {
        throw new NotImplementedException();
    }

    public void Update(Student student)
    {
        throw new NotImplementedException();
    }

    public void Delete(Student student)
    {
        throw new NotImplementedException();
    }
}