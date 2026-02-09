using HRManagement.Domain.Entities;

namespace HRManagement.Application.Interfaces;

// Inherit everything from Generic and only add what is unique to CourseType
public interface ICourseTypeRepository : IGenericRepository<CourseType>
{
    Task<bool> ExistsByDescriptionAsync(string description, int? excludeId = null);
    Task<bool> IsUsedInCoursesAsync(int id);
}