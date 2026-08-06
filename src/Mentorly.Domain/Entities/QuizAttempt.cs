namespace Mentorly.Domain.Entities;
public class QuizAttempt
{
    private QuizAttempt() { }
    public QuizAttempt(Guid id, Guid enrollmentId, Guid activityId, decimal score, bool passed, DateTime submittedAtUtc) { Id=id; EnrollmentId=enrollmentId; ActivityId=activityId; Score=score; Passed=passed; SubmittedAt=submittedAtUtc.Kind==DateTimeKind.Utc?submittedAtUtc:submittedAtUtc.ToUniversalTime(); }
    public Guid Id { get; private set; } public Guid EnrollmentId { get; private set; } public Guid ActivityId { get; private set; } public decimal Score { get; private set; } public bool Passed { get; private set; } public DateTime SubmittedAt { get; private set; }
}
