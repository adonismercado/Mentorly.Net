namespace Mentorly.Application.Abstractions.Persistence;

public interface IStudentRepository
{
    Task<bool> ExistsAsync(Guid studentId, CancellationToken cancellationToken = default);
}
