namespace employee_management.Server.Services;

/// <summary>
/// Service interface for database initialization operations.
/// Handles database setup tasks such as applying migrations and ensuring the database is ready for use.
/// This service is typically called during application startup to ensure database schema is up-to-date.
/// </summary>
/// <example>
/// // Dependency injection usage
/// services.AddScoped&lt;IDatabaseInitializationService, DatabaseInitializationService&gt;();
/// 
/// // In Program.cs startup
/// using (var scope = app.Services.CreateScope())
/// {
///     var databaseInitService = scope.ServiceProvider.GetRequiredService&lt;IDatabaseInitializationService&gt;();
///     await databaseInitService.ApplyMigrationsAsync();
/// }
/// </example>
public interface IDatabaseInitializationService
{
    /// <summary>
    /// Applies pending database migrations to ensure the database schema is up-to-date.
    /// This method should be called during application startup to ensure all database changes are applied.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <example>
    /// // Apply migrations on startup
    /// await _databaseInitService.ApplyMigrationsAsync();
    /// </example>
    Task ApplyMigrationsAsync();
}
