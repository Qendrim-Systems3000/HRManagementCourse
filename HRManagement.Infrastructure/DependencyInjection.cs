using Microsoft.Extensions.DependencyInjection;
using HRManagement.Application.Interfaces;
using HRManagement.Infrastructure.Repositories;


namespace HRManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ICourseTypeRepository, CourseTypeRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IEmployeeCourseRepository, EmployeeCourseRepository>();

        return services;
    }
}