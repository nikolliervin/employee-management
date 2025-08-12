import React, { useEffect, useState, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button, Card, Badge } from 'react-bootstrap';
import DataTable from 'react-data-table-component';
import { toast } from 'react-toastify';
import { useDepartmentContext } from '../../context/DepartmentContext';
import { API_CONFIG, LOADING_STATES } from '../../types';
import LoadingSpinner from '../common/LoadingSpinner';
import ConfirmDialog from '../common/ConfirmDialog';
import SearchFilters from './SearchFilters';

/**
 * Department list component with search, pagination, and CRUD operations
 * @returns {JSX.Element} Department list component
 */
function DepartmentList() {

  const navigate = useNavigate();
  const {
    departments,
    pagination,
    searchFilters,
    loadingState,
    fetchDepartments,
    searchDepartments,
    deleteDepartment,
    updateSearchFilters,
    updatePagination,
  } = useDepartmentContext();
  


  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [departmentToDelete, setDepartmentToDelete] = useState(null);
  const [isDeleting, setIsDeleting] = useState(false);
  
  // Use ref to track if we've already initiated loading - refs don't cause re-renders
  const hasInitiatedRef = useRef(false);

  // Use useEffect to call fetchDepartments once, avoiding render-time state updates
  useEffect(() => {
    if (!hasInitiatedRef.current) {

      hasInitiatedRef.current = true;
      fetchDepartments(1, 10);
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
    if (filters.searchTerm) {
      searchDepartments({
        ...filters,
        pageNumber: 1,
        pageSize: pagination.pageSize,
      });
    } else {
      fetchDepartments(1, pagination.pageSize);
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
    
    if (searchFilters.searchTerm) {
      searchDepartments({
        ...searchFilters,
        pageNumber: newPageNumber,
        pageSize: pagination.pageSize,
      });
    } else {
      fetchDepartments(newPageNumber, pagination.pageSize);
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
    
    if (searchFilters.searchTerm) {
      searchDepartments({
        ...searchFilters,
        pageNumber: 1,
        pageSize: newPageSize,
      });
    } else {
      fetchDepartments(1, newPageSize);
    }
  };

  /**
   * Handle edit department
   * @param {string} id - Department ID
   */
  const handleEdit = (id) => {
    navigate(`/departments/edit/${id}`);
  };

  /**
   * Handle delete department
   * @param {string} id - Department ID
   */
  const handleDelete = (id) => {
    setDepartmentToDelete(id);
    setShowDeleteDialog(true);
  };

  /**
   * Confirm department deletion
   */
  const confirmDelete = async () => {
    if (!departmentToDelete) return;
    
    try {
      setIsDeleting(true);
      await deleteDepartment(departmentToDelete);
      setShowDeleteDialog(false);
      setDepartmentToDelete(null);
    } catch (error) {
      // Error is already handled by DepartmentContext with the actual API message
      // Just close the dialog and reset state
      setShowDeleteDialog(false);
      setDepartmentToDelete(null);
    } finally {
      setIsDeleting(false);
    }
  };

  /**
   * Cancel department deletion
   */
  const cancelDelete = () => {
    setShowDeleteDialog(false);
    setDepartmentToDelete(null);
  };

  /**
   * Get custom columns for the data table
   * @returns {Array} Columns configuration
   */
  const getCustomColumns = () => [
    {
      name: 'Name',
      selector: row => row.name,
      sortable: true,
      grow: 2,
    },
    {
      name: 'Description',
      selector: row => row.description || 'No description',
      sortable: true,
      grow: 3,
    },
    {
      name: 'Employees',
      selector: row => row.employeeCount || 0,
      sortable: true,
      grow: 1,
      cell: (row) => (
        <Badge bg="info" pill>
          {row.employeeCount || 0}
        </Badge>
      ),
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
      cell: (row) => (
        <div className="d-flex gap-2">
          <Button
            size="sm"
            variant="outline-primary"
            onClick={() => handleEdit(row.id)}
            title="Edit department"
          >
            <i className="bi bi-pencil"></i>
          </Button>
          <Button
            size="sm"
            variant="outline-danger"
            onClick={() => handleDelete(row.id)}
            title="Delete department"
          >
            <i className="bi bi-trash"></i>
          </Button>
        </div>
      ),
    },
  ];

  // Show loading spinner while loading
  if (loadingState === LOADING_STATES.LOADING && (!departments || departments.length === 0)) {
    return <LoadingSpinner text="Loading departments..." />;
  }

  return (
    <div>
      {/* Header */}
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h1 className="h2 mb-2 fw-bold" style={{ color: 'var(--text-primary)' }}>
            Department Management
          </h1>
        </div>
        
        <Button
          variant="primary"
          onClick={() => navigate('/departments/new')}
          className="d-flex align-items-center gap-2 px-4 py-2"
        >
          <i className="bi bi-plus-circle"></i>
          Add Department
        </Button>
      </div>

      {/* Search Filters */}
      <SearchFilters
        searchFilters={searchFilters}
        onFiltersChange={handleFiltersChange}
        onSearch={handleSearch}
      />

      {/* Department Table */}
      <Card className="shadow-sm">
        <Card.Body className="p-0">
          <DataTable
            title=""
            columns={getCustomColumns()}
            data={departments}
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
                  {searchFilters.searchTerm
                    ? 'No departments found matching your search criteria'
                    : 'No departments found. Add your first department to get started!'
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
            {pagination.totalCount} departments
          </small>
          
          <small>
            Page {pagination.pageNumber} of {pagination.totalPages}
          </small>
        </div>
      )}

      {/* Delete Confirmation Dialog */}
      <ConfirmDialog
        show={showDeleteDialog}
        title="Delete Department"
        message="Are you sure you want to delete this department? This action cannot be undone."
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

DepartmentList.propTypes = {};

export default DepartmentList;