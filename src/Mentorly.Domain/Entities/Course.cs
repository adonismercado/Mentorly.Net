namespace Mentorly.Domain.Entities;

public class Course
{
    private Course()
    {
    }

    public Course(Guid id, string title, string description, Guid createdByAdminId, int requiredPeerReviews, string? imageUrl = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Course id is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Course title is required.", nameof(title));
        }

        if (createdByAdminId == Guid.Empty)
        {
            throw new ArgumentException("Admin id is required.", nameof(createdByAdminId));
        }

        Id = id;
        Title = title.Trim();
        Description = description.Trim();
        CreatedByAdminId = createdByAdminId;
        CreatedAt = DateTime.UtcNow;

        UpdateRequiredPeerReviews(requiredPeerReviews);
        UpdateImageUrl(imageUrl);
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string? ImageUrl { get; private set; }

    public Guid CreatedByAdminId { get; private set; }

    public bool IsPublished { get; private set; }

    public int RequiredPeerReviews { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public ICollection<Enrollment> Enrollments { get; private set; } = [];

    public ICollection<Unit> Units { get; private set; } = [];

    public void UpdateRequiredPeerReviews(int requiredPeerReviews)
    {
        if (requiredPeerReviews <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requiredPeerReviews), "Required peer reviews must be greater than zero.");
        }

        RequiredPeerReviews = requiredPeerReviews;
    }

    public void Rename(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Course title is required.", nameof(title));
        }

        Title = title.Trim();
    }

    public void UpdateDescription(string description)
    {
        Description = description.Trim();
    }

    public void UpdateImageUrl(string? imageUrl)
    {
        ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
    }

    public void Publish()
    {
        IsPublished = true;
    }

    public void Unpublish()
    {
        IsPublished = false;
    }
}
