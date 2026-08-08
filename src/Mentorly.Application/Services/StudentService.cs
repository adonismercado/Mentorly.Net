using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.DTOs;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;

namespace Mentorly.Application.Services;

public sealed class StudentService(
    IStudentRepository studentRepository,
    IUnitOfWork unitOfWork) : IStudentService
{
    public async Task<StudentDto[]> GetAllStudentsAsync(CancellationToken cancellationToken = default)
    {
        var students = await studentRepository.GetAllAsync(cancellationToken);

        return students.Select(s => new StudentDto(
            s.Id,
            s.DisplayName,
            s.Role,
            s.IsLeaderboardPublic,
            s.TotalPoints))
            .ToArray();
    }

    public async Task<StudentProfileDto?> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var student = await studentRepository.GetByIdAsync(studentId, cancellationToken);

        if (student is null)
        {
            return null;
        }

        return new StudentProfileDto(
            student.Id,
            student.Email,
            student.DisplayName,
            student.Role,
            student.IsLeaderboardPublic,
            student.TotalPoints);
    }

    public async Task<StudentProfileDto> ProvisionStudentAsync(ProvisionStudentDto dto, CancellationToken cancellationToken = default)
    {
        var students = await studentRepository.GetAllAsync(cancellationToken);
        var student = students.FirstOrDefault(x => x.GoogleUserId == dto.GoogleUserId);

        if (student is null)
        {
            student = new Student(
                Guid.NewGuid(),
                dto.GoogleUserId,
                dto.Email,
                dto.DisplayName);

            studentRepository.Add(student);
        }
        else
        {
            student.UpdateProfile(dto.Email, dto.DisplayName);
            studentRepository.Update(student);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new StudentProfileDto(
            student.Id,
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

    public async Task<bool> PromoteToAdminAsync(Guid adminId, Guid studentId, CancellationToken cancellationToken = default)
    {
        var admin = await studentRepository.GetByIdAsync(adminId, cancellationToken);
        if (admin?.Role != StudentRole.Admin)
        {
            return false;
        }

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
