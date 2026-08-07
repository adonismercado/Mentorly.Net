using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;

namespace Mentorly.Application.Services;

public interface IGamificationService
{
    Task AwardAsync(Guid studentId, GamificationEventType type, Guid referenceId, CancellationToken cancellationToken = default);
}

public sealed class GamificationService(IStudentRepository studentRepository, IGamificationEventRepository eventRepository, IBadgeRepository badgeRepository, IUnitOfWork unitOfWork) : IGamificationService
{
    public async Task AwardAsync(Guid studentId, GamificationEventType type, Guid referenceId, CancellationToken cancellationToken = default)
    {
        if (await eventRepository.ExistsAsync(studentId, type, referenceId, cancellationToken)) return;
        var student = await studentRepository.GetByIdAsync(studentId, cancellationToken) ?? throw new InvalidOperationException("Student not found.");
        var points = type switch { GamificationEventType.ThemeCompleted => 5, GamificationEventType.ExerciseSubmitted => 10, GamificationEventType.ExerciseApproved => 20, GamificationEventType.ConstructivePeerReview => 15, _ => throw new ArgumentOutOfRangeException(nameof(type)) };
        student.AddPoints(points);
        eventRepository.Add(new GamificationEvent(Guid.NewGuid(), studentId, type, referenceId, points, DateTime.UtcNow));
        var badgeName = type switch { GamificationEventType.ThemeCompleted => "Explorer", GamificationEventType.ExerciseApproved => "Builder", GamificationEventType.ConstructivePeerReview => "Collaborator", _ => null };
        var badge = badgeName is null ? null : await badgeRepository.GetByNameAsync(badgeName, cancellationToken);
        if (badge is not null && !await badgeRepository.HasStudentBadgeAsync(studentId, badge.Id, cancellationToken)) badgeRepository.AddStudentBadge(new StudentBadge(studentId, badge.Id, DateTime.UtcNow));
        studentRepository.Update(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
