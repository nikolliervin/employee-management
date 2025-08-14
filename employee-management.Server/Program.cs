using Serilog;
using employee_management.Server.Extensions;
using employee_management.Server.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog using our extension method
builder.Host.ConfigureSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger services using extension method
builder.Services.AddSwaggerServices();

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
app.UseSwaggerServices();

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
