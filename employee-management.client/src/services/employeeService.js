import { API_CONFIG } from '../types';

/**
 * Employee API service for handling all employee-related operations
 */
class EmployeeService {
  /**
   * Get the base URL for API requests
   * @returns {string} Base API URL
   */
  getBaseUrl() {
    return API_CONFIG.BASE_URL;
  }

  /**
   * Handle API response and extract data or throw errors
   * @param {Response} response - Fetch response object
   * @returns {Promise<any>} Parsed response data
   * @throws {Error} If response is not successful
   */
  async handleResponse(response) {
    if (!response.ok) {
      const errorText = await response.text();
      let errorData;
      
      try {
        errorData = JSON.parse(errorText);
        
        // Handle API response wrapper in error cases
        if (errorData && typeof errorData === 'object' && 'isSuccess' in errorData) {
          const errorMessage = (errorData.errors && errorData.errors.length > 0) 
            ? errorData.errors.join(', ') 
            : (errorData.message || 'Operation failed');
          throw new Error(errorMessage);
        }
      } catch (parseError) {
        // If it's not our custom Error from above, treat as JSON parse error
        if (!(parseError instanceof Error && parseError.message !== parseError.constructor.name)) {
          errorData = { message: errorText || 'An error occurred' };
        } else {
          throw parseError; // Re-throw our custom error
        }
      }
      
      throw new Error(errorData.message || `HTTP ${response.status}: ${response.statusText}`);
    }
    
    const data = await response.json();
    
    // Handle API response wrapper
    if (data && typeof data === 'object' && 'isSuccess' in data) {
      if (!data.isSuccess) {
        // Prioritize the detailed errors array, then fallback to message
        const errorMessage = (data.errors && data.errors.length > 0) 
          ? data.errors.join(', ') 
          : data.message || 'Operation failed';
        throw new Error(errorMessage);
      }
      return data.data;
    }
    
    return data;
  }

  /**
   * Get paginated list of employees
   * @param {number} pageNumber - Page number (1-based)
   * @param {number} pageSize - Page size
   * @returns {Promise<PaginatedResult>} Paginated employee results
   */
  async getEmployees(pageNumber = 1, pageSize = API_CONFIG.DEFAULT_PAGE_SIZE) {
    try {
      const url = new URL(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.EMPLOYEES}`);
      url.searchParams.append('pageNumber', pageNumber.toString());
      url.searchParams.append('pageSize', pageSize.toString());
      
      const response = await fetch(url.toString(), {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      
      return await this.handleResponse(response);
    } catch (error) {
      console.error('Error fetching employees:', error);
      throw error;
    }
  }

  /**
   * Get employee by ID
   * @param {string} id - Employee ID
   * @returns {Promise<Employee>} Employee data
   */
  async getEmployeeById(id) {
    try {
      const response = await fetch(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.EMPLOYEE_BY_ID(id)}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      
      return await this.handleResponse(response);
    } catch (error) {
      console.error(`Error fetching employee ${id}:`, error);
      throw error;
    }
  }

  /**
   * Create new employee
   * @param {CreateEmployeeRequest} employeeData - Employee data to create
   * @returns {Promise<Employee>} Created employee data
   */
  async createEmployee(employeeData) {
    try {
      const response = await fetch(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.EMPLOYEES}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(employeeData),
      });
      
      return await this.handleResponse(response);
    } catch (error) {
      console.error('Error creating employee:', error);
      throw error;
    }
  }

  /**
   * Update existing employee
   * @param {string} id - Employee ID
   * @param {UpdateEmployeeRequest} employeeData - Updated employee data
   * @returns {Promise<Employee>} Updated employee data
   */
  async updateEmployee(id, employeeData) {
    try {
      const response = await fetch(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.EMPLOYEE_BY_ID(id)}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(employeeData),
      });
      
      return await this.handleResponse(response);
    } catch (error) {
      console.error(`Error updating employee ${id}:`, error);
      throw error;
    }
  }

  /**
   * Delete employee
   * @param {string} id - Employee ID
   * @returns {Promise<void>} Success response
   */
  async deleteEmployee(id) {
    try {
      const response = await fetch(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.EMPLOYEE_BY_ID(id)}`, {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      
      return await this.handleResponse(response);
    } catch (error) {
      console.error(`Error deleting employee ${id}:`, error);
      throw error;
    }
  }

  /**
   * Search employees with filters
   * @param {SearchRequest} searchParams - Search parameters
   * @returns {Promise<PaginatedResult>} Search results
   */
  async searchEmployees(searchParams) {
    try {
      const url = new URL(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.EMPLOYEE_SEARCH}`);
      
      // Add search parameters
      Object.entries(searchParams).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          url.searchParams.append(key, value.toString());
        }
      });
      
      const response = await fetch(url.toString(), {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      
      return await this.handleResponse(response);
    } catch (error) {
      console.error('Error searching employees:', error);
      throw error;
    }
  }

  /**
   * Get deleted employees
   * @param {number} pageNumber - Page number (1-based)
   * @param {number} pageSize - Page size
   * @returns {Promise<PaginatedResult>} Paginated deleted employee results
   */
  async getDeletedEmployees(pageNumber = 1, pageSize = API_CONFIG.DEFAULT_PAGE_SIZE) {
    try {
      const url = new URL(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.DELETED_EMPLOYEES}`);
      url.searchParams.append('pageNumber', pageNumber.toString());
      url.searchParams.append('pageSize', pageSize.toString());
      
      const response = await fetch(url.toString(), {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      
      return await this.handleResponse(response);
    } catch (error) {
      console.error('Error fetching deleted employees:', error);
      throw error;
    }
  }

  /**
   * Restore deleted employee
   * @param {string} id - Employee ID
   * @returns {Promise<Employee>} Restored employee data
   */
  async restoreEmployee(id) {
    try {
      const response = await fetch(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.RESTORE_EMPLOYEE(id)}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      
      return await this.handleResponse(response);
    } catch (error) {
      console.error(`Error restoring employee ${id}:`, error);
      throw error;
    }
  }
}

// Export singleton instance
export const employeeService = new EmployeeService();
export default employeeService; 