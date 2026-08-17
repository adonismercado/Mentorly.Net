using Mentorly.Application.Abstractions.Persistence; using Mentorly.Domain.Entities; using Microsoft.EntityFrameworkCore;
namespace Mentorly.Infrastructure.Persistence.Repositories;
public sealed class QuizRepository(MentorlyDbContext db) : IQuizRepository
{
    public async Task<IReadOnlyList<QuizQuestion>> GetQuestionsAsync(Guid activityId, CancellationToken c = default) =>
        await db.QuizQuestions
            .Where(x => x.ActivityId == activityId)
            .OrderBy(x => x.OrderIndex)
            .ToListAsync(c);

    public Task<QuizQuestion?> GetQuestionByIdAsync(Guid questionId, CancellationToken c = default) =>
        db.QuizQuestions.FirstOrDefaultAsync(x => x.Id == questionId, c);

    public Task<QuizAttempt?> GetLatestAttemptAsync(Guid enrollmentId, Guid activityId, CancellationToken c = default) =>
        db.QuizAttempts
            .Where(x => x.EnrollmentId == enrollmentId && x.ActivityId == activityId)
            .OrderByDescending(x => x.SubmittedAt)
            .FirstOrDefaultAsync(c);

    public async Task<IReadOnlySet<Guid>> GetPassedActivityIdsAsync(Guid enrollmentId, IReadOnlyCollection<Guid> ids, CancellationToken c = default) =>
        await db.QuizAttempts
            .Where(x => x.EnrollmentId == enrollmentId && x.Passed && ids.Contains(x.ActivityId))
            .Select(x => x.ActivityId)
            .ToHashSetAsync(c);

    public void AddQuestion(QuizQuestion question) => db.QuizQuestions.Add(question);
    public void UpdateQuestion(QuizQuestion question) => db.QuizQuestions.Update(question);
    public void DeleteQuestion(QuizQuestion question) => db.QuizQuestions.Remove(question);
    public void AddAttempt(QuizAttempt attempt) => db.QuizAttempts.Add(attempt);
}
