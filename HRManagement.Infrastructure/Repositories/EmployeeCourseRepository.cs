using HRManagement.Application.Interfaces;
using HRManagement.Domain.Entities;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Repositories;

public class EmployeeCourseRepository : GenericRepository<EmployeeCourse>, IEmployeeCourseRepository
{
    public EmployeeCourseRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<EmployeeCourse>> GetByEmployeeIdAsync(int employeeId)
    {
        return await _dbSet
            .Include(ec => ec.Course)
            .Where(ec => ec.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<EmployeeCourse>> GetFilteredAsync(int? employeeId, int? courseId)
    {
        var query = _dbSet.Include(ec => ec.Course).Include(ec => ec.Employee).AsQueryable();
        if (employeeId.HasValue) query = query.Where(ec => ec.EmployeeId == employeeId.Value);
        if (courseId.HasValue) query = query.Where(ec => ec.CourseId == courseId.Value);
        return await query.ToListAsync();
    }
}