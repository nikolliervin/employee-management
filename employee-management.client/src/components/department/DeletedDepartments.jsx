import React, { useEffect, useState, useRef, useCallback } from 'react';
import PropTypes from 'prop-types';
import { Button, Card, Badge } from 'react-bootstrap';
import DataTable from 'react-data-table-component';
import { toast } from 'react-toastify';
import { departmentService } from '../../services/departmentService';
import { API_CONFIG, LOADING_STATES } from '../../types';
import LoadingSpinner from '../common/LoadingSpinner';
import ConfirmDialog from '../common/ConfirmDialog';

/**
 * Deleted departments component for viewing and restoring deleted departments
 * @returns {JSX.Element} Deleted departments component
 */
function DeletedDepartments() {
  // Local state management to avoid context issues
  const [deletedDepartments, setDeletedDepartments] = useState([]);
  const [pagination, setPagination] = useState({
    pageNumber: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0,
    hasPreviousPage: false,
    hasNextPage: false,
  });
  const [loadingState, setLoadingState] = useState(LOADING_STATES.IDLE);
  const [showRestoreDialog, setShowRestoreDialog] = useState(false);
  const [departmentToRestore, setDepartmentToRestore] = useState(null);
  const [isRestoring, setIsRestoring] = useState(false);

  // Use ref to track if we've already initiated loading
  const hasInitiatedRef = useRef(false);

  /**
   * Fetch deleted departments from API - memoized to prevent recreation
   * @param {number} pageNumber - Page number
   * @param {number} pageSize - Page size
   */
  const fetchDeletedDepartments = useCallback(async (pageNumber = 1, pageSize = 10) => {
    try {
      setLoadingState(LOADING_STATES.LOADING);
      const result = await departmentService.getDeletedDepartments(pageNumber, pageSize);
      
      // The API returns a simple array, not a paginated result
      const deletedDepartmentsArray = Array.isArray(result) ? result : [];
      setDeletedDepartments(deletedDepartmentsArray);
      
      // Since there's no pagination in the backend, simulate it for the UI
      setPagination({
        pageNumber: 1,
        pageSize: deletedDepartmentsArray.length,
        totalCount: deletedDepartmentsArray.length,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      });
      setLoadingState(LOADING_STATES.SUCCESS);
    } catch (error) {
      setLoadingState(LOADING_STATES.ERROR);
      toast.error(error.message || 'Failed to fetch deleted departments');
    }
  }, []);

  /**
   * Restore department
   * @param {string} id - Department ID
   */
  const restoreDepartment = async (id) => {
    try {
      setLoadingState(LOADING_STATES.LOADING);
      await departmentService.restoreDepartment(id);
      
      // Remove restored department from the list
      setDeletedDepartments(prev => prev.filter(dept => dept.id !== id));
      toast.success('Department restored successfully!');
      setLoadingState(LOADING_STATES.SUCCESS);
    } catch (error) {
      setLoadingState(LOADING_STATES.ERROR);
      toast.error(error.message || 'Failed to restore department');
    }
  };

  // Load deleted departments once on component mount
  useEffect(() => {
    if (!hasInitiatedRef.current) {
      hasInitiatedRef.current = true;
      fetchDeletedDepartments(1, 10);
    }
  }, [fetchDeletedDepartments]);

  // Note: Page change handlers are no longer needed since we're using client-side pagination

  /**
   * Handle restore department
   * @param {string} id - Department ID
   */
  const handleRestore = (id) => {
    setDepartmentToRestore(id);
    setShowRestoreDialog(true);
  };

  /**
   * Confirm department restoration
   */
  const confirmRestore = async () => {
    if (!departmentToRestore) return;
    
    try {
      setIsRestoring(true);
      await restoreDepartment(departmentToRestore);
      setShowRestoreDialog(false);
      setDepartmentToRestore(null);
      
      // Update pagination count after restore
      setPagination(prev => ({
        ...prev,
        totalCount: Math.max(0, prev.totalCount - 1)
      }));
    } catch (error) {
      toast.error('Failed to restore department');
    } finally {
      setIsRestoring(false);
    }
  };

  /**
   * Cancel department restoration
   */
  const cancelRestore = () => {
    setShowRestoreDialog(false);
    setDepartmentToRestore(null);
  };

  /**
   * Get columns for deleted departments table
   * @returns {Array} Table columns configuration
   */
  const getColumns = () => [
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
      name: 'Employee Count',
      selector: row => row.employeeCount || 0,
      sortable: true,
      grow: 1,
      cell: (row) => (
        <Badge bg="secondary" pill>
          {row.employeeCount || 0}
        </Badge>
      ),
    },
    {
      name: 'Deleted',
      selector: row => new Date(row.updatedAt || row.createdAt).toLocaleDateString(),
      sortable: true,
      grow: 1,
    },
    {
      name: 'Actions',
      selector: row => row.id,
      sortable: false,
      grow: 1,
      cell: (row) => (
        <Button
          size="sm"
          variant="outline-success"
          onClick={() => handleRestore(row.id)}
          title="Restore department"
        >
          <i className="bi bi-arrow-clockwise"></i>
          Restore
        </Button>
      ),
    },
  ];

  // Show loading spinner while loading
  if (loadingState === LOADING_STATES.LOADING && (!deletedDepartments || deletedDepartments.length === 0)) {
    return <LoadingSpinner text="Loading deleted departments..." />;
  }

  return (
    <div>
      {/* Status Badge */}
      <div className="d-flex justify-content-end mb-4">
        <Badge bg="secondary" className="fs-6">
          {pagination.totalCount} deleted departments
        </Badge>
      </div>

      {/* Deleted Departments Table */}
      <Card className="shadow-sm">
        <Card.Body className="p-0">
          <DataTable
            title=""
            columns={getColumns()}
            data={deletedDepartments || []}
            pagination
            paginationRowsPerPageOptions={[10, 25, 50]}
            progressPending={loadingState === LOADING_STATES.LOADING}
            progressComponent={<LoadingSpinner text="Loading..." size="sm" />}
            noDataComponent={
              <div className="text-center py-4">
                <p className="text-muted mb-0">
                  No deleted departments found
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

      {/* Restore Confirmation Dialog */}
      <ConfirmDialog
        show={showRestoreDialog}
        title="Restore Department"
        message="Are you sure you want to restore this department? It will be available in the main department list again."
        confirmText="Restore"
        cancelText="Cancel"
        variant="success"
        onConfirm={confirmRestore}
        onCancel={cancelRestore}
        isLoading={isRestoring}
      />
    </div>
  );
}

DeletedDepartments.propTypes = {};

export default DeletedDepartments;