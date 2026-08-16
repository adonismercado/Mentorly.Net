namespace Mentorly.Domain.Entities;

public class PeerReviewRubricCriterion
{
    private PeerReviewRubricCriterion() { }

    public PeerReviewRubricCriterion(Guid id, Guid activityId, string title, string description, int maxScore, int orderIndex)
    {
        if (id == Guid.Empty || activityId == Guid.Empty) throw new ArgumentException("Criterion and activity ids are required.");
        Id = id;
        ActivityId = activityId;
        Update(title, description, maxScore, orderIndex);
    }

    public Guid Id { get; private set; }
    public Guid ActivityId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int MaxScore { get; private set; }
    public int OrderIndex { get; private set; }
    public Activity Activity { get; private set; } = null!;

    public void Update(string title, string description, int maxScore, int orderIndex)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Criterion title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Criterion description is required.", nameof(description));
        if (maxScore <= 0 || orderIndex <= 0) throw new ArgumentOutOfRangeException(maxScore <= 0 ? nameof(maxScore) : nameof(orderIndex));
        Title = title.Trim(); Description = description.Trim(); MaxScore = maxScore; OrderIndex = orderIndex;
    }
}
