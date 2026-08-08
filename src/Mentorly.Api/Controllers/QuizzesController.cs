using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mentorly.Api.Controllers;

[ApiController]
[Route("api")]
public class QuizzesController(IQuizService quizService) : ControllerBase
{
    [HttpGet("activities/{activityId:guid}/quiz")]
    public async Task<ActionResult<QuizQuestionDto[]>> GetQuestionsAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        return Ok(await quizService.GetQuestionsAsync(activityId, cancellationToken));
    }

    [HttpPost("admins/{adminId:guid}/activities/{activityId:guid}/quiz/questions")]
    public async Task<ActionResult<QuizQuestionDto>> CreateQuestionAsync(Guid adminId, Guid activityId, CreateQuizQuestionDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var question = await quizService.CreateQuestionAsync(adminId, activityId, dto, cancellationToken);
            return question is null
                ? NotFound()
                : CreatedAtAction(nameof(GetQuestionsAsync), new { activityId }, question);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPost("enrollments/{enrollmentId:guid}/activities/{activityId:guid}/quiz-attempts")]
    public async Task<ActionResult<QuizAttemptDto>> SubmitAttemptAsync(Guid enrollmentId, Guid activityId, SubmitQuizAttemptDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var attempt = await quizService.SubmitAsync(enrollmentId, activityId, dto, cancellationToken);
            return attempt is null ? NotFound() : Ok(attempt);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
