using HRManagement.Domain.Entities;

namespace HRManagement.Application.Interfaces;

public interface ICourseService
{
    // Retrieval
    Task<IEnumerable<Course>> GetCoursesAsync(int? typeId, DateTime? date, bool? approved);
    Task<Course?> GetCourseByIdAsync(int id);

    // Commands
    Task CreateCourseAsync(Course course);
    Task UpdateCourseAsync(Course course);
    Task DeleteCourseAsync(int id);
}