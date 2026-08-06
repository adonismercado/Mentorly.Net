using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface ISubmissionService
{
    Task<IReadOnlyList<SubmissionDto>> GetAllSubmissionsAsync(CancellationToken cancellationToken = default);
    Task<SubmissionDto?> GetSubmissionByIdAsync(Guid submissionId, CancellationToken cancellationToken = default);
    Task<SubmissionDto> CreateSubmissionAsync(CreateSubmissionDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateSubmissionAsync(Guid submissionId, UpdateSubmissionDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteSubmissionAsync(Guid submissionId, CancellationToken cancellationToken = default);
    Task<bool> EscalateAsync(Guid submissionId, Guid studentId, CancellationToken cancellationToken = default);
    Task<bool> DecideAsAdminAsync(Guid submissionId, bool isApproved, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SubmissionDto>> GetMySubmissionsAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PeerReviewFeedbackDto>?> GetMySubmissionReviewsAsync(Guid submissionId, Guid studentId, CancellationToken cancellationToken = default);
}
