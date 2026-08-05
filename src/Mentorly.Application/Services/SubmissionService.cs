using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Domain.Entities;

namespace Mentorly.Application.Services;

public sealed class SubmissionService(
    ISubmissionRepository submissionRepository,
    IGamificationService gamificationService,
    IUnitOfWork unitOfWork) : ISubmissionService
{
    public async Task<IReadOnlyList<SubmissionDto>> GetAllSubmissionsAsync(CancellationToken cancellationToken = default)
    {
        var submissions = await submissionRepository.GetAllAsync(cancellationToken);

        return submissions.Select(s => new SubmissionDto(
            s.Id,
            s.EnrollmentId,
            s.ActivityId,
            s.EvidenceUrl,
            s.Status,
            s.SubmittedAt,
            s.ReviewedAt))
            .ToList();
    }

    public async Task<SubmissionDto?> GetSubmissionByIdAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetByIdAsync(submissionId, cancellationToken);

        if (submission is null)
        {
            return null;
        }

        return new SubmissionDto(
            submission.Id,
            submission.EnrollmentId,
            submission.ActivityId,
            submission.EvidenceUrl,
            submission.Status,
            submission.SubmittedAt,
            submission.ReviewedAt);
    }

    public async Task<SubmissionDto> CreateSubmissionAsync(CreateSubmissionDto dto, CancellationToken cancellationToken = default)
    {
        var submission = Submission.Create(
            dto.EnrollmentId,
            dto.ActivityId,
            dto.EvidenceUrl,
            DateTime.UtcNow);

        submissionRepository.Add(submission);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubmissionDto(
            submission.Id,
            submission.EnrollmentId,
            submission.ActivityId,
            submission.EvidenceUrl,
            submission.Status,
            submission.SubmittedAt,
            submission.ReviewedAt);
    }

    public async Task<bool> UpdateSubmissionAsync(Guid submissionId, UpdateSubmissionDto dto, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetByIdAsync(submissionId, cancellationToken);

        if (submission is null)
        {
            return false;
        }

        submission.ReplaceEvidence(dto.EvidenceUrl);

        submissionRepository.Update(submission);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteSubmissionAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetByIdAsync(submissionId, cancellationToken);

        if (submission is null)
        {
            return false;
        }

        submissionRepository.Delete(submission);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> EscalateAsync(Guid submissionId, Guid studentId, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetByIdWithContextAsync(submissionId, cancellationToken);
        if (submission is null || submission.Enrollment.StudentId != studentId)
        {
            return false;
        }

        submission.Escalate(DateTime.UtcNow);
        submissionRepository.Update(submission);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DecideAsAdminAsync(Guid submissionId, bool isApproved, CancellationToken cancellationToken = default)
    {
        var submission = await submissionRepository.GetByIdWithContextAsync(submissionId, cancellationToken);
        if (submission is null)
        {
            return false;
        }

        if (isApproved)
        {
            submission.Approve(DateTime.UtcNow);
        }
        else
        {
            submission.Reject(DateTime.UtcNow);
        }

        submissionRepository.Update(submission);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        if (isApproved)
        {
            await gamificationService.AwardAsync(submission.Enrollment.StudentId, Domain.Enums.GamificationEventType.ExerciseApproved, submission.Id, cancellationToken);
        }
        return true;
    }
}
