namespace Mentorly.Domain.Entities;

public class PeerReviewCriterionScore
{
    private PeerReviewCriterionScore() { }
    public PeerReviewCriterionScore(Guid peerReviewId, Guid rubricCriterionId, int score)
    {
        if (peerReviewId == Guid.Empty || rubricCriterionId == Guid.Empty) throw new ArgumentException("Review and criterion ids are required.");
        PeerReviewId = peerReviewId; RubricCriterionId = rubricCriterionId; Score = score;
    }
    public Guid PeerReviewId { get; private set; }
    public Guid RubricCriterionId { get; private set; }
    public int Score { get; private set; }
    public PeerReview PeerReview { get; private set; } = null!;
    public PeerReviewRubricCriterion RubricCriterion { get; private set; } = null!;
}
