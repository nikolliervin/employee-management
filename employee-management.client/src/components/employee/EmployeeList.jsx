import React, { useEffect, useState, useRef } from 'react';
import PropTypes from 'prop-types';
import { useNavigate } from 'react-router-dom';
import { Button, Row, Col, Card, Badge } from 'react-bootstrap';
import DataTable from 'react-data-table-component';
import { toast } from 'react-toastify';
import { useEmployeeContext } from '../../context/EmployeeContext';
import { EMPLOYEE_COLUMNS, API_CONFIG, LOADING_STATES } from '../../types';
import SearchFilters from './SearchFilters';
import LoadingSpinner from '../common/LoadingSpinner';
import ConfirmDialog from '../common/ConfirmDialog';
import { employeeService } from '../../services/employeeService';

/**
 * Employee list component with search, pagination, and CRUD operations
 * @returns {JSX.Element} Employee list component
 */
function EmployeeList() {

  const navigate = useNavigate();
  const {
    employees,
    pagination,
    searchFilters,
    loadingState,
    fetchEmployees,
    searchEmployees,
    deleteEmployee,
    updateSearchFilters,
    updatePagination,
  } = useEmployeeContext();
  


  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [employeeToDelete, setEmployeeToDelete] = useState(null);
  const [isDeleting, setIsDeleting] = useState(false);
  
  // Use ref to track if we've already initiated loading - refs don't cause re-renders
  const hasInitiatedRef = useRef(false);

  // Use useEffect to call fetchEmployees once, avoiding render-time state updates
  useEffect(() => {
    if (!hasInitiatedRef.current) {

      hasInitiatedRef.current = true;
      fetchEmployees(1, 10);
    } else {

    }
  }, []); // Empty dependency array - runs only once

  /**
   * Handle search filters change
   * @param {Object} filters - New search filters
   */
  const handleFiltersChange = (filters) => {
    updateSearchFilters(filters);
  };

  /**
   * Handle search execution
   * @param {Object} filters - Search filters to apply
   */
  const handleSearch = (filters) => {
    if (filters.searchTerm || filters.departmentId) {
      searchEmployees({
        ...filters,
        pageNumber: 1,
        pageSize: pagination.pageSize,
      });
    } else {
      fetchEmployees(1, pagination.pageSize);
    }
  };

  /**
   * Handle page change
   * @param {number} page - New page number
   */
  const handlePageChange = (page) => {
    const newPageNumber = page;

    
    // Prevent calling the same page multiple times
    if (newPageNumber === pagination.pageNumber) {

      return;
    }
    

    updatePagination({ pageNumber: newPageNumber });
    
    if (searchFilters.searchTerm || searchFilters.departmentId) {
      searchEmployees({
        ...searchFilters,
        pageNumber: newPageNumber,
        pageSize: pagination.pageSize,
      });
    } else {
      fetchEmployees(newPageNumber, pagination.pageSize);
    }
  };

  /**
   * Handle page size change
   * @param {number} newPageSize - New page size
   */
  const handlePageSizeChange = (newPageSize) => {

    
    // Prevent calling if the page size is the same
    if (newPageSize === pagination.pageSize) {

      return;
    }
    

    updatePagination({ pageSize: newPageSize, pageNumber: 1 });
    
    if (searchFilters.searchTerm || searchFilters.departmentId) {
      searchEmployees({
        ...searchFilters,
        pageNumber: 1,
        pageSize: newPageSize,
      });
    } else {
      fetchEmployees(1, newPageSize);
    }
  };

  /**
   * Handle edit employee
   * @param {string} id - Employee ID
   */
  const handleEdit = (id) => {
    navigate(`/employees/edit/${id}`);
  };

  /**
   * Handle delete employee
   * @param {string} id - Employee ID
   */
  const handleDelete = (id) => {
    setEmployeeToDelete(id);
    setShowDeleteDialog(true);
  };

  /**
   * Confirm employee deletion
   */
  const confirmDelete = async () => {
    if (!employeeToDelete) return;
    
    try {
      setIsDeleting(true);
      await deleteEmployee(employeeToDelete);
      setShowDeleteDialog(false);
      setEmployeeToDelete(null);
    } catch (error) {
      toast.error('Failed to delete employee');
    } finally {
      setIsDeleting(false);
    }
  };

  /**
   * Cancel employee deletion
   */
  const cancelDelete = () => {
    setShowDeleteDialog(false);
    setEmployeeToDelete(null);
  };

  /**
   * Get custom columns with action buttons
   * @returns {Array} Custom columns configuration
   */
  const getCustomColumns = () => {
    return [
      ...EMPLOYEE_COLUMNS.slice(0, -1), // All columns except Actions
      {
        name: 'Actions',
        selector: row => row.id,
        sortable: false,
        grow: 1,
        cell: (row) => (
          <div className="d-flex gap-1">
            <Button
              size="sm"
              variant="outline-primary"
              onClick={() => handleEdit(row.id)}
              title="Edit employee"
            >
              <i className="bi bi-pencil"></i>
            </Button>
            <Button
              size="sm"
              variant="outline-danger"
              onClick={() => handleDelete(row.id)}
              title="Delete employee"
            >
              <i className="bi bi-trash"></i>
            </Button>
          </div>
        ),
      },
    ];
  };

  // Show loading spinner while loading
  if (loadingState === LOADING_STATES.LOADING && employees.length === 0) {
    return <LoadingSpinner text="Loading employees..." />;
  }

  return (
    <div>
      {/* Header */}
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h1 className="h2 mb-2 fw-bold" style={{ color: 'var(--text-primary)' }}>
            Employee Management
          </h1>
        </div>
        
        <Button
          variant="primary"
          onClick={() => navigate('/employees/new')}
          className="d-flex align-items-center gap-2 px-4 py-2"
        >
          <i className="bi bi-plus-circle"></i>
          Add Employee
        </Button>
      </div>

      {/* Search Filters */}
      <SearchFilters
        filters={searchFilters}
        onFiltersChange={handleFiltersChange}
        onSearch={handleSearch}
        isLoading={loadingState === LOADING_STATES.LOADING}
      />

      {/* Employee Table */}
      <Card className="shadow-sm">
        <Card.Body className="p-0">
          <DataTable
            title=""
            columns={getCustomColumns()}
            data={employees}
            pagination
            paginationServer
            paginationTotalRows={pagination.totalCount}
            paginationPerPage={pagination.pageSize}
            paginationRowsPerPageOptions={API_CONFIG.PAGE_SIZE_OPTIONS}
            onChangePage={handlePageChange}
            onChangeRowsPerPage={handlePageSizeChange}
            progressPending={loadingState === LOADING_STATES.LOADING}
            progressComponent={<LoadingSpinner text="Loading..." size="sm" />}
            noDataComponent={
              <div className="text-center py-4">
                <p className="text-muted mb-0">
                  {searchFilters.searchTerm || searchFilters.departmentId
                    ? 'No employees found matching your search criteria'
                    : 'No employees found. Add your first employee to get started!'
                  }
                </p>
              </div>
            }
            responsive
            highlightOnHover
            pointerOnHover
            customStyles={{
              headRow: {
                style: {
                  backgroundColor: '#f8f9fa',
                  borderBottom: '2px solid #dee2e6',
                },
              },
              rows: {
                style: {
                  minHeight: '60px',
                },
              },
            }}
          />
        </Card.Body>
      </Card>

      {/* Pagination Info */}
      {pagination.totalCount > 0 && (
        <div className="d-flex justify-content-between align-items-center mt-3 text-muted">
          <small>
            Showing {((pagination.pageNumber - 1) * pagination.pageSize) + 1} to{' '}
            {Math.min(pagination.pageNumber * pagination.pageSize, pagination.totalCount)} of{' '}
            {pagination.totalCount} employees
          </small>
          
          <small>
            Page {pagination.pageNumber} of {pagination.totalPages}
          </small>
        </div>
      )}

      {/* Delete Confirmation Dialog */}
      <ConfirmDialog
        show={showDeleteDialog}
        title="Delete Employee"
        message="Are you sure you want to delete this employee? This action cannot be undone."
        confirmText="Delete"
        cancelText="Cancel"
        variant="danger"
        onConfirm={confirmDelete}
        onCancel={cancelDelete}
        isLoading={isDeleting}
      />
    </div>
  );
}

EmployeeList.propTypes = {};

export default EmployeeList; 