using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface IStudentEnrollmentService
{
    Task<EnrollmentResultDto> EnrollAsync(CreateEnrollmentRequestDto request, CancellationToken cancellationToken = default);

    Task<EnrollmentStatusDto?> GetEnrollmentStatusAsync(Guid enrollmentId, DateTime utcNow, CancellationToken cancellationToken = default);

    Task<SubmissionResultDto> SubmitExerciseAsync(SubmitExerciseRequestDto request, CancellationToken cancellationToken = default);
}
