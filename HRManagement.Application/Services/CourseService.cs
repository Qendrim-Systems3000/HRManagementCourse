using HRManagement.Application.Interfaces;
using HRManagement.Domain.Entities;

namespace HRManagement.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseTypeService _courseTypeService;

    public CourseService(ICourseRepository courseRepository, ICourseTypeService courseTypeService)
    {
        _courseRepository = courseRepository;
        _courseTypeService = courseTypeService;
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
        var courseType = await _courseTypeService.GetByIdAsync(course.CourseTypeId);
        if (courseType == null)
            throw new KeyNotFoundException("Course type does not exist.");

        if (await _courseRepository.CourseExistsAsync(course.Description, course.StartDate))
            throw new InvalidOperationException("A course with this description and start date already exists.");

        await _courseRepository.AddAsync(course);
        await _courseRepository.SaveChangesAsync();
    }

    public async Task UpdateCourseAsync(Course course)
    {
        var existing = await _courseRepository.GetByIdAsync(course.CourseId);
        if (existing == null) throw new KeyNotFoundException("Course not found in your district.");

        var courseType = await _courseTypeService.GetByIdAsync(course.CourseTypeId);
        if (courseType == null)
            throw new KeyNotFoundException("Course type does not exist.");

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