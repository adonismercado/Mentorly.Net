using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Application.Services;
using Mentorly.Domain.Entities;
using Mentorly.Domain.Enums;

namespace Mentorly.Tests.Application;

public sealed class EnrollmentProgressServiceTests
{
    [Fact]
    public async Task CompleteThemeAsync_EmitsCertificate_WhenAllRequirementsAreComplete()
    {
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var themeId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var enrollment = Enrollment.CreateNew(studentId, courseId, 1, DateTime.UtcNow);
        var service = CreateService(enrollment, [themeId], [activityId], new HashSet<Guid> { activityId });

        var progress = await service.CompleteThemeAsync(enrollment.Id, studentId, themeId);

        Assert.NotNull(progress);
        Assert.True(progress.IsCompleted);
        Assert.Equal(EnrollmentStatus.Completed, enrollment.Status);
        Assert.Equal($"/api/enrollments/{enrollment.Id}/certificate", enrollment.CertificateUrl);
        Assert.NotNull(enrollment.CompletedAt);
    }

    [Fact]
    public async Task RestartAsync_CreatesNextAttempt_ForExpiredEnrollment()
    {
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var enrollment = Enrollment.CreateNew(studentId, courseId, 1, DateTime.UtcNow.AddMonths(-4));
        enrollment.RefreshStatus(DateTime.UtcNow);
        var service = CreateService(enrollment, [], [], new HashSet<Guid>());

        var restarted = await service.RestartAsync(studentId, courseId);

        Assert.NotNull(restarted);
        Assert.Equal(2, restarted.AttemptNumber);
        Assert.Equal(EnrollmentStatus.Active, restarted.Status);
    }

    [Fact]
    public async Task CompleteThemeAsync_DoesNotEmitCertificate_WhenMandatoryExerciseIsPending()
    {
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var themeId = Guid.NewGuid();
        var enrollment = Enrollment.CreateNew(studentId, courseId, 1, DateTime.UtcNow);
        var service = CreateService(enrollment, [themeId], [Guid.NewGuid()], new HashSet<Guid>());

        var progress = await service.CompleteThemeAsync(enrollment.Id, studentId, themeId);

        Assert.NotNull(progress);
        Assert.False(progress.IsCompleted);
        Assert.Equal(EnrollmentStatus.Active, enrollment.Status);
        Assert.Null(enrollment.CertificateUrl);
    }

    private static EnrollmentProgressService CreateService(Enrollment enrollment, IReadOnlyList<Guid> themes, IReadOnlyList<Guid> activities, IReadOnlySet<Guid> approved)
    {
        return new EnrollmentProgressService(
            new FakeEnrollmentRepository(enrollment),
            new FakeCourseRepository(enrollment.CourseId),
            new FakeThemeCompletionRepository(),
            new FakeProgressRepository(enrollment.CourseId, themes, activities),
            new FakeSubmissionRepository(approved),
            new FakeQuizRepository(),
            new CertificateService(),
            new FakeGamificationService(),
            new FakeUnitOfWork());
    }

    private sealed class FakeEnrollmentRepository(Enrollment enrollment) : IEnrollmentRepository
    {
        public Task<Enrollment[]> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<Enrollment[]>([enrollment]);
        public Task<Enrollment?> GetByIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default) => Task.FromResult<Enrollment?>(enrollmentId == enrollment.Id ? enrollment : null);
        public Task<Enrollment[]> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult<Enrollment[]>(studentId == enrollment.StudentId ? [enrollment] : []);
        public Task<Enrollment?> GetLatestByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default) => Task.FromResult<Enrollment?>(studentId == enrollment.StudentId && courseId == enrollment.CourseId ? enrollment : null);
        public Task<bool> HasActiveEnrollmentAsync(Guid studentId, Guid courseId, DateTime utcNow, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<int> GetNextAttemptNumberAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default) => Task.FromResult(enrollment.AttemptNumber + 1);
        public Task AddAsync(Enrollment value, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Add(Enrollment value) { }
    }

    private sealed class FakeCourseRepository(Guid courseId) : ICourseRepository
    {
        private readonly Course _course = new(courseId, "Course", "Description", Guid.NewGuid(), 1);
        public Task<Course[]> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<Course[]>([_course]);
        public Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Course?>(id == courseId ? _course : null);
        public void Add(Course course) { } public void Update(Course course) { } public void Delete(Course course) { }
    }

    private sealed class FakeThemeCompletionRepository : IThemeCompletionRepository
    {
        private readonly List<ThemeCompletion> _items = [];
        public Task<IReadOnlyList<ThemeCompletion>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ThemeCompletion>>(_items.Where(x => x.EnrollmentId == enrollmentId).ToList());
        public Task<bool> ExistsAsync(Guid enrollmentId, Guid themeId, CancellationToken cancellationToken = default) => Task.FromResult(_items.Any(x => x.EnrollmentId == enrollmentId && x.ThemeId == themeId));
        public void Add(ThemeCompletion completion) => _items.Add(completion);
    }

    private sealed class FakeProgressRepository(Guid courseId, IReadOnlyList<Guid> themes, IReadOnlyList<Guid> activities) : IEnrollmentProgressRepository
    {
        public Task<bool> ThemeBelongsToCourseAsync(Guid themeId, Guid requestedCourseId, CancellationToken cancellationToken = default) => Task.FromResult(requestedCourseId == courseId && themes.Contains(themeId));
        public Task<IReadOnlyList<Guid>> GetThemeIdsAsync(Guid requestedCourseId, CancellationToken cancellationToken = default) => Task.FromResult(requestedCourseId == courseId ? themes : (IReadOnlyList<Guid>)[]);
        public Task<IReadOnlyList<Guid>> GetMandatoryActivityIdsAsync(Guid requestedCourseId, CancellationToken cancellationToken = default) => Task.FromResult(requestedCourseId == courseId ? activities : (IReadOnlyList<Guid>)[]);
    }

    private sealed class FakeSubmissionRepository(IReadOnlySet<Guid> approved) : ISubmissionRepository
    {
        public Task<Submission[]> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<Submission[]>([]);
        public Task<Submission?> GetByIdAsync(Guid submissionId, CancellationToken cancellationToken = default) => Task.FromResult<Submission?>(null);
        public Task<Submission?> GetByIdWithContextAsync(Guid submissionId, CancellationToken cancellationToken = default) => Task.FromResult<Submission?>(null);
        public Task<Submission?> GetByEnrollmentAndActivityAsync(Guid enrollmentId, Guid activityId, CancellationToken cancellationToken = default) => Task.FromResult<Submission?>(null);
        public Task<bool> HasStudentSubmittedActivityAsync(Guid studentId, Guid activityId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> HasSubmissionsForActivityAsync(Guid activityId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<IReadOnlySet<Guid>> GetApprovedActivityIdsAsync(Guid enrollmentId, IReadOnlyCollection<Guid> activityIds, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlySet<Guid>>(approved);
        public Task<Submission[]> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default) => Task.FromResult<Submission[]>([]);
        public Task AddAsync(Submission submission, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task DeleteAsync(Submission submission, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeQuizRepository : IQuizRepository
    {
        public Task<IReadOnlyList<QuizQuestion>> GetQuestionsAsync(Guid activityId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<QuizQuestion>>([]);
        public Task<QuizAttempt?> GetLatestAttemptAsync(Guid enrollmentId, Guid activityId, CancellationToken cancellationToken = default) => Task.FromResult<QuizAttempt?>(null);
        public Task<IReadOnlySet<Guid>> GetPassedActivityIdsAsync(Guid enrollmentId, IReadOnlyCollection<Guid> activityIds, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlySet<Guid>>(new HashSet<Guid>());
        public void AddQuestion(QuizQuestion question) { } public void AddAttempt(QuizAttempt attempt) { }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork { public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1); }
    private sealed class FakeGamificationService : IGamificationService { public Task AwardAsync(Guid studentId, GamificationEventType type, Guid referenceId, CancellationToken cancellationToken = default) => Task.CompletedTask; }
}
