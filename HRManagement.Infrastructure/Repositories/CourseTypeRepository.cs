using HRManagement.Application.Interfaces;
using HRManagement.Domain.Entities;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Repositories
{
    public class CourseTypeRepository : GenericRepository<CourseType>, ICourseTypeRepository
{
    public CourseTypeRepository(ApplicationDbContext context) : base(context) { }

    public async Task<bool> ExistsByDescriptionAsync(string description, int? excludeId = null)
        => await _dbSet.AnyAsync(ct => ct.Description == description && (!excludeId.HasValue || ct.CourseTypeId != excludeId.Value));

    public async Task<bool> IsUsedInCoursesAsync(int id) 
        => await _context.Courses.AnyAsync(c => c.CourseTypeId == id);
}
}