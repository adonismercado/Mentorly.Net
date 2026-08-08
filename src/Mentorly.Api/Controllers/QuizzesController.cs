using Mentorly.Application.DTOs;
using Mentorly.Application.Services;
using Microsoft.AspNetCore.Mvc;
namespace Mentorly.Api.Controllers;
[ApiController][Route("api")]
public class QuizzesController(IQuizService quizService):ControllerBase
{ [HttpGet("activities/{activityId:guid}/quiz")] public async Task<ActionResult<IEnumerable<QuizQuestionDto>>> GetAsync(Guid activityId,CancellationToken c=default)=>Ok(await quizService.GetQuestionsAsync(activityId,c)); [HttpPost("activities/{activityId:guid}/quiz/questions")] public async Task<ActionResult<QuizQuestionDto>> CreateAsync(Guid activityId,CreateQuizQuestionDto dto,CancellationToken c=default){var q=await quizService.CreateQuestionAsync(activityId,dto,c);return q is null?NotFound():CreatedAtAction(nameof(GetAsync),new{activityId},q);}[HttpPost("students/{studentId:guid}/enrollments/{enrollmentId:guid}/activities/{activityId:guid}/quiz-attempts")] public async Task<ActionResult<QuizAttemptDto>> SubmitAsync(Guid studentId, Guid enrollmentId,Guid activityId,SubmitQuizAttemptDto dto,CancellationToken c=default){try{var attempt=await quizService.SubmitAsync(enrollmentId,studentId,activityId,dto,c);return attempt is null?NotFound():Ok(attempt);}catch(InvalidOperationException e){return Conflict(new{message=e.Message});}} }
