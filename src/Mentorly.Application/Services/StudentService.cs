using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Domain.Entities;

namespace Mentorly.Application.Services;

public sealed class StudentService(
    IStudentRepository studentRepository,
    IUnitOfWork unitOfWork) : IStudentService
{
    public async Task<IReadOnlyList<StudentDto>> GetAllStudentsAsync(CancellationToken cancellationToken = default)
    {
        var students = await studentRepository.GetAllAsync(cancellationToken);

        return students.Select(s => new StudentDto(
            s.Id,
            s.GoogleUserId,
            s.Email,
            s.DisplayName,
            s.Role,
            s.IsLeaderboardPublic,
            s.TotalPoints))
            .ToList();
    }

    public async Task<StudentDto?> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var student = await studentRepository.GetByIdAsync(studentId, cancellationToken);

        if (student is null)
        {
            return null;
        }

        return new StudentDto(
            student.Id,
            student.GoogleUserId,
            student.Email,
            student.DisplayName,
            student.Role,
            student.IsLeaderboardPublic,
            student.TotalPoints);
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = new Student(
            Guid.NewGuid(),
            dto.GoogleUserId,
            dto.Email,
            dto.DisplayName);

        studentRepository.Add(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new StudentDto(
            student.Id,
            student.GoogleUserId,
            student.Email,
            student.DisplayName,
            student.Role,
            student.IsLeaderboardPublic,
            student.TotalPoints);
    }

    public async Task<bool> UpdateStudentAsync(Guid studentId, UpdateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = await studentRepository.GetByIdAsync(studentId, cancellationToken);

        if (student is null)
        {
            return false;
        }

        student.UpdateProfile(dto.Email, dto.DisplayName);

        studentRepository.Update(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var student = await studentRepository.GetByIdAsync(studentId, cancellationToken);

        if (student is null)
        {
            return false;
        }

        studentRepository.Delete(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> UpdateLeaderboardPrivacyAsync(Guid studentId, bool isLeaderboardPublic, CancellationToken cancellationToken = default)
    {
        var student = await studentRepository.GetByIdAsync(studentId, cancellationToken);
        if (student is null)
        {
            return false;
        }

        student.SetLeaderboardVisibility(isLeaderboardPublic);
        studentRepository.Update(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<StudentStatisticsDto?> GetStudentStatisticsAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var student = await studentRepository.GetByIdWithBadgesAsync(studentId, cancellationToken);
        if (student is null)
        {
            return null;
        }

        var badges = student.StudentBadges
            .OrderBy(x => x.GrantedAt)
            .Select(x => new BadgeDto(
                x.Badge.Id,
                x.Badge.Name,
                x.Badge.Description,
                x.Badge.ImageUrl,
                x.GrantedAt))
            .ToList();

        return new StudentStatisticsDto(
            student.Id,
            student.Role,
            student.IsLeaderboardPublic,
            student.TotalPoints,
            badges);
    }

    public async Task<bool> PromoteToAdminAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var student = await studentRepository.GetByIdAsync(studentId, cancellationToken);
        if (student is null)
        {
            return false;
        }

        student.PromoteToAdmin();
        studentRepository.Update(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
