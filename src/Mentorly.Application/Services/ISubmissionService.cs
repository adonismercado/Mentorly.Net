using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface ISubmissionService
{
    Task<SubmissionDto[]> GetAllSubmissionsAsync(CancellationToken cancellationToken = default);
    Task<SubmissionDto[]> GetEscalatedSubmissionsAsync(Guid adminId, CancellationToken cancellationToken = default);
    Task<SubmissionDto?> GetSubmissionByIdAsync(Guid submissionId, CancellationToken cancellationToken = default);
    Task<SubmissionDto> CreateSubmissionAsync(Guid enrollmentId, Guid activityId, CreateSubmissionDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateSubmissionAsync(Guid submissionId, UpdateSubmissionDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteSubmissionAsync(Guid submissionId, CancellationToken cancellationToken = default);
    Task<bool> EscalateAsync(Guid submissionId, Guid studentId, CancellationToken cancellationToken = default);
    Task<bool> DecideAsAdminAsync(Guid adminId, Guid submissionId, bool isApproved, CancellationToken cancellationToken = default);
    Task<SubmissionDto[]> GetMySubmissionsAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<PeerReviewFeedbackDto[]?> GetMySubmissionReviewsAsync(Guid submissionId, Guid studentId, CancellationToken cancellationToken = default);
}
