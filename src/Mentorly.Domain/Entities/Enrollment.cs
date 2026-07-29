using Mentorly.Domain.Enums;

namespace Mentorly.Domain.Entities;

public class Enrollment
{
    private Enrollment()
    {
    }

    private Enrollment(Guid id, Guid studentId, Guid courseId, int attemptNumber, DateTime startedAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Enrollment id is required.", nameof(id));
        }

        if (studentId == Guid.Empty)
        {
            throw new ArgumentException("Student id is required.", nameof(studentId));
        }

        if (courseId == Guid.Empty)
        {
            throw new ArgumentException("Course id is required.", nameof(courseId));
        }

        if (attemptNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(attemptNumber), "Attempt number must be greater than zero.");
        }

        Id = id;
        StudentId = studentId;
        CourseId = courseId;
        AttemptNumber = attemptNumber;
        StartedAt = EnsureUtc(startedAtUtc);
        ExpiresAt = StartedAt.AddMonths(3);
        Status = EnrollmentStatus.Active;
    }

    public Guid Id { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid CourseId { get; private set; }

    public int AttemptNumber { get; private set; }

    public DateTime StartedAt { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public EnrollmentStatus Status { get; private set; }

    public string? CertificateUrl { get; private set; }

    public Student Student { get; private set; } = null!;

    public Course Course { get; private set; } = null!;

    public ICollection<Submission> Submissions { get; private set; } = [];

    public static Enrollment CreateNew(Guid studentId, Guid courseId, int attemptNumber, DateTime startedAtUtc)
    {
        return new Enrollment(Guid.NewGuid(), studentId, courseId, attemptNumber, startedAtUtc);
    }

    public void RefreshStatus(DateTime utcNow)
    {
        var now = EnsureUtc(utcNow);

        if (Status == EnrollmentStatus.Completed)
        {
            return;
        }

        if (now > ExpiresAt)
        {
            Status = EnrollmentStatus.Expired;
            return;
        }

        Status = EnrollmentStatus.Active;
    }

    public bool CanSubmit(DateTime utcNow)
    {
        RefreshStatus(utcNow);
        return Status == EnrollmentStatus.Active;
    }

    public void Complete(string certificateUrl)
    {
        if (string.IsNullOrWhiteSpace(certificateUrl))
        {
            throw new ArgumentException("Certificate url is required.", nameof(certificateUrl));
        }

        if (Status == EnrollmentStatus.Expired)
        {
            throw new InvalidOperationException("Expired enrollments cannot be completed.");
        }

        Status = EnrollmentStatus.Completed;
        CertificateUrl = certificateUrl.Trim();
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
