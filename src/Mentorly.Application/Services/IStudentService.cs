using Mentorly.Application.DTOs;

namespace Mentorly.Application.Services;

public interface IStudentService
{
    Task<StudentDto[]> GetAllStudentsAsync(CancellationToken cancellationToken = default);
    Task<StudentProfileDto?> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<StudentProfileDto> ProvisionStudentAsync(ProvisionStudentDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateStudentAsync(Guid studentId, UpdateStudentDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateLeaderboardPrivacyAsync(Guid studentId, bool isLeaderboardPublic, CancellationToken cancellationToken = default);
    Task<StudentStatisticsDto?> GetStudentStatisticsAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<bool> PromoteToAdminAsync(Guid adminId, Guid studentId, CancellationToken cancellationToken = default);
}
