using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;

namespace Mentorly.Application.Services;

public interface IGamificationService
{
    Task AwardAsync(Guid studentId, GamificationEventType type, Guid referenceId, CancellationToken cancellationToken = default);
}

public sealed class GamificationService(IStudentRepository studentRepository, IGamificationEventRepository eventRepository, IUnitOfWork unitOfWork) : IGamificationService
{
    public async Task AwardAsync(Guid studentId, GamificationEventType type, Guid referenceId, CancellationToken cancellationToken = default)
    {
        if (await eventRepository.ExistsAsync(studentId, type, referenceId, cancellationToken)) return;
        var student = await studentRepository.GetByIdAsync(studentId, cancellationToken) ?? throw new InvalidOperationException("Student not found.");
        var points = type switch { GamificationEventType.ThemeCompleted => 5, GamificationEventType.ExerciseSubmitted => 10, GamificationEventType.ExerciseApproved => 20, GamificationEventType.ConstructivePeerReview => 15, _ => throw new ArgumentOutOfRangeException(nameof(type)) };
        student.AddPoints(points);
        eventRepository.Add(new GamificationEvent(Guid.NewGuid(), studentId, type, referenceId, points, DateTime.UtcNow));
        studentRepository.Update(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
