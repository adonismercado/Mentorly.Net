namespace Mentorly.Domain.Entities;

public class PeerReview
{
    private PeerReview()
    {
    }

    private PeerReview(Guid id, Guid submissionId, Guid reviewerStudentId, bool isApproved, string feedbackComment, DateTime createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Peer review id is required.", nameof(id));
        }

        if (submissionId == Guid.Empty)
        {
            throw new ArgumentException("Submission id is required.", nameof(submissionId));
        }

        if (reviewerStudentId == Guid.Empty)
        {
            throw new ArgumentException("Reviewer student id is required.", nameof(reviewerStudentId));
        }

        if (string.IsNullOrWhiteSpace(feedbackComment))
        {
            throw new ArgumentException("Feedback comment is required.", nameof(feedbackComment));
        }

        Id = id;
        SubmissionId = submissionId;
        ReviewerStudentId = reviewerStudentId;
        IsApproved = isApproved;
        FeedbackComment = feedbackComment.Trim();
        CreatedAt = EnsureUtc(createdAtUtc);
    }

    public Guid Id { get; private set; }

    public Guid SubmissionId { get; private set; }

    public Guid ReviewerStudentId { get; private set; }

    public bool IsApproved { get; private set; }

    public string FeedbackComment { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public Submission Submission { get; private set; } = null!;

    public Student ReviewerStudent { get; private set; } = null!;
    public ICollection<PeerReviewCriterionScore> CriterionScores { get; private set; } = [];

    public void AddCriterionScore(Guid rubricCriterionId, int score) => CriterionScores.Add(new PeerReviewCriterionScore(Id, rubricCriterionId, score));

    public static PeerReview Create(Guid submissionId, Guid reviewerStudentId, bool isApproved, string feedbackComment, DateTime createdAtUtc)
    {
        return new PeerReview(Guid.NewGuid(), submissionId, reviewerStudentId, isApproved, feedbackComment, createdAtUtc);
    }

    public void UpdateReview(bool isApproved, string feedbackComment)
    {
        if (string.IsNullOrWhiteSpace(feedbackComment))
        {
            throw new ArgumentException("Feedback comment is required.", nameof(feedbackComment));
        }

        IsApproved = isApproved;
        FeedbackComment = feedbackComment.Trim();
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
