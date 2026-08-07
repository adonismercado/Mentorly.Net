using System.Security.Claims;
using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Mentorly.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Authorize(Policy = MentorlyPolicies.Student)]
[Route("api/courses/{courseId:guid}")]
public class CourseCommunityController(ICourseCommunityService communityService) : ControllerBase
{
    [HttpGet("members")]
    public async Task<ActionResult<IEnumerable<CourseMemberDto>>> GetMembersAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var members = await communityService.GetMembersAsync(courseId, User.IsInRole(MentorlyRoles.Admin), cancellationToken);
        return members is null ? NotFound() : Ok(members);
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<IEnumerable<LeaderboardEntryDto>>> GetLeaderboardAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var leaderboard = await communityService.GetLeaderboardAsync(courseId, User.IsInRole(MentorlyRoles.Admin), cancellationToken);
        return leaderboard is null ? NotFound() : Ok(leaderboard);
    }

    [HttpGet("leaderboard/me")]
    public async Task<ActionResult<LeaderboardEntryDto>> GetMyPositionAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(MentorlyClaimTypes.StudentId), out var studentId)) return Unauthorized();
        var position = await communityService.GetOwnPositionAsync(courseId, studentId, cancellationToken);
        return position is null ? NotFound() : Ok(position);
    }
}
