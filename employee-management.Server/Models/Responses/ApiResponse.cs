namespace employee_management.Server.Models.Responses;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public int StatusCode { get; set; }

    // Success response with optional message and status code
    public static ApiResponse<T> Success(T data, string? message = null, int statusCode = 200)
    {
        var response = new ApiResponse<T>();
        response.IsSuccess = true;
        response.Data = data;
        response.Message = message ?? "Operation completed successfully";
        response.StatusCode = statusCode;
        return response;
    }

    // Created response for POST operations
    public static ApiResponse<T> Created(T data, string? message = null)
    {
        var response = new ApiResponse<T>();
        response.IsSuccess = true;
        response.Data = data;
        response.Message = message ?? "Resource created successfully";
        response.StatusCode = 201;
        return response;
    }

    // Error response
    public static ApiResponse<T> Error(string message, int statusCode = 500, List<string>? errors = null)
    {
        var response = new ApiResponse<T>();
        response.IsSuccess = false;
        response.Message = message;
        response.StatusCode = statusCode;
        response.Errors = errors ?? new List<string>();
        return response;
    }

    // Bad request response
    public static ApiResponse<T> BadRequest(string message, List<string>? errors = null)
    {
        var response = new ApiResponse<T>();
        response.IsSuccess = false;
        response.Message = message;
        response.StatusCode = 400;
        response.Errors = errors ?? new List<string>();
        return response;
    }

    // Validation error response
    public static ApiResponse<T> ValidationError(List<string> errors)
    {
        var response = new ApiResponse<T>();
        response.IsSuccess = false;
        response.Message = "Validation failed";
        response.StatusCode = 400;
        response.Errors = errors;
        return response;
    }

    // Not found response
    public static ApiResponse<T> NotFound(string message = "Resource not found")
    {
        var response = new ApiResponse<T>();
        response.IsSuccess = false;
        response.Message = message;
        response.StatusCode = 404;
        return response;
    }

    // Conflict response (e.g., duplicate email)
    public static ApiResponse<T> Conflict(string message)
    {
        var response = new ApiResponse<T>();
        response.IsSuccess = false;
        response.Message = message;
        response.StatusCode = 409;
        return response;
    }
} 