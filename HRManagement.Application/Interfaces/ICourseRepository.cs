using HRManagement.Domain.Entities;

namespace HRManagement.Application.Interfaces;

public interface ICourseRepository : IGenericRepository<Course>
{
    // Requirement: Don't allow duplicate Course
    Task<bool> CourseExistsAsync(string description, DateTime startDate);
    
    // Requirement: Don't allow delete if used in Employee Course
    Task<bool> HasEnrolledEmployeesAsync(int courseId);

    // Specific filtering logic
    Task<IEnumerable<Course>> GetFilteredCoursesAsync(int? typeId, DateTime? date, bool? approved);
}