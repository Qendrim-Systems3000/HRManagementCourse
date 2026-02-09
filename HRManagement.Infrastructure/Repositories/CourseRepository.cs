using HRManagement.Application.Interfaces;
using HRManagement.Domain.Entities;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Repositories;

public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context) { }

    public async Task<bool> CourseExistsAsync(string description, DateTime startDate)
        => await _dbSet.AnyAsync(c => c.Description == description && c.StartDate == startDate);

    public async Task<bool> HasEnrolledEmployeesAsync(int courseId)
        => await _context.Set<EmployeeCourse>().AnyAsync(ec => ec.CourseId == courseId);

    public async Task<IEnumerable<Course>> GetFilteredCoursesAsync(int? typeId, DateTime? date, bool? approved)
    {
        var query = _dbSet.AsQueryable();

        if (typeId.HasValue)
            query = query.Where(c => c.CourseTypeId == typeId.Value);

        if (date.HasValue)
            query = query.Where(c => c.StartDate.Date == date.Value.Date);

        if (approved.HasValue)
            query = query.Where(c => c.Approved == approved.Value);

        return await query.Include(c => c.CourseType).ToListAsync();
    }
}