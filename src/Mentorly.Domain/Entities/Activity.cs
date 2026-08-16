using Mentorly.Domain.Enums;

namespace Mentorly.Domain.Entities;

public class Activity
{
    private Activity() { }
    public Activity(Guid id, Guid themeId, string title, ActivityType type, bool isMandatory, ApprovalStrategy approvalStrategy, int orderIndex)
    {
        if (id == Guid.Empty || themeId == Guid.Empty) throw new ArgumentException("Activity and theme ids are required.");
        Id = id; ThemeId = themeId; Update(title, type, isMandatory, approvalStrategy, orderIndex);
    }
    public Guid Id { get; private set; }
    public Guid ThemeId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public ActivityType Type { get; private set; }
    public bool IsMandatory { get; private set; }
    public ApprovalStrategy ApprovalStrategy { get; private set; }
    public int OrderIndex { get; private set; }
    public Theme Theme { get; private set; } = null!;
    public ICollection<PeerReviewRubricCriterion> PeerReviewRubricCriteria { get; private set; } = [];
    public void Update(string title, ActivityType type, bool isMandatory, ApprovalStrategy approvalStrategy, int orderIndex)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Activity title is required.", nameof(title));
        if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));
        if (!Enum.IsDefined(approvalStrategy)) throw new ArgumentOutOfRangeException(nameof(approvalStrategy));
        if (type == ActivityType.Quiz && approvalStrategy != ApprovalStrategy.Auto) throw new ArgumentException("Quizzes must use automatic approval.", nameof(approvalStrategy));
        if (orderIndex <= 0) throw new ArgumentOutOfRangeException(nameof(orderIndex), "Order index must be greater than zero.");
        Title = title.Trim(); Type = type; IsMandatory = isMandatory; ApprovalStrategy = approvalStrategy; OrderIndex = orderIndex;
    }
    public void ChangeOrder(int orderIndex) { if (orderIndex <= 0) throw new ArgumentOutOfRangeException(nameof(orderIndex)); OrderIndex = orderIndex; }
}
