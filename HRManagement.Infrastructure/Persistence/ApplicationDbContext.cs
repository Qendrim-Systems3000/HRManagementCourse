using HRManagement.Application.Interfaces;
using HRManagement.Domain.Common;
using HRManagement.Domain.Entities;
using HRManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ITenantService _tenantService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService) 
        : base(options)
    {
        _tenantService = tenantService;
    }

    public DbSet<CourseType> CourseTypes => Set<CourseType>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeCourse> EmployeeCourses => Set<EmployeeCourse>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 1. CourseType Constraints (unique Description per tenant)
        builder.Entity<CourseType>(entity =>
        {
            entity.HasIndex(e => new { e.LeaProfileId, e.Description }).IsUnique();
            entity.HasMany(e => e.Courses)
                  .WithOne(c => c.CourseType)
                  .HasForeignKey(c => c.CourseTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 2. Course Constraints (unique Description+StartDate per tenant)
        builder.Entity<Course>(entity =>
        {
            entity.HasIndex(e => new { e.LeaProfileId, e.Description, e.StartDate }).IsUnique();
            entity.HasMany(e => e.EmployeeCourses)
                  .WithOne(ec => ec.Course)
                  .HasForeignKey(ec => ec.CourseId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 3. Multi-Tenancy: dynamic filters; use OrNull so startup (e.g. role seeding) without HTTP context does not throw
        builder.Entity<CourseType>().HasQueryFilter(e => e.LeaProfileId == (_tenantService.GetCurrentTenantIdOrNull() ?? -1));
        builder.Entity<Course>().HasQueryFilter(e => e.LeaProfileId == (_tenantService.GetCurrentTenantIdOrNull() ?? -1));
        builder.Entity<Employee>().HasQueryFilter(e => e.LeaProfileId == (_tenantService.GetCurrentTenantIdOrNull() ?? -1));
        builder.Entity<EmployeeCourse>().HasQueryFilter(e => e.LeaProfileId == (_tenantService.GetCurrentTenantIdOrNull() ?? -1));

        // 4. Refresh tokens (no tenant filter)
        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var districtId = _tenantService.GetCurrentTenantIdOrNull();
        if (!districtId.HasValue) return base.SaveChangesAsync(cancellationToken);

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.LeaProfileId = districtId.Value;
                    break;
                case EntityState.Modified:
                    entry.Property(x => x.LeaProfileId).IsModified = false;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}