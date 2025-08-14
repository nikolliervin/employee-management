/**
 * Type definitions and constants for the Employee Management system
 */

/**
 * Employee entity structure
 * @typedef {Object} Employee
 * @property {string} id - Unique identifier
 * @property {string} name - Employee full name
 * @property {string} email - Employee email address
 * @property {string} dateOfBirth - Date of birth in ISO format
 * @property {string} departmentId - Department identifier
 * @property {string} departmentName - Department name
 * @property {string} createdAt - Creation timestamp
 * @property {string} createdBy - Creator identifier
 * @property {string|null} updatedAt - Last update timestamp
 * @property {string|null} updatedBy - Last updater identifier
 */

/**
 * API Response wrapper structure
 * @typedef {Object} ApiResponse
 * @property {boolean} isSuccess - Success status
 * @property {string|null} message - Response message
 * @property {any} data - Response data
 * @property {string[]|null} errors - Error messages
 * @property {number} statusCode - HTTP status code
 */

/**
 * Paginated result structure
 * @typedef {Object} PaginatedResult
 * @property {Employee[]} data - Array of employees
 * @property {number} totalCount - Total number of records
 * @property {number} pageNumber - Current page number
 * @property {number} pageSize - Page size
 * @property {number} totalPages - Total number of pages
 * @property {boolean} hasPreviousPage - Has previous page
 * @property {boolean} hasNextPage - Has next page
 */

/**
 * Search request parameters
 * @typedef {Object} SearchRequest
 * @property {string} searchTerm - Search term
 * @property {string} departmentId - Department filter
 * @property {number} pageNumber - Page number
 * @property {number} pageSize - Page size
 */

/**
 * Pagination request parameters
 * @typedef {Object} PaginationRequest
 * @property {number} pageNumber - Page number
 * @property {number} pageSize - Page size
 */

/**
 * Create employee request
 * @typedef {Object} CreateEmployeeRequest
 * @property {string} name - Employee name
 * @property {string} email - Employee email
 * @property {string} dateOfBirth - Date of birth
 * @property {string} departmentId - Department ID
 */

/**
 * Update employee request
 * @typedef {Object} UpdateEmployeeRequest
 * @property {string} name - Employee name
 * @property {string} email - Employee email
 * @property {string} dateOfBirth - Date of birth
 * @property {string} departmentId - Department ID
 */

// API Configuration
export const API_CONFIG = {
  BASE_URL: import.meta.env.VITE_API_URL || (import.meta.env.DEV ? 'https://localhost:7063' : window.location.origin),
  ENDPOINTS: {
    EMPLOYEES: '/api/v1/Employees',
    EMPLOYEE_BY_ID: (id) => `/api/v1/Employees/${id}`,
    EMPLOYEE_SEARCH: '/api/v1/Employees/search',
    DELETED_EMPLOYEES: '/api/v1/Employees/deleted',
    RESTORE_EMPLOYEE: (id) => `/api/v1/Employees/${id}/restore`,
    DEPARTMENTS: '/api/v1/Departments',
    DEPARTMENT_BY_ID: (id) => `/api/v1/Departments/${id}`,
    DEPARTMENT_SEARCH: '/api/v1/Departments/search',
    DELETED_DEPARTMENTS: '/api/v1/Departments/deleted',
    RESTORE_DEPARTMENT: (id) => `/api/v1/Departments/${id}/restore`,
  },
  DEFAULT_PAGE_SIZE: 10,
  PAGE_SIZE_OPTIONS: [5, 10, 20, 50],
};

// Table column definitions for react-data-table-component
export const EMPLOYEE_COLUMNS = [
  {
    name: 'Name',
    selector: row => row.name,
    sortable: true,
    grow: 2,
  },
  {
    name: 'Email',
    selector: row => row.email,
    sortable: true,
    grow: 2,
  },
  {
    name: 'Department',
    selector: row => row.departmentName,
    sortable: true,
    grow: 1,
  },
  {
    name: 'Date of Birth',
    selector: row => new Date(row.dateOfBirth).toLocaleDateString(),
    sortable: true,
    grow: 1,
  },
  {
    name: 'Created',
    selector: row => new Date(row.createdAt).toLocaleDateString(),
    sortable: true,
    grow: 1,
  },
  {
    name: 'Actions',
    selector: row => row.id,
    sortable: false,
    grow: 1,
    cell: (row) => null, // Will be rendered by the component
  },
];

// Form validation schemas
export const EMPLOYEE_VALIDATION_SCHEMA = {
  name: {
    required: 'Name is required',
    minLength: { value: 2, message: 'Name must be at least 2 characters' },
    maxLength: { value: 100, message: 'Name must be less than 100 characters' },
  },
  email: {
    required: 'Email is required',
    pattern: {
      value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
      message: 'Invalid email address',
    },
  },
  dateOfBirth: {
    required: 'Date of birth is required',
    validate: (value) => {
      const date = new Date(value);
      const today = new Date();
      const age = today.getFullYear() - date.getFullYear();
      if (age < 18 || age > 100) {
        return 'Employee must be between 18 and 100 years old';
      }
      return true;
    },
  },
  departmentId: {
    required: 'Department is required',
  },
};

// Toast notification types
export const TOAST_TYPES = {
  SUCCESS: 'success',
  ERROR: 'error',
  WARNING: 'warning',
  INFO: 'info',
};

// Loading states
export const LOADING_STATES = {
  IDLE: 'idle',
  LOADING: 'loading',
  SUCCESS: 'success',
  ERROR: 'error',
};

// Action types for the context reducers
export const ACTION_TYPES = {
  SET_LOADING: 'SET_LOADING',
  SET_ERROR: 'SET_ERROR',
  CLEAR_ERROR: 'CLEAR_ERROR',
  SET_EMPLOYEES: 'SET_EMPLOYEES',
  SET_DELETED_EMPLOYEES: 'SET_DELETED_EMPLOYEES',
  ADD_EMPLOYEE: 'ADD_EMPLOYEE',
  UPDATE_EMPLOYEE: 'UPDATE_EMPLOYEE',
  REMOVE_EMPLOYEE: 'REMOVE_EMPLOYEE',
  RESTORE_EMPLOYEE: 'RESTORE_EMPLOYEE',
  SET_DEPARTMENTS: 'SET_DEPARTMENTS',
  SET_DELETED_DEPARTMENTS: 'SET_DELETED_DEPARTMENTS',
  ADD_DEPARTMENT: 'ADD_DEPARTMENT',
  UPDATE_DEPARTMENT: 'UPDATE_DEPARTMENT',
  REMOVE_DEPARTMENT: 'REMOVE_DEPARTMENT',
  RESTORE_DEPARTMENT: 'RESTORE_DEPARTMENT',
  SET_PAGINATION: 'SET_PAGINATION',
  SET_SEARCH_FILTERS: 'SET_SEARCH_FILTERS',
}; 