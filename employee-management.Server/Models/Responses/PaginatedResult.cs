namespace employee_management.Server.Models.Responses;

/// <summary>
/// Generic paginated result wrapper that provides pagination metadata along with the actual data.
/// Includes navigation properties for building pagination controls in the UI.
/// </summary>
/// <typeparam name="T">The type of items being paginated.</typeparam>
/// <example>
/// {
///   "data": [
///     { "id": "123e4567-e89b-12d3-a456-426614174000", "name": "John Doe" },
///     { "id": "789e0123-e89b-12d3-a456-426614174000", "name": "Jane Smith" }
///   ],
///   "totalCount": 50,
///   "pageNumber": 1,
///   "pageSize": 10,
///   "totalPages": 5,
///   "hasPreviousPage": false,
///   "hasNextPage": true,
///   "firstItemOnPage": 1,
///   "lastItemOnPage": 10
/// }
/// </example>
public class PaginatedResult<T>
{
    /// <summary>
    /// The collection of items for the current page.
    /// </summary>
    /// <example>[{ "id": "123e4567-e89b-12d3-a456-426614174000", "name": "John Doe" }]</example>
    public IEnumerable<T> Data { get; set; } = new List<T>();
    
    /// <summary>
    /// The total number of items across all pages.
    /// </summary>
    /// <example>50</example>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// The current page number (1-based indexing).
    /// </summary>
    /// <example>1</example>
    public int PageNumber { get; set; }
    
    /// <summary>
    /// The number of items per page.
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; set; }
    
    /// <summary>
    /// The total number of pages based on total count and page size.
    /// </summary>
    /// <example>5</example>
    public int TotalPages { get; set; }
    
    /// <summary>
    /// Indicates whether there is a previous page available.
    /// </summary>
    /// <example>false</example>
    public bool HasPreviousPage => PageNumber > 1;
    
    /// <summary>
    /// Indicates whether there is a next page available.
    /// </summary>
    /// <example>true</example>
    public bool HasNextPage => PageNumber < TotalPages;
    
    /// <summary>
    /// The index of the first item on the current page (1-based indexing).
    /// </summary>
    /// <example>1</example>
    public int FirstItemOnPage => (PageNumber - 1) * PageSize + 1;
    
    /// <summary>
    /// The index of the last item on the current page (1-based indexing).
    /// </summary>
    /// <example>10</example>
    public int LastItemOnPage => Math.Min(PageNumber * PageSize, TotalCount);

    /// <summary>
    /// Default constructor for creating an empty paginated result.
    /// </summary>
    public PaginatedResult()
    {
    }

    /// <summary>
    /// Creates a new paginated result with the specified data and pagination parameters.
    /// </summary>
    /// <param name="data">The collection of items for the current page.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <param name="pageNumber">The current page number (1-based indexing).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <example>
    /// var result = new PaginatedResult&lt;EmployeeDto&gt;(employees, 50, 1, 10);
    /// </example>
    public PaginatedResult(IEnumerable<T> data, int totalCount, int pageNumber, int pageSize)
    {
        Data = data;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
    }
} 