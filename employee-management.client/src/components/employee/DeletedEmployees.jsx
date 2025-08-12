import React, { useEffect, useState, useRef, useCallback } from 'react';
import PropTypes from 'prop-types';
import { Button, Card, Badge } from 'react-bootstrap';
import DataTable from 'react-data-table-component';
import { toast } from 'react-toastify';
import { employeeService } from '../../services/employeeService';
import { API_CONFIG, LOADING_STATES } from '../../types';
import LoadingSpinner from '../common/LoadingSpinner';
import ConfirmDialog from '../common/ConfirmDialog';

/**
 * Deleted employees component for viewing and restoring deleted employees
 * @returns {JSX.Element} Deleted employees component
 */
function DeletedEmployees() {
  // Local state management to avoid context issues
  const [deletedEmployees, setDeletedEmployees] = useState([]);
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
  const [employeeToRestore, setEmployeeToRestore] = useState(null);
  const [isRestoring, setIsRestoring] = useState(false);

  // Use ref to track if we've already initiated loading
  const hasInitiatedRef = useRef(false);

  /**
   * Fetch deleted employees from API - memoized to prevent recreation
   * @param {number} pageNumber - Page number
   * @param {number} pageSize - Page size
   */
  const fetchDeletedEmployees = useCallback(async (pageNumber = 1, pageSize = 10) => {
    try {
      setLoadingState(LOADING_STATES.LOADING);
      const result = await employeeService.getDeletedEmployees(pageNumber, pageSize);
      
      // The API returns a simple array, not a paginated result
      const deletedEmployeesArray = Array.isArray(result) ? result : [];
      setDeletedEmployees(deletedEmployeesArray);
      
      // Since there's no pagination in the backend, simulate it for the UI
      setPagination({
        pageNumber: 1,
        pageSize: deletedEmployeesArray.length,
        totalCount: deletedEmployeesArray.length,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      });
      setLoadingState(LOADING_STATES.SUCCESS);
    } catch (error) {
      setLoadingState(LOADING_STATES.ERROR);
      toast.error(error.message || 'Failed to fetch deleted employees');
    }
  }, []);

  /**
   * Restore employee
   * @param {string} id - Employee ID
   */
  const restoreEmployee = async (id) => {
    try {
      setLoadingState(LOADING_STATES.LOADING);
      await employeeService.restoreEmployee(id);
      
      // Remove restored employee from the list
      setDeletedEmployees(prev => prev.filter(emp => emp.id !== id));
      toast.success('Employee restored successfully!');
      setLoadingState(LOADING_STATES.SUCCESS);
    } catch (error) {
      setLoadingState(LOADING_STATES.ERROR);
      toast.error(error.message || 'Failed to restore employee');
    }
  };

  // Load deleted employees once on component mount
  useEffect(() => {
    if (!hasInitiatedRef.current) {
      hasInitiatedRef.current = true;
      fetchDeletedEmployees(1, 10);
    }
  }, [fetchDeletedEmployees]);

  // Note: Page change handlers are no longer needed since we're using client-side pagination

  /**
   * Handle restore employee
   * @param {string} id - Employee ID
   */
  const handleRestore = (id) => {
    setEmployeeToRestore(id);
    setShowRestoreDialog(true);
  };

  /**
   * Confirm employee restoration
   */
  const confirmRestore = async () => {
    if (!employeeToRestore) return;
    
    try {
      setIsRestoring(true);
      await restoreEmployee(employeeToRestore);
      setShowRestoreDialog(false);
      setEmployeeToRestore(null);
      
      // Update pagination count after restore
      setPagination(prev => ({
        ...prev,
        totalCount: Math.max(0, prev.totalCount - 1)
      }));
    } catch (error) {
      toast.error('Failed to restore employee');
    } finally {
      setIsRestoring(false);
    }
  };

  /**
   * Cancel employee restoration
   */
  const cancelRestore = () => {
    setShowRestoreDialog(false);
    setEmployeeToRestore(null);
  };

  /**
   * Get columns for deleted employees table
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
          title="Restore employee"
        >
          <i className="bi bi-arrow-clockwise"></i>
          Restore
        </Button>
      ),
    },
  ];

  // Show loading spinner while loading
  if (loadingState === LOADING_STATES.LOADING && (!deletedEmployees || deletedEmployees.length === 0)) {
    return <LoadingSpinner text="Loading deleted employees..." />;
  }

  return (
    <div>
      {/* Status Badge */}
      <div className="d-flex justify-content-end mb-4">
        <Badge bg="secondary" className="fs-6">
          {pagination.totalCount} deleted employees
        </Badge>
      </div>

      {/* Deleted Employees Table */}
      <Card className="shadow-sm">
        <Card.Body className="p-0">
          <DataTable
            title=""
            columns={getColumns()}
            data={deletedEmployees || []}
            pagination
            paginationRowsPerPageOptions={[10, 25, 50]}
            progressPending={loadingState === LOADING_STATES.LOADING}
            progressComponent={<LoadingSpinner text="Loading..." size="sm" />}
            noDataComponent={
              <div className="text-center py-4">
                <p className="text-muted mb-0">
                  No deleted employees found
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
        title="Restore Employee"
        message="Are you sure you want to restore this employee? They will be available in the main employee list again."
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

DeletedEmployees.propTypes = {};

export default DeletedEmployees; 