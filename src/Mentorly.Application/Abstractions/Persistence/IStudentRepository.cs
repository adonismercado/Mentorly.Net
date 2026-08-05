using Mentorly.Domain.Entities;

namespace Mentorly.Application.Abstractions.Persistence;

public interface IStudentRepository
{
    Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Student?> GetByIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid studentId, CancellationToken cancellationToken = default);
    void Add(Student student);
    void Update(Student student);
    void Delete(Student student);
}
