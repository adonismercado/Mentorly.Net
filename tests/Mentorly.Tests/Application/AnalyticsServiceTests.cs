using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.Services;
using Mentorly.Domain.Enums;

namespace Mentorly.Tests.Application;

public sealed class AnalyticsServiceTests
{
    [Fact]
    public async Task GetDropOffAsync_CalculatesCompletionRate()
    {
        var courseId = Guid.NewGuid();
        var service = new AnalyticsService(new FakeAnalyticsRepository(courseId));

        var report = await service.GetDropOffAsync(courseId);

        var item = Assert.Single(report!);
        Assert.Equal(50m, item.CompletionRate);
    }

    [Fact]
    public async Task GetCompletionTimesAsync_ReturnsControlledValues()
    {
        var courseId = Guid.NewGuid();
        var service = new AnalyticsService(new FakeAnalyticsRepository(courseId));

        var report = await service.GetCompletionTimesAsync(courseId);

        Assert.Equal(12.5d, report!.CourseAverageDays);
        Assert.Equal(4d, Assert.Single(report.Units).AverageDays);
    }

    private sealed class FakeAnalyticsRepository(Guid courseId) : IAnalyticsRepository
    {
        public Task<AnalyticsOverviewData> GetOverviewAsync(CancellationToken cancellationToken = default) => Task.FromResult(new AnalyticsOverviewData(1, 2, 3, 4, 5));
        public Task<bool> CourseExistsAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(id == courseId);
        public Task<IReadOnlyList<DropOffData>> GetDropOffAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<DropOffData>>([new DropOffData(Guid.NewGuid(), "Unit", Guid.NewGuid(), "Theme", 10, 5)]);
        public Task<double?> GetCourseAverageCompletionDaysAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<double?>(12.5d);
        public Task<IReadOnlyList<CompletionTimeData>> GetUnitCompletionTimesAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<CompletionTimeData>>([new CompletionTimeData(Guid.NewGuid(), "Unit", 4d)]);
        public Task<IReadOnlyList<PeerReviewBottleneckData>> GetPeerReviewBottlenecksAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<PeerReviewBottleneckData>>([]);
        public Task<IReadOnlyList<EnrollmentHistoryData>> GetEnrollmentHistoryAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<EnrollmentHistoryData>>([]);
    }
}
