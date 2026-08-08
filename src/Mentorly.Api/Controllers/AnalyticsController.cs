using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/admin/analytics")]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    [HttpGet("overview")]
    public async Task<ActionResult<AnalyticsOverviewDto>> GetOverviewAsync(CancellationToken cancellationToken = default) => Ok(await analyticsService.GetOverviewAsync(cancellationToken));
    [HttpGet("courses/{courseId:guid}/drop-off")]
    public async Task<ActionResult<IEnumerable<DropOffDto>>> GetDropOffAsync(Guid courseId, CancellationToken cancellationToken = default) { var result = await analyticsService.GetDropOffAsync(courseId, cancellationToken); return result is null ? NotFound() : Ok(result); }
    [HttpGet("courses/{courseId:guid}/completion-time")]
    public async Task<ActionResult<CompletionTimeReportDto>> GetCompletionTimeAsync(Guid courseId, CancellationToken cancellationToken = default) { var result = await analyticsService.GetCompletionTimesAsync(courseId, cancellationToken); return result is null ? NotFound() : Ok(result); }
    [HttpGet("courses/{courseId:guid}/peer-review-bottlenecks")]
    public async Task<ActionResult<IEnumerable<PeerReviewBottleneckDto>>> GetBottlenecksAsync(Guid courseId, CancellationToken cancellationToken = default) { var result = await analyticsService.GetPeerReviewBottlenecksAsync(courseId, cancellationToken); return result is null ? NotFound() : Ok(result); }
    [HttpGet("courses/{courseId:guid}/enrollment-history")]
    public async Task<ActionResult<IEnumerable<EnrollmentHistoryDto>>> GetHistoryAsync(Guid courseId, CancellationToken cancellationToken = default) { var result = await analyticsService.GetEnrollmentHistoryAsync(courseId, cancellationToken); return result is null ? NotFound() : Ok(result); }
}
