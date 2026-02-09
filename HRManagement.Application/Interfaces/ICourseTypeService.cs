using HRManagement.Domain.Entities;

namespace HRManagement.Application.Interfaces;

public interface ICourseTypeService
{
    Task<IEnumerable<CourseType>> GetAllAsync();
    Task<CourseType?> GetByIdAsync(int id);
    Task CreateAsync(CourseType courseType);
    Task UpdateAsync(CourseType courseType);
    Task DeleteAsync(int id);
}
