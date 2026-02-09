using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public class CourseType : BaseEntity
{
    public int CourseTypeId { get; set; }
    public string Description { get; set; } = string.Empty;

    // Navigation property
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}