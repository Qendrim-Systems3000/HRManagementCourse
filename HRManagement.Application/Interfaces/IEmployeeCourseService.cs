using HRManagement.Domain.Entities;

namespace HRManagement.Application.Interfaces;

public interface IEmployeeCourseService
{
    Task EnrollEmployeeAsync(EmployeeCourse enrollment);
    Task<EmployeeCourse?> GetEnrollmentByIdAsync(int id);
    Task<IEnumerable<EmployeeCourse>> GetEmployeeTranscriptAsync(int employeeId);
    Task<IEnumerable<EmployeeCourse>> GetEnrollmentsAsync(int? employeeId, int? courseId);
    Task UpdateEnrollmentAsync(EmployeeCourse enrollment);
    Task RemoveEnrollmentAsync(int id);
}