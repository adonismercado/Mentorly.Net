using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/admins/{adminId:guid}/analytics")]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    [HttpGet("overview")]
    public async Task<ActionResult<AnalyticsOverviewDto>> GetOverviewAsync(Guid adminId, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await analyticsService.GetOverviewAsync(adminId, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpGet("courses/{courseId:guid}/drop-off")]
    public Task<ActionResult<IReadOnlyList<DropOffDto>>> GetDropOffAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default)
        => GetCourseReportAsync(() => analyticsService.GetDropOffAsync(adminId, courseId, cancellationToken));

    [HttpGet("courses/{courseId:guid}/completion-time")]
    public Task<ActionResult<CompletionTimeReportDto>> GetCompletionTimeAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default)
        => GetCourseReportAsync(() => analyticsService.GetCompletionTimesAsync(adminId, courseId, cancellationToken));

    [HttpGet("courses/{courseId:guid}/peer-review-bottlenecks")]
    public Task<ActionResult<IReadOnlyList<PeerReviewBottleneckDto>>> GetBottlenecksAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default)
        => GetCourseReportAsync(() => analyticsService.GetPeerReviewBottlenecksAsync(adminId, courseId, cancellationToken));

    [HttpGet("courses/{courseId:guid}/enrollment-history")]
    public Task<ActionResult<IReadOnlyList<EnrollmentHistoryDto>>> GetHistoryAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default)
        => GetCourseReportAsync(() => analyticsService.GetEnrollmentHistoryAsync(adminId, courseId, cancellationToken));

    private static async Task<ActionResult<T>> GetCourseReportAsync<T>(Func<Task<T?>> getReportAsync) where T : class
    {
        try
        {
            var report = await getReportAsync();
            return report is null ? new NotFoundResult() : new OkObjectResult(report);
        }
        catch (InvalidOperationException exception)
        {
            return new ConflictObjectResult(new { message = exception.Message });
        }
    }
}
