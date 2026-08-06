namespace Mentorly.Domain.Entities;

public class ThemeCompletion
{
    private ThemeCompletion() { }

    public ThemeCompletion(Guid enrollmentId, Guid themeId, DateTime completedAtUtc)
    {
        if (enrollmentId == Guid.Empty || themeId == Guid.Empty) throw new ArgumentException("Enrollment and theme ids are required.");
        EnrollmentId = enrollmentId;
        ThemeId = themeId;
        CompletedAt = completedAtUtc.Kind == DateTimeKind.Utc ? completedAtUtc : completedAtUtc.ToUniversalTime();
    }

    public Guid EnrollmentId { get; private set; }
    public Guid ThemeId { get; private set; }
    public DateTime CompletedAt { get; private set; }
    public Enrollment Enrollment { get; private set; } = null!;
    public Theme Theme { get; private set; } = null!;
}
