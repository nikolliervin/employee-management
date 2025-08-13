using Microsoft.EntityFrameworkCore;
using Serilog;
using employee_management.Server.Data;

namespace employee_management.Server.Services;

/// <summary>
/// Service for handling database initialization operations.
/// </summary>
public class DatabaseInitializationService : IDatabaseInitializationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseInitializationService> _logger;

    public DatabaseInitializationService(
        ApplicationDbContext context,
        ILogger<DatabaseInitializationService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Applies pending database migrations asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ApplyMigrationsAsync()
    {
        try
        {
            Log.Information("Applying database migrations...");
            await _context.Database.MigrateAsync();
            Log.Information("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while applying database migrations");
            // Don't rethrow - let the application continue in case DB is already up to date
        }
    }
}
