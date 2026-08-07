using Mentorly.Domain.Entities;
namespace Mentorly.Application.Abstractions.Persistence;
public interface IQuizRepository { Task<IReadOnlyList<QuizQuestion>> GetQuestionsAsync(Guid activityId, CancellationToken cancellationToken=default); Task<QuizAttempt?> GetLatestAttemptAsync(Guid enrollmentId, Guid activityId, CancellationToken cancellationToken=default); Task<IReadOnlySet<Guid>> GetPassedActivityIdsAsync(Guid enrollmentId, IReadOnlyCollection<Guid> activityIds, CancellationToken cancellationToken=default); void AddQuestion(QuizQuestion question); void AddAttempt(QuizAttempt attempt); }
