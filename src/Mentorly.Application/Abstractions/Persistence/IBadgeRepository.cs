using Mentorly.Domain.Entities;
namespace Mentorly.Application.Abstractions.Persistence;
public interface IBadgeRepository { Task<Badge?> GetByNameAsync(string name,CancellationToken cancellationToken=default); Task<bool> HasStudentBadgeAsync(Guid studentId,Guid badgeId,CancellationToken cancellationToken=default); void AddStudentBadge(StudentBadge badge); }
