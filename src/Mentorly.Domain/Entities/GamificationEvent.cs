using Mentorly.Domain.Enums;

namespace Mentorly.Domain.Entities;

public class GamificationEvent
{
    private GamificationEvent() { }

    public GamificationEvent(Guid id, Guid studentId, GamificationEventType type, Guid referenceId, int points, DateTime createdAtUtc)
    {
        if (id == Guid.Empty || studentId == Guid.Empty || referenceId == Guid.Empty) throw new ArgumentException("Event, student, and reference ids are required.");
        if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));
        if (points <= 0) throw new ArgumentOutOfRangeException(nameof(points));
        Id = id; StudentId = studentId; Type = type; ReferenceId = referenceId; Points = points;
        CreatedAt = createdAtUtc.Kind == DateTimeKind.Utc ? createdAtUtc : createdAtUtc.ToUniversalTime();
    }

    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public GamificationEventType Type { get; private set; }
    public Guid ReferenceId { get; private set; }
    public int Points { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Student Student { get; private set; } = null!;
}
