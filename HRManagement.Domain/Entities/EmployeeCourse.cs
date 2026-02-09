using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public class EmployeeCourse : BaseEntity
{
    public int EmployeeCourseId { get; set; }
    
    public int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;

    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Hours { get; set; }
    public int Credits { get; set; }
    public decimal DistrictCost { get; set; }
    public decimal EmployeeCost { get; set; }
    public string? Grade { get; set; }
    public string? Major { get; set; }
    public string? Notes { get; set; }
}