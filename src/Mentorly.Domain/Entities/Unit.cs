namespace Mentorly.Domain.Entities;

public class Unit
{
    private Unit() { }
    public Unit(Guid id, Guid courseId, string title, int orderIndex)
    {
        if (id == Guid.Empty || courseId == Guid.Empty) throw new ArgumentException("Unit and course ids are required.");
        Id = id; CourseId = courseId; Rename(title); ChangeOrder(orderIndex);
    }
    public Guid Id { get; private set; }
    public Guid CourseId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int OrderIndex { get; private set; }
    public Course Course { get; private set; } = null!;
    public ICollection<Theme> Themes { get; private set; } = [];
    public void Rename(string title) { if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Unit title is required.", nameof(title)); Title = title.Trim(); }
    public void ChangeOrder(int orderIndex) { if (orderIndex <= 0) throw new ArgumentOutOfRangeException(nameof(orderIndex), "Order index must be greater than zero."); OrderIndex = orderIndex; }
}
