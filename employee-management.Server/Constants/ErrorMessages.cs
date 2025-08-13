namespace employee_management.Server.Constants;

/// <summary>
/// Centralized error messages for consistent messaging across the application.
/// Provides maintainable and user-friendly error message strings without technical details.
/// </summary>
public static class ErrorMessages
{
    #region Employee Messages
    public static class Employee
    {
        public const string NotFound = "Employee with ID {0} not found";
        public const string NotFoundOrNotDeleted = "Employee with ID {0} not found or not deleted";
        public const string EmailAlreadyExists = "Employee with email '{0}' already exists";
        public const string CreatedSuccessfully = "Employee created successfully";
        public const string UpdatedSuccessfully = "Employee updated successfully";
        public const string DeletedSuccessfully = "Employee soft deleted successfully";
        public const string RestoredSuccessfully = "Employee restored successfully";
        public const string SearchCriteriaRequired = "At least one search criteria must be provided";
        
        // User-friendly error messages (no technical details)
        public const string RetrievalFailed = "Unable to retrieve employees at this time";
        public const string CreationFailed = "Unable to create employee at this time";
        public const string UpdateFailed = "Unable to update employee at this time";
        public const string DeletionFailed = "Unable to delete employee at this time";
        public const string RestoreFailed = "Unable to restore employee at this time";
        public const string SearchFailed = "Unable to search employees at this time";
    }
    #endregion

    #region Department Messages
    public static class Department
    {
        public const string NotFound = "Department with ID {0} not found";
        public const string NotFoundGeneric = "Department not found";
        public const string NotFoundOrNotDeleted = "Department with ID {0} not found or not deleted";
        public const string NotFoundOrNotDeletedGeneric = "Department not found or not deleted";
        public const string NameAlreadyExists = "Department name already exists";
        public const string CannotDeleteWithEmployees = "Cannot delete department with existing employees";
        public const string CreatedSuccessfully = "Department created successfully";
        public const string UpdatedSuccessfully = "Department updated successfully";
        public const string DeletedSuccessfully = "Department soft deleted successfully";
        public const string RestoredSuccessfully = "Department restored successfully";
        public const string SearchCriteriaRequired = "At least one search criteria must be provided";
        
        // User-friendly error messages (no technical details)
        public const string RetrievalFailed = "Unable to retrieve departments at this time";
        public const string CreationFailed = "Unable to create department at this time";
        public const string UpdateFailed = "Unable to update department at this time";
        public const string DeletionFailed = "Unable to delete department at this time";
        public const string RestoreFailed = "Unable to restore department at this time";
        public const string SearchFailed = "Unable to search departments at this time";
    }
    #endregion

    #region General Messages
    public static class General
    {
        public const string InternalServerError = "An unexpected error occurred. Please try again later";
        public const string ValidationFailed = "Validation failed";
        public const string UnauthorizedAccess = "Unauthorized access";
        public const string ResourceNotFound = "The requested resource was not found";
    }
    #endregion
}
