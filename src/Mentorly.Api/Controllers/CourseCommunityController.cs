using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api")]
public class CourseCommunityController(ICourseCommunityService communityService) : ControllerBase
{
    [HttpGet("courses/{courseId:guid}/members")]
    public async Task<ActionResult<IEnumerable<CourseMemberDto>>> GetMembersAsync(Guid courseId, [FromQuery] Guid viewerStudentId, CancellationToken cancellationToken = default)
    {
        var members = await communityService.GetMembersAsync(courseId, viewerStudentId, cancellationToken);
        return members is null ? NotFound() : Ok(members);
    }

    [HttpGet("courses/{courseId:guid}/leaderboard")]
    public async Task<ActionResult<IEnumerable<LeaderboardEntryDto>>> GetLeaderboardAsync(Guid courseId, [FromQuery] Guid viewerStudentId, CancellationToken cancellationToken = default)
    {
        var leaderboard = await communityService.GetLeaderboardAsync(courseId, viewerStudentId, cancellationToken);
        return leaderboard is null ? NotFound() : Ok(leaderboard);
    }

    [HttpGet("courses/{courseId:guid}/leaderboard/{studentId:guid}")]
    public async Task<ActionResult<LeaderboardEntryDto>> GetMyPositionAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default)
    {
        var position = await communityService.GetOwnPositionAsync(courseId, studentId, cancellationToken);
        return position is null ? NotFound() : Ok(position);
    }

    [HttpGet("admins/{adminId:guid}/courses/{courseId:guid}/leaderboard")]
    public async Task<ActionResult<IEnumerable<LeaderboardEntryDto>>> GetAdminLeaderboardAsync(Guid adminId, Guid courseId, CancellationToken cancellationToken = default)
    {
        try
        {
            var leaderboard = await communityService.GetAdminLeaderboardAsync(adminId, courseId, cancellationToken);
            return leaderboard is null ? NotFound() : Ok(leaderboard);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
