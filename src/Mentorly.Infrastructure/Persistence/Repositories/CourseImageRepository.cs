using Mentorly.Application.Abstractions.Persistence;
using Mentorly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mentorly.Infrastructure.Persistence.Repositories;
public sealed class CourseImageRepository(MentorlyDbContext dbContext) : ICourseImageRepository
{ public async Task<IReadOnlyList<CourseImage>> GetByCourseIdAsync(Guid courseId, CancellationToken c = default) => await dbContext.CourseImages.AsNoTracking().Where(x => x.CourseId == courseId).OrderBy(x => x.OrderIndex).ToListAsync(c); public Task<CourseImage?> GetByIdAsync(Guid id, CancellationToken c = default) => dbContext.CourseImages.FirstOrDefaultAsync(x => x.Id == id, c); public void Add(CourseImage image) => dbContext.CourseImages.Add(image); public void Update(CourseImage image) => dbContext.CourseImages.Update(image); public void Delete(CourseImage image) => dbContext.CourseImages.Remove(image); }
