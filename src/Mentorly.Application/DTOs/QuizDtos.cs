namespace Mentorly.Application.DTOs;
public sealed record CreateQuizQuestionDto(string Prompt,string CorrectAnswer,int OrderIndex);
public sealed record QuizQuestionDto(Guid Id,string Prompt,int OrderIndex);
public sealed record QuizAnswerDto(Guid QuestionId,string Answer);
public sealed record SubmitQuizAttemptDto(IReadOnlyList<QuizAnswerDto> Answers);
public sealed record QuizAttemptDto(Guid Id,decimal Score,bool Passed,DateTime SubmittedAtUtc);
