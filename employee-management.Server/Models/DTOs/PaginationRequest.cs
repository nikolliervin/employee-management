using System.ComponentModel.DataAnnotations;

namespace employee_management.Server.Models.DTOs;

/// <summary>
/// Request model for pagination parameters used in list operations.
/// </summary>
/// <example>
/// {
///   "pageNumber": 1,
///   "pageSize": 10,
///   "sortBy": "Name",
///   "sortOrder": "asc"
/// }
/// </example>
public class PaginationRequest
{
    /// <summary>
    /// The page number to retrieve. Must be greater than 0.
    /// </summary>
    /// <example>1</example>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// The number of items per page. Must be between 1 and 100.
    /// </summary>
    /// <example>10</example>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// The field to sort by. Defaults to "Name" if not specified.
    /// </summary>
    /// <example>Name</example>
    public string? SortBy { get; set; } = "Name";
    
    /// <summary>
    /// The sort order. Must be either "asc" (ascending) or "desc" (descending).
    /// </summary>
    /// <example>asc</example>
    [RegularExpression("^(asc|desc)$", ErrorMessage = "Sort order must be 'asc' or 'desc'")]
    public string SortOrder { get; set; } = "asc";
} 