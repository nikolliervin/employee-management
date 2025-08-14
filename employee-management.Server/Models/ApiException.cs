namespace employee_management.Server.Models;

/// <summary>
/// Custom exception model for API error responses.
/// Used to provide structured error information including HTTP status code, message, and optional details.
/// This model is typically used in exception middleware to format error responses consistently.
/// </summary>
/// <example>
/// {
///   "statusCode": 400,
///   "message": "Validation failed",
///   "details": "The email address format is invalid and the name field is required."
/// }
/// </example>
public class ApiException
{
    /// <summary>
    /// Initializes a new instance of the ApiException class with the specified status code, message, and optional details.
    /// </summary>
    /// <param name="statusCode">The HTTP status code that should be returned to the client.</param>
    /// <param name="message">A human-readable error message describing what went wrong.</param>
    /// <param name="details">Optional detailed information about the error, useful for debugging.</param>
    /// <example>
    /// var exception = new ApiException(400, "Validation failed", "Email format is invalid");
    /// </example>
    public ApiException(int statusCode, string message, string? details)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }

    /// <summary>
    /// The HTTP status code that should be returned to the client.
    /// Common values include 400 (Bad Request), 401 (Unauthorized), 404 (Not Found), 500 (Internal Server Error).
    /// </summary>
    /// <example>400</example>
    public int StatusCode { get; set; }
    
    /// <summary>
    /// A human-readable error message that describes what went wrong.
    /// This message should be user-friendly and provide clear guidance on how to resolve the issue.
    /// </summary>
    /// <example>Validation failed</example>
    public string Message { get; set; }
    
    /// <summary>
    /// Optional detailed information about the error, typically used for debugging purposes.
    /// This field may contain technical details, validation errors, or stack trace information.
    /// Can be null if no additional details are available.
    /// </summary>
    /// <example>The email address format is invalid and the name field is required.</example>
    public string? Details { get; set; }
} 
