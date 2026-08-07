namespace Mentorly.Domain.Entities;

public class Theme
{
    private Theme() { }
    public Theme(Guid id, Guid unitId, string title, string contentText, int orderIndex)
    {
        if (id == Guid.Empty || unitId == Guid.Empty) throw new ArgumentException("Theme and unit ids are required.");
        Id = id; UnitId = unitId; Update(title, contentText, orderIndex);
    }
    public Guid Id { get; private set; }
    public Guid UnitId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string ContentText { get; private set; } = string.Empty;
    public int OrderIndex { get; private set; }
    public Unit Unit { get; private set; } = null!;
    public ICollection<Activity> Activities { get; private set; } = [];
    public void Update(string title, string contentText, int orderIndex)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Theme title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(contentText)) throw new ArgumentException("Theme content is required.", nameof(contentText));
        if (orderIndex <= 0) throw new ArgumentOutOfRangeException(nameof(orderIndex), "Order index must be greater than zero.");
        Title = title.Trim(); ContentText = contentText.Trim(); OrderIndex = orderIndex;
    }
    public void ChangeOrder(int orderIndex) { if (orderIndex <= 0) throw new ArgumentOutOfRangeException(nameof(orderIndex)); OrderIndex = orderIndex; }
}
