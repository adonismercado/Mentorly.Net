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
            s.DisplayName))
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
            student.DisplayName);
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
            student.DisplayName);
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
}
