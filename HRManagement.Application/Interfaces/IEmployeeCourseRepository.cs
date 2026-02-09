using HRManagement.Domain.Entities;

namespace HRManagement.Application.Interfaces;

public interface IEmployeeCourseRepository : IGenericRepository<EmployeeCourse>
{
    Task<IEnumerable<EmployeeCourse>> GetByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<EmployeeCourse>> GetFilteredAsync(int? employeeId, int? courseId);
}