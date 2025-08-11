using Microsoft.EntityFrameworkCore;
using Serilog;
using employee_management.Server.Data;
using employee_management.Server.Data.Repositories;
using employee_management.Server.Services;
using employee_management.Server.Mapping;

namespace employee_management.Server.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Entity Framework Core with In-Memory Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("EmployeeManagementDb"));

        // Add AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // Add Repositories
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        // Add Services
        services.AddScoped<IEmployeeService, EmployeeService>();

        return services;
    }

    public static IHostBuilder ConfigureSerilog(this IHostBuilder builder)
    {
        return builder.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/employee-management-.txt", 
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7);
        });
    }
} 