namespace employee_management.Server.Services;

/// <summary>
/// Service interface for database initialization operations.
/// </summary>
public interface IDatabaseInitializationService
{
    /// <summary>
    /// Applies pending database migrations.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ApplyMigrationsAsync();
}
