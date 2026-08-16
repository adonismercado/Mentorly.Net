namespace Mentorly.Application.DTOs;

public sealed record AdminEscalatedSubmissionDto(
    Guid SubmissionId,
    Guid EnrollmentId,
    Guid AuthorStudentId,
    string AuthorDisplayName,
    Guid CourseId,
    string CourseTitle,
    Guid ActivityId,
    string ActivityTitle,
    Mentorly.Domain.Enums.EvidenceType EvidenceType,
    string EvidenceContent,
    DateTime SubmittedAtUtc,
    DateTime EscalatedAtUtc,
    int PositiveReviews,
    int RejectedReviews);
