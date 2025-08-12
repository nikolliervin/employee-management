using Microsoft.EntityFrameworkCore;
using Serilog;
using employee_management.Server.Data;
using employee_management.Server.Data.Repositories;
using employee_management.Server.Services;
using employee_management.Server.Mapping;
using Serilog.Events;

namespace employee_management.Server.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext with SQL Server
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }),
            ServiceLifetime.Transient); //This is used because we are using multiple threads on paginated results

        // Add AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // Add Repositories - Transient to match DbContext lifetime
        services.AddTransient<IEmployeeRepository, EmployeeRepository>();
        services.AddTransient<IDepartmentRepository, DepartmentRepository>();

        // Add Services - Transient to match DbContext lifetime
        services.AddTransient<IEmployeeService, EmployeeService>();
        services.AddTransient<IDepartmentService, DepartmentService>();

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