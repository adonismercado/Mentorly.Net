using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record AdminSubmissionAuditDto(
    Guid SubmissionId,
    Guid EnrollmentId,
    Guid AuthorStudentId,
    string AuthorDisplayName,
    string AuthorEmail,
    Guid CourseId,
    string CourseTitle,
    Guid ActivityId,
    string ActivityTitle,
    EvidenceType EvidenceType,
    string EvidenceContent,
    SubmissionStatus Status,
    DateTime SubmittedAtUtc,
    DateTime? ReviewedAtUtc,
    AdminPeerReviewAuditItemDto[] PeerReviews);
