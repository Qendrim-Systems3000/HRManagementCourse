using HRManagement.Application.Interfaces;
using HRManagement.Domain.Entities;

namespace HRManagement.Application.Services;

public class EmployeeCourseService : IEmployeeCourseService
{
    private readonly IEmployeeCourseRepository _repo;
    private readonly IGenericRepository<Employee> _employeeRepo;
    private readonly ICourseRepository _courseRepo;

    public EmployeeCourseService(
        IEmployeeCourseRepository repo, 
        IGenericRepository<Employee> employeeRepo,
        ICourseRepository courseRepo)
    {
        _repo = repo;
        _employeeRepo = employeeRepo;
        _courseRepo = courseRepo;
    }

    public async Task EnrollEmployeeAsync(EmployeeCourse enrollment)
    {
        // 1. Verify Employee exists in this district
        var emp = await _employeeRepo.GetByIdAsync(enrollment.EmployeeId);
        if (emp == null) throw new KeyNotFoundException("Employee not found.");

        // 2. Verify Course exists in this district
        var course = await _courseRepo.GetByIdAsync(enrollment.CourseId);
        if (course == null) throw new KeyNotFoundException("Course not found.");

        await _repo.AddAsync(enrollment);
        await _repo.SaveChangesAsync();
    }

    public async Task<EmployeeCourse?> GetEnrollmentByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<IEnumerable<EmployeeCourse>> GetEmployeeTranscriptAsync(int employeeId)
    {
        return await _repo.GetByEmployeeIdAsync(employeeId);
    }

    public async Task<IEnumerable<EmployeeCourse>> GetEnrollmentsAsync(int? employeeId, int? courseId)
    {
        return await _repo.GetFilteredAsync(employeeId, courseId);
    }

    public async Task UpdateEnrollmentAsync(EmployeeCourse enrollment)
    {
        _repo.Update(enrollment);
        await _repo.SaveChangesAsync();
    }

    public async Task RemoveEnrollmentAsync(int id)
    {
        var enrollment = await _repo.GetByIdAsync(id);
        if (enrollment != null)
        {
            _repo.Delete(enrollment);
            await _repo.SaveChangesAsync();
        }
    }
}