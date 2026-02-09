using HRManagement.Domain.Entities;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Seeding;

/// <summary>
/// Seeds initial CourseTypes, Courses, Employees, and EmployeeCourses for a default tenant (e.g. development).
/// Only runs when no course types exist for that tenant.
/// </summary>
public static class DataSeeder
{
    /// <summary>Tenant/department id used for seeded data (e.g. district 1).</summary>
    private const int SeedTenantId = 1;

    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken cancellationToken = default)
    {
        // Skip if this tenant already has data (ignore global filter since we're not in a request)
        if (await db.CourseTypes.IgnoreQueryFilters().AnyAsync(ct => ct.LeaProfileId == SeedTenantId, cancellationToken))
            return;

        // 1. Course types
        var safetyType = new CourseType { LeaProfileId = SeedTenantId, Description = "Safety Training" };
        var complianceType = new CourseType { LeaProfileId = SeedTenantId, Description = "Compliance" };
        var professionalType = new CourseType { LeaProfileId = SeedTenantId, Description = "Professional Development" };
        db.CourseTypes.AddRange(safetyType, complianceType, professionalType);
        await db.SaveChangesAsync(cancellationToken);

        // 2. Courses (reference course types by their new IDs)
        var courses = new List<Course>
        {
            new Course
            {
                LeaProfileId = SeedTenantId,
                Description = "Fire Safety Basics",
                CourseTypeId = safetyType.CourseTypeId,
                StartDate = new DateTime(2025, 3, 1),
                EndDate = new DateTime(2025, 3, 1),
                Hours = 4,
                Credits = 0,
                DistrictCost = 50m,
                EmployeeCost = 0m,
                TuitionEligible = false,
                Approved = true,
                MaintenanceOfLicense = false,
                Provider = "Internal",
                Location = "Main Office"
            },
            new Course
            {
                LeaProfileId = SeedTenantId,
                Description = "Workplace Compliance 2025",
                CourseTypeId = complianceType.CourseTypeId,
                StartDate = new DateTime(2025, 4, 10),
                EndDate = new DateTime(2025, 4, 10),
                Hours = 2,
                Credits = 0,
                DistrictCost = 0m,
                EmployeeCost = 0m,
                Approved = true,
                Provider = "HR Department"
            },
            new Course
            {
                LeaProfileId = SeedTenantId,
                Description = "Leadership Workshop",
                CourseTypeId = professionalType.CourseTypeId,
                StartDate = new DateTime(2025, 5, 15),
                EndDate = new DateTime(2025, 5, 16),
                Hours = 16,
                Credits = 1,
                DistrictCost = 200m,
                EmployeeCost = 0m,
                Approved = true,
                Institution = "Training Center"
            }
        };
        db.Courses.AddRange(courses);
        await db.SaveChangesAsync(cancellationToken);

        // 3. Employees
        var employees = new List<Employee>
        {
            new Employee { LeaProfileId = SeedTenantId, FirstName = "Jane", LastName = "Doe", Email = "jane.doe@district1.org" },
            new Employee { LeaProfileId = SeedTenantId, FirstName = "John", LastName = "Smith", Email = "john.smith@district1.org" },
            new Employee { LeaProfileId = SeedTenantId, FirstName = "Maria", LastName = "Garcia", Email = "maria.garcia@district1.org" }
        };
        db.Employees.AddRange(employees);
        await db.SaveChangesAsync(cancellationToken);

        // 4. Employee courses (enrollments) – after courses and employees have IDs
        var enrollments = new List<EmployeeCourse>
        {
            new EmployeeCourse
            {
                LeaProfileId = SeedTenantId,
                EmployeeId = employees[0].EmployeeId,
                CourseId = courses[0].CourseId,
                StartDate = new DateTime(2025, 3, 1),
                EndDate = new DateTime(2025, 3, 1),
                Hours = 4,
                Credits = 0,
                DistrictCost = 50m,
                EmployeeCost = 0m,
                Grade = "Pass"
            },
            new EmployeeCourse
            {
                LeaProfileId = SeedTenantId,
                EmployeeId = employees[0].EmployeeId,
                CourseId = courses[1].CourseId,
                StartDate = new DateTime(2025, 4, 10),
                EndDate = new DateTime(2025, 4, 10),
                Hours = 2,
                Credits = 0,
                DistrictCost = 0m,
                EmployeeCost = 0m
            },
            new EmployeeCourse
            {
                LeaProfileId = SeedTenantId,
                EmployeeId = employees[1].EmployeeId,
                CourseId = courses[0].CourseId,
                StartDate = new DateTime(2025, 3, 1),
                EndDate = new DateTime(2025, 3, 1),
                Hours = 4,
                Credits = 0,
                DistrictCost = 50m,
                EmployeeCost = 0m
            }
        };
        db.EmployeeCourses.AddRange(enrollments);
        await db.SaveChangesAsync(cancellationToken);
    }
}
