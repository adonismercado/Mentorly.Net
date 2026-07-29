using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Domain.Entities;

namespace Mentorly.Application.Services;

public sealed class StudentEnrollmentService(
    IStudentRepository studentRepository,
    ICourseRepository courseRepository,
    IEnrollmentRepository enrollmentRepository,
    ISubmissionRepository submissionRepository,
    IUnitOfWork unitOfWork) : IStudentEnrollmentService
{
    public async Task<EnrollmentResultDto> EnrollAsync(CreateEnrollmentRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!await studentRepository.ExistsAsync(request.StudentId, cancellationToken))
        {
            throw new InvalidOperationException("Student not found.");
        }

        var course = await courseRepository.GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new InvalidOperationException("Course not found.");

        var hasActiveEnrollment = await enrollmentRepository.HasActiveEnrollmentAsync(
            request.StudentId,
            request.CourseId,
            request.StartedAtUtc,
            cancellationToken);

        if (hasActiveEnrollment)
        {
            throw new InvalidOperationException("The student already has an active enrollment for this course.");
        }

        var attemptNumber = await enrollmentRepository.GetNextAttemptNumberAsync(
            request.StudentId,
            request.CourseId,
            cancellationToken);

        var enrollment = Enrollment.CreateNew(request.StudentId, course.Id, attemptNumber, request.StartedAtUtc);

        await enrollmentRepository.AddAsync(enrollment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new EnrollmentResultDto(
            enrollment.Id,
            enrollment.StudentId,
            enrollment.CourseId,
            enrollment.AttemptNumber,
            enrollment.StartedAt,
            enrollment.ExpiresAt,
            enrollment.Status);
    }

    public async Task<EnrollmentStatusDto?> GetEnrollmentStatusAsync(Guid enrollmentId, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken);
        if (enrollment is null)
        {
            return null;
        }

        enrollment.RefreshStatus(utcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new EnrollmentStatusDto(
            enrollment.Id,
            enrollment.Status,
            enrollment.StartedAt,
            enrollment.ExpiresAt,
            enrollment.Status == Domain.Enums.EnrollmentStatus.Active);
    }

    public async Task<SubmissionResultDto> SubmitExerciseAsync(SubmitExerciseRequestDto request, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken)
            ?? throw new InvalidOperationException("Enrollment not found.");

        if (!enrollment.CanSubmit(request.SubmittedAtUtc))
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException("Enrollment is expired or inactive. Submission is not allowed.");
        }

        var existingSubmission = await submissionRepository.GetByEnrollmentAndActivityAsync(
            request.EnrollmentId,
            request.ActivityId,
            cancellationToken);

        if (existingSubmission is null)
        {
            var newSubmission = Submission.Create(
                request.EnrollmentId,
                request.ActivityId,
                request.EvidenceUrl,
                request.SubmittedAtUtc);

            await submissionRepository.AddAsync(newSubmission, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new SubmissionResultDto(
                newSubmission.Id,
                newSubmission.EnrollmentId,
                newSubmission.ActivityId,
                newSubmission.EvidenceUrl,
                newSubmission.SubmittedAt,
                newSubmission.Status);
        }

        existingSubmission.ReplaceEvidence(request.EvidenceUrl);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubmissionResultDto(
            existingSubmission.Id,
            existingSubmission.EnrollmentId,
            existingSubmission.ActivityId,
            existingSubmission.EvidenceUrl,
            existingSubmission.SubmittedAt,
            existingSubmission.Status);
    }
}
