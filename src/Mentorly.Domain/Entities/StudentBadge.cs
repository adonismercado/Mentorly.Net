namespace Mentorly.Domain.Entities;

public class StudentBadge
{
    private StudentBadge()
    {
    }

    public StudentBadge(Guid studentId, Guid badgeId, DateTime grantedAtUtc)
    {
        if (studentId == Guid.Empty)
        {
            throw new ArgumentException("Student id is required.", nameof(studentId));
        }

        if (badgeId == Guid.Empty)
        {
            throw new ArgumentException("Badge id is required.", nameof(badgeId));
        }

        StudentId = studentId;
        BadgeId = badgeId;
        GrantedAt = grantedAtUtc.Kind == DateTimeKind.Utc
            ? grantedAtUtc
            : grantedAtUtc.ToUniversalTime();
    }

    public Guid StudentId { get; private set; }

    public Guid BadgeId { get; private set; }

    public DateTime GrantedAt { get; private set; }

    public Student Student { get; private set; } = null!;

    public Badge Badge { get; private set; } = null!;
}
