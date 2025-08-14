namespace employee_management.Server.Models.Common;

/// <summary>
/// Interface that defines audit properties for tracking entity lifecycle events.
/// Implemented by entities that need to track creation, updates, and soft delete operations.
/// This interface provides a consistent audit trail across all auditable entities in the system.
/// </summary>
/// <example>
/// public class Employee : IAuditable
/// {
///     public Guid Id { get; set; }
///     public string Name { get; set; }
///     // ... other business properties
///     
///     // IAuditable implementation
///     public DateTime CreatedAt { get; set; }
///     public string CreatedBy { get; set; }
///     public DateTime? UpdatedAt { get; set; }
///     public string? UpdatedBy { get; set; }
///     public DateTime? DeletedAt { get; set; }
///     public string? DeletedBy { get; set; }
///     public bool IsDeleted { get; set; }
///     public byte[] RowVersion { get; set; }
/// }
/// </example>
public interface IAuditable
{
    /// <summary>
    /// The unique identifier (GUID) of the entity.
    /// </summary>
    /// <example>123e4567-e89b-12d3-a456-426614174000</example>
    Guid Id { get; set; }
    
    /// <summary>
    /// The timestamp when the entity record was created.
    /// This field is automatically set when the entity is first persisted to the database.
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// The identifier of the user or system that created the entity record.
    /// This field tracks who or what process initiated the creation of the entity.
    /// </summary>
    /// <example>system</example>
    string CreatedBy { get; set; }
    
    /// <summary>
    /// The timestamp when the entity record was last updated. Null if never updated.
    /// This field is automatically updated whenever the entity is modified and persisted.
    /// </summary>
    /// <example>2024-01-20T15:45:00Z</example>
    DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// The identifier of the user or system that last updated the entity record. Null if never updated.
    /// This field tracks who or what process made the most recent modification to the entity.
    /// </summary>
    /// <example>admin</example>
    string? UpdatedBy { get; set; }
    
    /// <summary>
    /// The timestamp when the entity record was soft-deleted. Null if not deleted.
    /// This field is set when the entity is marked as deleted but not physically removed from the database.
    /// </summary>
    /// <example>null</example>
    DateTime? DeletedAt { get; set; }
    
    /// <summary>
    /// The identifier of the user or system that soft-deleted the entity record. Null if not deleted.
    /// This field tracks who or what process initiated the soft delete operation.
    /// </summary>
    /// <example>null</example>
    string? DeletedBy { get; set; }
    
    /// <summary>
    /// Indicates whether the entity record has been soft-deleted.
    /// When true, the entity is considered deleted but remains in the database for audit purposes.
    /// </summary>
    /// <example>false</example>
    bool IsDeleted { get; set; }
    
    /// <summary>
    /// Row version for optimistic concurrency control.
    /// This byte array is automatically updated by the database on each modification
    /// and is used to detect concurrent modification conflicts.
    /// </summary>
    /// <example>[1, 0, 0, 0]</example>
    byte[] RowVersion { get; set; } 
} 
