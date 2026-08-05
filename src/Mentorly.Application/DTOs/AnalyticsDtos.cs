using Mentorly.Domain.Enums;

namespace Mentorly.Application.DTOs;

public sealed record AnalyticsOverviewDto(int Courses, int ActiveEnrollments, int CompletedEnrollments, int ExpiredEnrollments, int PendingPeerReviewSubmissions);
public sealed record DropOffDto(Guid UnitId, string UnitTitle, Guid ThemeId, string ThemeTitle, int EnrollmentCount, int CompletionCount, decimal CompletionRate);
public sealed record CompletionTimeReportDto(double? CourseAverageDays, IReadOnlyList<UnitCompletionTimeDto> Units);
public sealed record UnitCompletionTimeDto(Guid UnitId, string UnitTitle, double? AverageDays);
public sealed record PeerReviewBottleneckDto(Guid ActivityId, string ActivityTitle, int PendingSubmissions, int EscalatedSubmissions, DateTime? OldestPendingAtUtc);
public sealed record EnrollmentHistoryDto(Guid EnrollmentId, Guid StudentId, int AttemptNumber, EnrollmentStatus Status, DateTime StartedAtUtc, DateTime ExpiresAtUtc, DateTime? CompletedAtUtc);
