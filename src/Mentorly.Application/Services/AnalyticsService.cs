using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Domain.Enums;

namespace Mentorly.Application.Services;

public interface IAnalyticsService
{
    Task<AnalyticsOverviewDto> GetOverviewAsync(Guid adminId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DropOffDto>?> GetDropOffAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default);
    Task<CompletionTimeReportDto?> GetCompletionTimesAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PeerReviewBottleneckDto>?> GetPeerReviewBottlenecksAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EnrollmentHistoryDto>?> GetEnrollmentHistoryAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default);
}

public sealed class AnalyticsService(
    IAnalyticsRepository repository,
    IStudentRepository studentRepository) : IAnalyticsService
{
    public async Task<AnalyticsOverviewDto> GetOverviewAsync(Guid adminId, CancellationToken cancellationToken = default)
    {
        await EnsureAdminAsync(adminId, cancellationToken);
        var data = await repository.GetOverviewAsync(cancellationToken);
        return new AnalyticsOverviewDto(data.Courses, data.ActiveEnrollments, data.CompletedEnrollments, data.ExpiredEnrollments, data.PendingPeerReviewSubmissions);
    }

    public async Task<IReadOnlyList<DropOffDto>?> GetDropOffAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default)
    {
        await EnsureAdminAsync(adminId, cancellationToken);
        if (!await repository.CourseExistsAsync(courseId, cancellationToken)) return null;

        return (await repository.GetDropOffAsync(courseId, cancellationToken))
            .Select(item => new DropOffDto(item.UnitId, item.UnitTitle, item.ThemeId, item.ThemeTitle, item.EnrollmentCount, item.CompletionCount, item.EnrollmentCount == 0 ? 0 : Math.Round(item.CompletionCount * 100m / item.EnrollmentCount, 2)))
            .ToList();
    }

    public async Task<CompletionTimeReportDto?> GetCompletionTimesAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default)
    {
        await EnsureAdminAsync(adminId, cancellationToken);
        if (!await repository.CourseExistsAsync(courseId, cancellationToken)) return null;

        var courseAverageDays = await repository.GetCourseAverageCompletionDaysAsync(courseId, cancellationToken);
        var units = (await repository.GetUnitCompletionTimesAsync(courseId, cancellationToken))
            .Select(item => new UnitCompletionTimeDto(item.UnitId, item.UnitTitle, item.AverageDays))
            .ToList();
        return new CompletionTimeReportDto(courseAverageDays, units);
    }

    public async Task<IReadOnlyList<PeerReviewBottleneckDto>?> GetPeerReviewBottlenecksAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default)
    {
        await EnsureAdminAsync(adminId, cancellationToken);
        if (!await repository.CourseExistsAsync(courseId, cancellationToken)) return null;

        return (await repository.GetPeerReviewBottlenecksAsync(courseId, cancellationToken))
            .Select(item => new PeerReviewBottleneckDto(item.ActivityId, item.ActivityTitle, item.PendingSubmissions, item.EscalatedSubmissions, item.OldestPendingAtUtc))
            .ToList();
    }

    public async Task<IReadOnlyList<EnrollmentHistoryDto>?> GetEnrollmentHistoryAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default)
    {
        await EnsureAdminAsync(adminId, cancellationToken);
        if (!await repository.CourseExistsAsync(courseId, cancellationToken)) return null;

        return (await repository.GetEnrollmentHistoryAsync(courseId, cancellationToken))
            .Select(item => new EnrollmentHistoryDto(item.EnrollmentId, item.StudentId, item.AttemptNumber, item.Status, item.StartedAtUtc, item.ExpiresAtUtc, item.CompletedAtUtc))
            .ToList();
    }

    private async Task EnsureAdminAsync(Guid adminId, CancellationToken cancellationToken)
    {
        var admin = await studentRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin?.Role != StudentRole.Admin)
        {
            throw new InvalidOperationException("Only an administrator can access analytics.");
        }
    }
}
