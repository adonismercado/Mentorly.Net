using Mentorly.Domain.Entities;

namespace Mentorly.Application.Abstractions.Persistence;

public sealed record AdminEscalatedSubmissionData(
    Guid SubmissionId,
    Guid EnrollmentId,
    Guid AuthorStudentId,
    string AuthorDisplayName,
    Guid CourseId,
    string CourseTitle,
    Guid ActivityId,
    string ActivityTitle,
    string EvidenceUrl,
    DateTime SubmittedAtUtc,
    DateTime EscalatedAtUtc,
    int PositiveReviews,
    int RejectedReviews);

public sealed record AdminPeerReviewAuditItemData(
    Guid PeerReviewId,
    Guid ReviewerStudentId,
    string ReviewerDisplayName,
    string ReviewerEmail,
    bool IsApproved,
    string FeedbackComment,
    DateTime CreatedAtUtc);

public sealed record AdminSubmissionAuditData(
    Guid SubmissionId,
    Guid EnrollmentId,
    Guid AuthorStudentId,
    string AuthorDisplayName,
    string AuthorEmail,
    Guid CourseId,
    string CourseTitle,
    Guid ActivityId,
    string ActivityTitle,
    string EvidenceUrl,
    Mentorly.Domain.Enums.SubmissionStatus Status,
    DateTime SubmittedAtUtc,
    DateTime? ReviewedAtUtc,
    AdminPeerReviewAuditItemData[] PeerReviews);

public interface ISubmissionRepository
{
    Task<Submission[]> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AdminEscalatedSubmissionData[]> GetEscalatedForAdminAsync(CancellationToken cancellationToken = default);
    Task<AdminSubmissionAuditData?> GetEscalatedAuditAsync(Guid submissionId, CancellationToken cancellationToken = default);
    Task<Submission?> GetByIdAsync(Guid submissionId, CancellationToken cancellationToken = default);

    Task<Submission?> GetByIdWithContextAsync(Guid submissionId, CancellationToken cancellationToken = default);

    Task<Submission?> GetByEnrollmentAndActivityAsync(Guid enrollmentId, Guid activityId, CancellationToken cancellationToken = default);

    Task<bool> HasStudentSubmittedActivityAsync(Guid studentId, Guid activityId, CancellationToken cancellationToken = default);
    Task<bool> HasSubmissionsForActivityAsync(Guid activityId, CancellationToken cancellationToken = default);
    Task<IReadOnlySet<Guid>> GetApprovedActivityIdsAsync(Guid enrollmentId, IReadOnlyCollection<Guid> activityIds, CancellationToken cancellationToken = default);
    Task<Submission[]> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);

    Task AddAsync(Submission submission, CancellationToken cancellationToken = default);
    Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default);
    Task DeleteAsync(Submission submission, CancellationToken cancellationToken = default);
}
