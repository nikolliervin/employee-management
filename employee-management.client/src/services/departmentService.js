import { API_CONFIG } from '../types';

/**
 * Department API service for handling all department-related operations
 */
class DepartmentService {
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
          : (data.message || 'Operation failed');
        throw new Error(errorMessage);
      }
      return data.data;
    }
    
    return data;
  }

  /**
   * Get all departments without pagination (for dropdowns)
   * @returns {Promise<Array>} All departments array
   */
  async getAllDepartments() {
    try {
      const url = new URL(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.DEPARTMENTS}`);
      url.searchParams.append('pageNumber', '1');
      url.searchParams.append('pageSize', '100'); // Maximum allowed page size to get all departments
      
      const response = await fetch(url.toString(), {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      
      const result = await this.handleResponse(response);
      return result.data || []; // Return the departments array from the paginated result
    } catch (error) {
      console.error('Error fetching all departments:', error);
      throw error;
    }
  }

  /**
   * Get paginated list of departments
   * @param {number} pageNumber - Page number (1-based)
   * @param {number} pageSize - Page size
   * @returns {Promise<PaginatedResult>} Paginated department results
   */
  async getDepartments(pageNumber = 1, pageSize = API_CONFIG.DEFAULT_PAGE_SIZE) {
    try {
      const url = new URL(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.DEPARTMENTS}`);
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
      console.error('Error fetching departments:', error);
      throw error;
    }
  }

  /**
   * Get department by ID
   * @param {string} id - Department ID
   * @returns {Promise<Department>} Department data
   */
  async getDepartmentById(id) {
    try {
      const response = await fetch(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.DEPARTMENT_BY_ID(id)}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      
      return await this.handleResponse(response);
    } catch (error) {
      console.error(`Error fetching department ${id}:`, error);
      throw error;
    }
  }

  /**
   * Create new department
   * @param {CreateDepartmentRequest} departmentData - Department data to create
   * @returns {Promise<Department>} Created department data
   */
  async createDepartment(departmentData) {
    try {
      const response = await fetch(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.DEPARTMENTS}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(departmentData),
      });
      
      return await this.handleResponse(response);
    } catch (error) {
      console.error('Error creating department:', error);
      throw error;
    }
  }

  /**
   * Update existing department
   * @param {string} id - Department ID
   * @param {UpdateDepartmentRequest} departmentData - Updated department data
   * @returns {Promise<Department>} Updated department data
   */
  async updateDepartment(id, departmentData) {
    try {
      const response = await fetch(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.DEPARTMENT_BY_ID(id)}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(departmentData),
      });
      
      return await this.handleResponse(response);
    } catch (error) {
      console.error(`Error updating department ${id}:`, error);
      throw error;
    }
  }

  /**
   * Delete department
   * @param {string} id - Department ID
   * @returns {Promise<void>} Success response
   */
  async deleteDepartment(id) {
    try {
      const response = await fetch(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.DEPARTMENT_BY_ID(id)}`, {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      
      return await this.handleResponse(response);
    } catch (error) {
      console.error(`Error deleting department ${id}:`, error);
      throw error;
    }
  }

  /**
   * Search departments with filters
   * @param {SearchRequest} searchParams - Search parameters
   * @returns {Promise<PaginatedResult>} Search results
   */
  async searchDepartments(searchParams) {
    try {
      const url = new URL(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.DEPARTMENT_SEARCH}`);
      
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
      console.error('Error searching departments:', error);
      throw error;
    }
  }

  /**
   * Get deleted departments
   * @param {number} pageNumber - Page number (1-based)
   * @param {number} pageSize - Page size
   * @returns {Promise<Array>} Deleted departments array
   */
  async getDeletedDepartments(pageNumber = 1, pageSize = API_CONFIG.DEFAULT_PAGE_SIZE) {
    try {
      const url = new URL(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.DELETED_DEPARTMENTS}`);
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
      console.error('Error fetching deleted departments:', error);
      throw error;
    }
  }

  /**
   * Restore deleted department
   * @param {string} id - Department ID
   * @returns {Promise<Department>} Restored department data
   */
  async restoreDepartment(id) {
    try {
      const response = await fetch(`${this.getBaseUrl()}${API_CONFIG.ENDPOINTS.RESTORE_DEPARTMENT(id)}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
      });
      
      return await this.handleResponse(response);
    } catch (error) {
      console.error(`Error restoring department ${id}:`, error);
      throw error;
    }
  }
}

// Export singleton instance
export const departmentService = new DepartmentService();
export default departmentService;