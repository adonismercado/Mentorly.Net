using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface IAnalyticsService
{
    Task<AnalyticsOverviewDto> GetOverviewAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DropOffDto>?> GetDropOffAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<CompletionTimeReportDto?> GetCompletionTimesAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PeerReviewBottleneckDto>?> GetPeerReviewBottlenecksAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EnrollmentHistoryDto>?> GetEnrollmentHistoryAsync(Guid courseId, CancellationToken cancellationToken = default);
}

public sealed class AnalyticsService(IAnalyticsRepository repository) : IAnalyticsService
{
    public async Task<AnalyticsOverviewDto> GetOverviewAsync(CancellationToken c = default) { var x = await repository.GetOverviewAsync(c); return new(x.Courses, x.ActiveEnrollments, x.CompletedEnrollments, x.ExpiredEnrollments, x.PendingPeerReviewSubmissions); }
    public async Task<IReadOnlyList<DropOffDto>?> GetDropOffAsync(Guid courseId, CancellationToken c = default) { if (!await repository.CourseExistsAsync(courseId, c)) return null; return (await repository.GetDropOffAsync(courseId, c)).Select(x => new DropOffDto(x.UnitId, x.UnitTitle, x.ThemeId, x.ThemeTitle, x.EnrollmentCount, x.CompletionCount, x.EnrollmentCount == 0 ? 0 : Math.Round(x.CompletionCount * 100m / x.EnrollmentCount, 2))).ToList(); }
    public async Task<CompletionTimeReportDto?> GetCompletionTimesAsync(Guid courseId, CancellationToken c = default) { if (!await repository.CourseExistsAsync(courseId, c)) return null; var course = await repository.GetCourseAverageCompletionDaysAsync(courseId, c); var units = (await repository.GetUnitCompletionTimesAsync(courseId, c)).Select(x => new UnitCompletionTimeDto(x.UnitId, x.UnitTitle, x.AverageDays)).ToList(); return new(course, units); }
    public async Task<IReadOnlyList<PeerReviewBottleneckDto>?> GetPeerReviewBottlenecksAsync(Guid courseId, CancellationToken c = default) { if (!await repository.CourseExistsAsync(courseId, c)) return null; return (await repository.GetPeerReviewBottlenecksAsync(courseId, c)).Select(x => new PeerReviewBottleneckDto(x.ActivityId, x.ActivityTitle, x.PendingSubmissions, x.EscalatedSubmissions, x.OldestPendingAtUtc)).ToList(); }
    public async Task<IReadOnlyList<EnrollmentHistoryDto>?> GetEnrollmentHistoryAsync(Guid courseId, CancellationToken c = default) { if (!await repository.CourseExistsAsync(courseId, c)) return null; return (await repository.GetEnrollmentHistoryAsync(courseId, c)).Select(x => new EnrollmentHistoryDto(x.EnrollmentId, x.StudentId, x.AttemptNumber, x.Status, x.StartedAtUtc, x.ExpiresAtUtc, x.CompletedAtUtc)).ToList(); }
}
