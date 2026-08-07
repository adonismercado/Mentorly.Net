using Mentorly.Domain.Enums;

namespace Mentorly.Application.Abstractions.Persistence;

public sealed record AnalyticsOverviewData(int Courses, int ActiveEnrollments, int CompletedEnrollments, int ExpiredEnrollments, int PendingPeerReviewSubmissions);
public sealed record DropOffData(Guid UnitId, string UnitTitle, Guid ThemeId, string ThemeTitle, int EnrollmentCount, int CompletionCount);
public sealed record CompletionTimeData(Guid UnitId, string UnitTitle, double? AverageDays);
public sealed record PeerReviewBottleneckData(Guid ActivityId, string ActivityTitle, int PendingSubmissions, int EscalatedSubmissions, DateTime? OldestPendingAtUtc);
public sealed record EnrollmentHistoryData(Guid EnrollmentId, Guid StudentId, int AttemptNumber, EnrollmentStatus Status, DateTime StartedAtUtc, DateTime ExpiresAtUtc, DateTime? CompletedAtUtc);

public interface IAnalyticsRepository
{
    Task<AnalyticsOverviewData> GetOverviewAsync(CancellationToken cancellationToken = default);
    Task<bool> CourseExistsAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DropOffData>> GetDropOffAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<double?> GetCourseAverageCompletionDaysAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CompletionTimeData>> GetUnitCompletionTimesAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PeerReviewBottleneckData>> GetPeerReviewBottlenecksAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EnrollmentHistoryData>> GetEnrollmentHistoryAsync(Guid courseId, CancellationToken cancellationToken = default);
}
