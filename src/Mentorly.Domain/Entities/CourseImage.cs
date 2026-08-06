namespace Mentorly.Domain.Entities;

public class CourseImage
{
    private CourseImage() { }

    public CourseImage(Guid id, Guid courseId, string imageUrl, string altText, bool isCover, int orderIndex)
    {
        if (id == Guid.Empty || courseId == Guid.Empty) throw new ArgumentException("Image and course ids are required.");
        Id = id; CourseId = courseId;
        Update(imageUrl, altText, isCover, orderIndex);
    }

    public Guid Id { get; private set; }
    public Guid CourseId { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public string AltText { get; private set; } = string.Empty;
    public bool IsCover { get; private set; }
    public int OrderIndex { get; private set; }
    public Course Course { get; private set; } = null!;

    public void Update(string imageUrl, string altText, bool isCover, int orderIndex)
    {
        if (!Uri.TryCreate(imageUrl?.Trim(), UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https")) throw new ArgumentException("Image url must be an absolute http or https url.", nameof(imageUrl));
        if (string.IsNullOrWhiteSpace(altText)) throw new ArgumentException("Alt text is required.", nameof(altText));
        if (orderIndex <= 0) throw new ArgumentOutOfRangeException(nameof(orderIndex), "Order index must be greater than zero.");
        ImageUrl = uri.ToString(); AltText = altText.Trim(); IsCover = isCover; OrderIndex = orderIndex;
    }
}
