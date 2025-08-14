namespace employee_management.Server.Models.Responses;

/// <summary>
/// Generic API response wrapper that provides consistent structure for all API responses.
/// Includes success status, message, data, errors, and HTTP status code.
/// </summary>
/// <typeparam name="T">The type of data being returned in the response.</typeparam>
/// <example>
/// Success Response:
/// {
///   "isSuccess": true,
///   "message": "Employee retrieved successfully",
///   "data": { ... },
///   "errors": [],
///   "statusCode": 200
/// }
/// 
/// Error Response:
/// {
///   "isSuccess": false,
///   "message": "Validation failed",
///   "data": null,
///   "errors": ["Name is required", "Email format is invalid"],
///   "statusCode": 400
/// }
/// </example>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    /// <example>true</example>
    public bool IsSuccess { get; set; }
    
    /// <summary>
    /// Human-readable message describing the result of the operation.
    /// </summary>
    /// <example>Employee retrieved successfully</example>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// The actual data payload of the response. Null for error responses.
    /// </summary>
    /// <example>{ "id": "123e4567-e89b-12d3-a456-426614174000", "name": "John Doe" }</example>
    public T? Data { get; set; }
    
    /// <summary>
    /// List of validation or error messages. Empty for successful responses.
    /// </summary>
    /// <example>["Name is required", "Email format is invalid"]</example>
    public List<string> Errors { get; set; } = new();
    
    /// <summary>
    /// HTTP status code corresponding to the response.
    /// </summary>
    /// <example>200</example>
    public int StatusCode { get; set; }

    /// <summary>
    /// Creates a successful response with optional message and status code.
    /// </summary>
    /// <param name="data">The data to include in the response.</param>
    /// <param name="message">Optional success message. Defaults to "Operation completed successfully".</param>
    /// <param name="statusCode">HTTP status code. Defaults to 200.</param>
    /// <returns>A new ApiResponse instance with success status.</returns>
    /// <example>
    /// var response = ApiResponse&lt;EmployeeDto&gt;.Success(employee, "Employee created successfully", 201);
    /// </example>
    public static ApiResponse<T> Success(T data, string? message = null, int statusCode = 200)
    {
        var response = new ApiResponse<T>();
        response.IsSuccess = true;
        response.Data = data;
        response.Message = message ?? "Operation completed successfully";
        response.StatusCode = statusCode;
        return response;
    }

    /// <summary>
    /// Creates a response for successful resource creation (POST operations).
    /// </summary>
    /// <param name="data">The created resource data.</param>
    /// <param name="message">Optional success message. Defaults to "Resource created successfully".</param>
    /// <returns>A new ApiResponse instance with 201 status code.</returns>
    /// <example>
    /// var response = ApiResponse&lt;EmployeeDto&gt;.Created(newEmployee, "Employee created successfully");
    /// </example>
    public static ApiResponse<T> Created(T data, string? message = null)
    {
        var response = new ApiResponse<T>();
        response.IsSuccess = true;
        response.Data = data;
        response.Message = message ?? "Resource created successfully";
        response.StatusCode = 201;
        return response;
    }

    /// <summary>
    /// Creates an error response with custom message and status code.
    /// </summary>
    /// <param name="message">Error message describing what went wrong.</param>
    /// <param name="statusCode">HTTP status code. Defaults to 500.</param>
    /// <param name="errors">Optional list of detailed error messages.</param>
    /// <returns>A new ApiResponse instance with error status.</returns>
    /// <example>
    /// var response = ApiResponse&lt;EmployeeDto&gt;.Error("Database connection failed", 500, ["Connection timeout", "Server unavailable"]);
    /// </example>
    public static ApiResponse<T> Error(string message, int statusCode = 500, List<string>? errors = null)
    {
        var response = new ApiResponse<T>();
        response.IsSuccess = false;
        response.Message = message;
        response.StatusCode = statusCode;
        response.Errors = errors ?? new List<string>();
        return response;
    }

    /// <summary>
    /// Creates a bad request response (400 status code).
    /// </summary>
    /// <param name="message">Error message describing the bad request.</param>
    /// <param name="errors">Optional list of validation errors.</param>
    /// <returns>A new ApiResponse instance with 400 status code.</returns>
    /// <example>
    /// var response = ApiResponse&lt;EmployeeDto&gt;.BadRequest("Invalid input data", ["Name is required"]);
    /// </example>
    public static ApiResponse<T> BadRequest(string message, List<string>? errors = null)
    {
        var response = new ApiResponse<T>();
        response.IsSuccess = false;
        response.Message = message;
        response.StatusCode = 400;
        response.Errors = errors ?? new List<string>();
        return response;
    }

    /// <summary>
    /// Creates a validation error response (400 status code).
    /// </summary>
    /// <param name="errors">List of validation error messages.</param>
    /// <returns>A new ApiResponse instance with 400 status code and validation errors.</returns>
    /// <example>
    /// var response = ApiResponse&lt;EmployeeDto&gt;.ValidationError(["Name is required", "Email format is invalid"]);
    /// </example>
    public static ApiResponse<T> ValidationError(List<string> errors)
    {
        var response = new ApiResponse<T>();
        response.IsSuccess = false;
        response.Message = "Validation failed";
        response.StatusCode = 400;
        response.Errors = errors;
        return response;
    }

    /// <summary>
    /// Creates a not found response (404 status code).
    /// </summary>
    /// <param name="message">Error message. Defaults to "Resource not found".</param>
    /// <returns>A new ApiResponse instance with 404 status code.</returns>
    /// <example>
    /// var response = ApiResponse&lt;EmployeeDto&gt;.NotFound("Employee with ID 123 not found");
    /// </example>
    public static ApiResponse<T> NotFound(string message = "Resource not found")
    {
        var response = new ApiResponse<T>();
        response.IsSuccess = false;
        response.Message = message;
        response.StatusCode = 404;
        return response;
    }

    /// <summary>
    /// Creates a conflict response (409 status code) for duplicate resources or business rule violations.
    /// </summary>
    /// <param name="message">Error message describing the conflict.</param>
    /// <returns>A new ApiResponse instance with 409 status code.</returns>
    /// <example>
    /// var response = ApiResponse&lt;EmployeeDto&gt;.Conflict("Employee with email john.doe@example.com already exists");
    /// </example>
    public static ApiResponse<T> Conflict(string message)
    {
        var response = new ApiResponse<T>();
        response.IsSuccess = false;
        response.Message = message;
        response.StatusCode = 409;
        return response;
    }
} 