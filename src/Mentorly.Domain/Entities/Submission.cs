using Mentorly.Domain.Enums;

namespace Mentorly.Domain.Entities;

public class Submission
{
    private Submission()
    {
    }

    private Submission(Guid id, Guid enrollmentId, Guid activityId, EvidenceType evidenceType, string evidenceContent, DateTime submittedAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Submission id is required.", nameof(id));
        }

        if (enrollmentId == Guid.Empty)
        {
            throw new ArgumentException("Enrollment id is required.", nameof(enrollmentId));
        }

        if (activityId == Guid.Empty)
        {
            throw new ArgumentException("Activity id is required.", nameof(activityId));
        }

        Id = id;
        EnrollmentId = enrollmentId;
        ActivityId = activityId;
        EvidenceType = evidenceType;
        EvidenceContent = NormalizeEvidenceContent(evidenceType, evidenceContent);
        SubmittedAt = EnsureUtc(submittedAtUtc);
        Status = SubmissionStatus.Pending;
    }

    public Guid Id { get; private set; }

    public Guid EnrollmentId { get; private set; }

    public Guid ActivityId { get; private set; }

    public EvidenceType EvidenceType { get; private set; }

    public string EvidenceContent { get; private set; } = string.Empty;

    public SubmissionStatus Status { get; private set; }

    public DateTime SubmittedAt { get; private set; }

    public DateTime? ReviewedAt { get; private set; }

    public Enrollment Enrollment { get; private set; } = null!;

    public Activity Activity { get; private set; } = null!;

    public ICollection<PeerReview> PeerReviews { get; private set; } = [];

    public static Submission Create(Guid enrollmentId, Guid activityId, EvidenceType evidenceType, string evidenceContent, DateTime submittedAtUtc)
    {
        return new Submission(Guid.NewGuid(), enrollmentId, activityId, evidenceType, evidenceContent, submittedAtUtc);
    }

    public void ReplaceEvidence(EvidenceType evidenceType, string evidenceContent, DateTime? submittedAtUtc = null)
    {
        EvidenceType = evidenceType;
        EvidenceContent = NormalizeEvidenceContent(evidenceType, evidenceContent);
        Status = SubmissionStatus.Pending;
        SubmittedAt = EnsureUtc(submittedAtUtc ?? DateTime.UtcNow);
        ReviewedAt = null;
    }

    public void Approve(DateTime reviewedAtUtc)
    {
        Status = SubmissionStatus.Approved;
        ReviewedAt = EnsureUtc(reviewedAtUtc);
    }

    public void Reject(DateTime reviewedAtUtc)
    {
        Status = SubmissionStatus.Rejected;
        ReviewedAt = EnsureUtc(reviewedAtUtc);
    }

    public void Escalate(DateTime reviewedAtUtc)
    {
        Status = SubmissionStatus.Escalated;
        ReviewedAt = EnsureUtc(reviewedAtUtc);
    }

    private static string NormalizeEvidenceContent(EvidenceType evidenceType, string evidenceContent)
    {
        if (string.IsNullOrWhiteSpace(evidenceContent))
        {
            throw new ArgumentException("Evidence content is required.", nameof(evidenceContent));
        }

        var normalizedContent = evidenceContent.Trim();
        if (evidenceType == EvidenceType.Text)
        {
            return normalizedContent;
        }

        if (evidenceType != EvidenceType.Url)
        {
            throw new ArgumentOutOfRangeException(nameof(evidenceType), "Evidence type is not supported.");
        }

        if (!Uri.TryCreate(normalizedContent, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("Evidence url must be an absolute url.", nameof(evidenceContent));
        }

        if (uri.Scheme is not ("http" or "https"))
        {
            throw new ArgumentException("Evidence url must be http or https.", nameof(evidenceContent));
        }

        return uri.ToString();
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
