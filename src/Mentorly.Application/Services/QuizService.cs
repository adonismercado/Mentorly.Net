using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;

namespace Mentorly.Application.Services;

public interface IQuizService
{
    Task<QuizQuestionDto[]> GetQuestionsAsync(Guid activityId, CancellationToken cancellationToken = default);
    Task<AdminQuizQuestionDto[]?> GetAdminQuestionsAsync(Guid adminId, Guid activityId, CancellationToken cancellationToken = default);
    Task<QuizQuestionDto?> CreateQuestionAsync(Guid adminId, Guid activityId, CreateQuizQuestionDto dto, CancellationToken cancellationToken = default);
    Task<AdminQuizQuestionDto?> UpdateQuestionAsync(Guid adminId, Guid questionId, UpdateQuizQuestionDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteQuestionAsync(Guid adminId, Guid questionId, CancellationToken cancellationToken = default);
    Task<QuizAttemptDto?> SubmitAsync(Guid enrollmentId, Guid activityId, SubmitQuizAttemptDto dto, CancellationToken cancellationToken = default);
}

public sealed class QuizService(
    IQuizRepository quizRepository,
    IActivityRepository activityRepository,
    IEnrollmentRepository enrollmentRepository,
    IStudentRepository studentRepository,
    IPeerReviewWorkflowRepository peerReviewWorkflowRepository,
    ICourseCompletionService courseCompletionService,
    IGamificationService gamificationService,
    IUnitOfWork unitOfWork) : IQuizService
{
    public async Task<QuizQuestionDto[]> GetQuestionsAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        return (await quizRepository.GetQuestionsAsync(activityId, cancellationToken))
            .Select(question => new QuizQuestionDto(question.Id, question.Prompt, question.OrderIndex))
            .ToArray();
    }

    public async Task<AdminQuizQuestionDto[]?> GetAdminQuestionsAsync(Guid adminId, Guid activityId, CancellationToken cancellationToken = default)
    {
        await EnsureAdminAsync(adminId, cancellationToken);

        var activity = await activityRepository.GetByIdAsync(activityId, cancellationToken);
        if (activity is null || activity.Type != ActivityType.Quiz)
        {
            return null;
        }

        return (await quizRepository.GetQuestionsAsync(activityId, cancellationToken))
            .Select(MapAdminQuestion)
            .ToArray();
    }

    public async Task<QuizQuestionDto?> CreateQuestionAsync(Guid adminId, Guid activityId, CreateQuizQuestionDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureAdminAsync(adminId, cancellationToken);

        var activity = await activityRepository.GetByIdAsync(activityId, cancellationToken);
        if (activity is null || activity.Type != ActivityType.Quiz)
        {
            return null;
        }

        var question = new QuizQuestion(Guid.NewGuid(), activityId, dto.Prompt, dto.CorrectAnswer, dto.OrderIndex);
        quizRepository.AddQuestion(question);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new QuizQuestionDto(question.Id, question.Prompt, question.OrderIndex);
    }

    public async Task<AdminQuizQuestionDto?> UpdateQuestionAsync(Guid adminId, Guid questionId, UpdateQuizQuestionDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureAdminAsync(adminId, cancellationToken);

        var question = await quizRepository.GetQuestionByIdAsync(questionId, cancellationToken);
        if (question is null)
        {
            return null;
        }

        var activity = await activityRepository.GetByIdAsync(question.ActivityId, cancellationToken);
        if (activity is null || activity.Type != ActivityType.Quiz)
        {
            return null;
        }

        question.Update(dto.Prompt, dto.CorrectAnswer, dto.OrderIndex);
        quizRepository.UpdateQuestion(question);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapAdminQuestion(question);
    }

    public async Task<bool> DeleteQuestionAsync(Guid adminId, Guid questionId, CancellationToken cancellationToken = default)
    {
        await EnsureAdminAsync(adminId, cancellationToken);

        var question = await quizRepository.GetQuestionByIdAsync(questionId, cancellationToken);
        if (question is null)
        {
            return false;
        }

        var activity = await activityRepository.GetByIdAsync(question.ActivityId, cancellationToken);
        if (activity is null || activity.Type != ActivityType.Quiz)
        {
            return false;
        }

        quizRepository.DeleteQuestion(question);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<QuizAttemptDto?> SubmitAsync(Guid enrollmentId, Guid activityId, SubmitQuizAttemptDto dto, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken);
        if (enrollment is null || enrollment.StudentId != dto.StudentId)
        {
            return null;
        }

        if (!enrollment.CanSubmit(DateTime.UtcNow))
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException("Enrollment is inactive.");
        }

        var activity = await activityRepository.GetByIdAsync(activityId, cancellationToken);
        var workflow = await peerReviewWorkflowRepository.GetActivityAsync(activityId, cancellationToken);
        if (activity is null || activity.Type != ActivityType.Quiz || workflow is null || workflow.CourseId != enrollment.CourseId)
        {
            return null;
        }

        var questions = await quizRepository.GetQuestionsAsync(activityId, cancellationToken);
        if (questions.Count == 0)
        {
            throw new InvalidOperationException("Quiz has no questions.");
        }

        var correctAnswers = questions.Count(question => dto.Answers.Any(answer =>
            answer.QuestionId == question.Id &&
            string.Equals(answer.Answer.Trim(), question.CorrectAnswer, StringComparison.OrdinalIgnoreCase)));
        var score = Math.Round(correctAnswers * 100m / questions.Count, 2);
        var passed = score >= 70m;
        var attempt = new QuizAttempt(Guid.NewGuid(), enrollmentId, activityId, score, passed, DateTime.UtcNow);

        quizRepository.AddAttempt(attempt);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (passed)
        {
            await gamificationService.AwardAsync(dto.StudentId, GamificationEventType.ExerciseApproved, attempt.Id, cancellationToken);
            await courseCompletionService.EvaluateAsync(enrollmentId, cancellationToken);
        }

        return new QuizAttemptDto(attempt.Id, attempt.Score, attempt.Passed, attempt.SubmittedAt);
    }

    private async Task EnsureAdminAsync(Guid adminId, CancellationToken cancellationToken)
    {
        var admin = await studentRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin?.Role != StudentRole.Admin)
        {
            throw new InvalidOperationException("Only an administrator can manage quiz questions.");
        }
    }

    private static AdminQuizQuestionDto MapAdminQuestion(QuizQuestion question) =>
        new(question.Id, question.Prompt, question.CorrectAnswer, question.OrderIndex);
}
