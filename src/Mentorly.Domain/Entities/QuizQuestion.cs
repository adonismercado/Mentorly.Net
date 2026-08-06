namespace Mentorly.Domain.Entities;
public class QuizQuestion
{
    private QuizQuestion() { }
    public QuizQuestion(Guid id, Guid activityId, string prompt, string correctAnswer, int orderIndex) { if (id == Guid.Empty || activityId == Guid.Empty) throw new ArgumentException("Ids are required."); if (string.IsNullOrWhiteSpace(prompt) || string.IsNullOrWhiteSpace(correctAnswer) || orderIndex <= 0) throw new ArgumentException("Question data is invalid."); Id=id; ActivityId=activityId; Prompt=prompt.Trim(); CorrectAnswer=correctAnswer.Trim(); OrderIndex=orderIndex; }
    public Guid Id { get; private set; } public Guid ActivityId { get; private set; } public string Prompt { get; private set; } = string.Empty; public string CorrectAnswer { get; private set; } = string.Empty; public int OrderIndex { get; private set; }
}
