using Microsoft.EntityFrameworkCore;
using Serilog;
using employee_management.Server.Data;
using employee_management.Server.Data.Repositories;
using employee_management.Server.Services;
using employee_management.Server.Mapping;
using Serilog.Events;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace employee_management.Server.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Add AutoMapper
        services.AddAutoMapper(typeof(Program));

        // Add Repositories
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        // Add Services
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IDatabaseInitializationService, DatabaseInitializationService>();
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

    public static void AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Configure Swagger to show versioned endpoints
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Employee Management API",
                Version = "v1",
                Description = "A comprehensive API for managing employees and departments with features including CRUD operations, search, pagination, and soft delete/restore functionality.",
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Include XML comments for better documentation
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Add security definitions if needed in the future
            // options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { ... });

            // Customize operation IDs for better client generation
            options.CustomOperationIds(apiDesc =>
            {
                return apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null;
            });
        });
    }
}

public static class ApplicationExtensions
{
    public static void UseSwaggerServices(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API v1");
                options.DocumentTitle = "Employee Management API Documentation";
                options.RoutePrefix = "swagger";
            });
        }
    }
} 