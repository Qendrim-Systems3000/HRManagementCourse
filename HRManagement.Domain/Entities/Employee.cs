using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public class Employee : BaseEntity
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public virtual ICollection<EmployeeCourse> EmployeeCourses { get; set; } = new List<EmployeeCourse>();
}