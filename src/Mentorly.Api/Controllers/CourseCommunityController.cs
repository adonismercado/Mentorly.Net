using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api/courses/{courseId:guid}")]
public class CourseCommunityController(ICourseCommunityService communityService) : ControllerBase
{
    [HttpGet("members")]
    public async Task<ActionResult<IEnumerable<CourseMemberDto>>> GetMembersAsync(Guid courseId, [FromQuery] bool includePrivate = false, CancellationToken cancellationToken = default)
    {
        var members = await communityService.GetMembersAsync(courseId, includePrivate, cancellationToken);
        return members is null ? NotFound() : Ok(members);
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<IEnumerable<LeaderboardEntryDto>>> GetLeaderboardAsync(Guid courseId, [FromQuery] bool includePrivate = false, CancellationToken cancellationToken = default)
    {
        var leaderboard = await communityService.GetLeaderboardAsync(courseId, includePrivate, cancellationToken);
        return leaderboard is null ? NotFound() : Ok(leaderboard);
    }

    [HttpGet("leaderboard/students/{studentId:guid}")]
    public async Task<ActionResult<LeaderboardEntryDto>> GetMyPositionAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default)
    {
        var position = await communityService.GetOwnPositionAsync(courseId, studentId, cancellationToken);
        return position is null ? NotFound() : Ok(position);
    }
}
