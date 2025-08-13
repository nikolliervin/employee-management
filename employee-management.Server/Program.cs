using Serilog;
using Microsoft.EntityFrameworkCore;
using employee_management.Server.Extensions;
using employee_management.Server.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog using our extension method
builder.Host.ConfigureSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Add API Versioning using .NET 8 built-in approach
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configure Swagger to show versioned endpoints
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Employee Management API",
        Version = "v1",
        Description = "Employee Management API Version 1.0"
    });
});

// Add our application services
builder.Services.AddApplicationServices(builder.Configuration);

// Add CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:5173", 
                    "https://localhost:54300",
                    "http://localhost:8080",  // Docker frontend
                    "http://employee-client:3000"  // Container-to-container
                  )
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API v1");
    });
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowReactApp");

// Add exception middleware (must be before authorization and routing)
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

// Apply database migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var databaseInitService = scope.ServiceProvider.GetRequiredService<employee_management.Server.Services.IDatabaseInitializationService>();
    await databaseInitService.ApplyMigrationsAsync();
}

app.Run();
