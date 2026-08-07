using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;

namespace Mentorly.Application.Abstractions.Persistence;

public interface IGamificationEventRepository
{
    Task<bool> ExistsAsync(Guid studentId, GamificationEventType type, Guid referenceId, CancellationToken cancellationToken = default);
    void Add(GamificationEvent gamificationEvent);
}
