namespace Mentorly.Domain.Entities;

public class Student
{
    private Student()
    {
    }

    public Student(Guid id, string googleUserId, string email, string displayName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Student id is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(googleUserId))
        {
            throw new ArgumentException("Google user id is required.", nameof(googleUserId));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name is required.", nameof(displayName));
        }

        Id = id;
        GoogleUserId = googleUserId.Trim();
        Email = email.Trim();
        DisplayName = displayName.Trim();
    }

    public Guid Id { get; private set; }

    public string GoogleUserId { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public ICollection<Enrollment> Enrollments { get; private set; } = [];

    public ICollection<PeerReview> PeerReviewsWritten { get; private set; } = [];

    public void UpdateProfile(string email, string displayName)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name is required.", nameof(displayName));
        }

        Email = email.Trim();
        DisplayName = displayName.Trim();
    }
}
