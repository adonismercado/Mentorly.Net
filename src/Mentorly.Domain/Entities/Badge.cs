namespace Mentorly.Domain.Entities;

public class Badge
{
    private Badge()
    {
    }

    public Badge(Guid id, string name, string description, string? imageUrl = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Badge id is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Badge name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Badge description is required.", nameof(description));
        }

        Id = id;
        Name = name.Trim();
        Description = description.Trim();
        ImageUrl = imageUrl?.Trim();
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string? ImageUrl { get; private set; }

    public ICollection<StudentBadge> StudentBadges { get; private set; } = [];
}
