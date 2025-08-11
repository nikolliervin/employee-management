namespace employee_management.Server.Models.Common;

public interface IAuditable
{
    Guid Id { get; set; }
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
    bool IsDeleted { get; set; }
    byte[] RowVersion { get; set; } // For optimistic concurrency
} 