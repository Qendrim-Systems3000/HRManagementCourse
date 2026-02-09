using HRManagement.Application.Interfaces;
using HRManagement.Domain.Entities;

namespace HRManagement.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<IEnumerable<Course>> GetCoursesAsync(int? typeId, DateTime? date, bool? approved)
    {
        // Logic for filtering is handled at the Repository level for database performance
        return await _courseRepository.GetFilteredCoursesAsync(typeId, date, approved);
    }

    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        return await _courseRepository.GetByIdAsync(id);
    }

    public async Task CreateCourseAsync(Course course)
    {
        // Rule: Don't allow duplicate Course
        // We check if a course with the same description and start date exists
        if (await _courseRepository.CourseExistsAsync(course.Description, course.StartDate))
        {
            throw new InvalidOperationException("A course with this description and start date already exists.");
        }

        await _courseRepository.AddAsync(course);
        await _courseRepository.SaveChangesAsync();
    }

    public async Task UpdateCourseAsync(Course course)
    {
        // Note: Global filters ensure the user can only update a course in their district
        var existing = await _courseRepository.GetByIdAsync(course.CourseId);
        if (existing == null) throw new KeyNotFoundException("Course not found in your district.");

        _courseRepository.Update(course);
        await _courseRepository.SaveChangesAsync();
    }

    public async Task DeleteCourseAsync(int id)
    {
        // Rule: Don't allow delete if is used in Employee Course
        if (await _courseRepository.HasEnrolledEmployeesAsync(id))
        {
            throw new InvalidOperationException("Cannot delete course: Employees are currently enrolled in this course.");
        }

        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null) throw new KeyNotFoundException("Course not found.");

        _courseRepository.Delete(course);
        await _courseRepository.SaveChangesAsync();
    }
}