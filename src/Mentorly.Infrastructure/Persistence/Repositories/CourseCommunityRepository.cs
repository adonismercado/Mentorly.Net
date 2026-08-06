using Mentorly.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;

public sealed class CourseCommunityRepository(MentorlyDbContext dbContext) : ICourseCommunityRepository
{
    public Task<bool> CourseExistsAsync(Guid courseId, CancellationToken cancellationToken = default) => dbContext.Courses.AnyAsync(x => x.Id == courseId, cancellationToken);
    public Task<bool> IsStudentEnrolledAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default) => dbContext.Enrollments.AnyAsync(x => x.CourseId == courseId && x.StudentId == studentId, cancellationToken);
    public Task<IReadOnlyList<CourseCommunityStudentData>> GetStudentsAsync(Guid courseId, bool includePrivate, CancellationToken cancellationToken = default) => GetStudentsQuery(courseId, includePrivate).ToListAsync(cancellationToken).ContinueWith(x => (IReadOnlyList<CourseCommunityStudentData>)x.Result, cancellationToken);
    public Task<IReadOnlyList<CourseCommunityStudentData>> GetAllStudentsAsync(Guid courseId, CancellationToken cancellationToken = default) => GetStudentsQuery(courseId, true).ToListAsync(cancellationToken).ContinueWith(x => (IReadOnlyList<CourseCommunityStudentData>)x.Result, cancellationToken);

    private IQueryable<CourseCommunityStudentData> GetStudentsQuery(Guid courseId, bool includePrivate) =>
        dbContext.Enrollments.Where(x => x.CourseId == courseId)
            .Select(x => x.Student)
            .Distinct()
            .Where(x => includePrivate || x.IsLeaderboardPublic)
            .Select(x => new CourseCommunityStudentData(x.Id, x.DisplayName, x.TotalPoints, x.IsLeaderboardPublic));
}
