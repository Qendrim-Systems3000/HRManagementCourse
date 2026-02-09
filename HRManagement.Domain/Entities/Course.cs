using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public class Course : BaseEntity
{
    public int CourseId { get; set; }
    public string Description { get; set; } = string.Empty;
    
    // Relationships
    public int CourseTypeId { get; set; }
    public virtual CourseType CourseType { get; set; } = null!;

    // Details
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Hours { get; set; }
    public int Credits { get; set; }
    public decimal DistrictCost { get; set; }
    public decimal EmployeeCost { get; set; }
    
    // Status flags
    public bool TuitionEligible { get; set; }
    public bool Approved { get; set; }
    public bool MaintenanceOfLicense { get; set; }

    // External Info
    public string? Provider { get; set; }
    public string? Presenter { get; set; }
    public string? Institution { get; set; }
    public string? Degree { get; set; }
    public string? CertNo { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }

    // Navigation for many-to-many link
    public virtual ICollection<EmployeeCourse> EmployeeCourses { get; set; } = new List<EmployeeCourse>();
}