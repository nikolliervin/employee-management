import React, { createContext, useContext, useReducer, useMemo } from 'react';
import { toast } from 'react-toastify';
import { employeeService } from '../services/employeeService';
import { departmentService } from '../services/departmentService';
import { LOADING_STATES, TOAST_TYPES, ACTION_TYPES } from '../types';

const initialState = {
  employees: [],
  deletedEmployees: [],
  departments: [], // Add departments for dropdown
  loadingState: LOADING_STATES.IDLE,
  error: null,
  pagination: {
    pageNumber: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0,
    hasPreviousPage: false,
    hasNextPage: false,
  },
  searchFilters: {
    searchTerm: '',
    departmentId: null,
  },
};

function employeeReducer(state, action) {
  
  switch (action.type) {
    case ACTION_TYPES.SET_LOADING:
      return { ...state, loadingState: action.payload, error: null };
    case ACTION_TYPES.SET_ERROR:
      return { ...state, error: action.payload, loadingState: LOADING_STATES.ERROR };
    case ACTION_TYPES.CLEAR_ERROR:
      return { ...state, error: null };
    case ACTION_TYPES.SET_EMPLOYEES:
      return { ...state, employees: action.payload, loadingState: LOADING_STATES.SUCCESS };
    case ACTION_TYPES.SET_DELETED_EMPLOYEES:
      return { ...state, deletedEmployees: action.payload, loadingState: LOADING_STATES.SUCCESS };
    case ACTION_TYPES.SET_DEPARTMENTS:
      return { ...state, departments: action.payload };
    case ACTION_TYPES.ADD_EMPLOYEE:
      return { ...state, employees: [...state.employees, action.payload], loadingState: LOADING_STATES.SUCCESS };
    case ACTION_TYPES.UPDATE_EMPLOYEE:
      return { ...state, employees: state.employees.map(emp => emp.id === action.payload.id ? action.payload : emp), loadingState: LOADING_STATES.SUCCESS };
    case ACTION_TYPES.REMOVE_EMPLOYEE:
      return { ...state, employees: state.employees.filter(emp => emp.id !== action.payload), loadingState: LOADING_STATES.SUCCESS };
    case ACTION_TYPES.RESTORE_EMPLOYEE:
      return { ...state, deletedEmployees: state.deletedEmployees.filter(emp => emp.id !== action.payload), loadingState: LOADING_STATES.SUCCESS };
    case ACTION_TYPES.SET_PAGINATION:
      return { ...state, pagination: { ...state.pagination, ...action.payload } };
    case ACTION_TYPES.SET_SEARCH_FILTERS:
      return { ...state, searchFilters: { ...state.searchFilters, ...action.payload } };
    default:
      return state;
  }
}

const EmployeeContext = createContext();

export function EmployeeProvider({ children }) {

  const [state, dispatch] = useReducer(employeeReducer, initialState);
    

  const actions = useMemo(() => {
  
    return {
      fetchEmployees: async (pageNumber = 1, pageSize = 10) => {
        
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
    
          const result = await employeeService.getEmployees(pageNumber, pageSize);
    
          
          dispatch({ type: ACTION_TYPES.SET_EMPLOYEES, payload: result.data });
          dispatch({ type: ACTION_TYPES.SET_PAGINATION, payload: {
            pageNumber: result.pageNumber,
            pageSize: result.pageSize,
            totalCount: result.totalCount,
            totalPages: result.totalPages,
            hasPreviousPage: result.hasPreviousPage,
            hasNextPage: result.hasNextPage,
          }});
        } catch (error) {
          console.error('CONTEXT: fetchEmployees error:', error);
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
        }
      },

      searchEmployees: async (filters) => {
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
          const result = await employeeService.searchEmployees(filters);
          
          dispatch({ type: ACTION_TYPES.SET_EMPLOYEES, payload: result.data });
          dispatch({ type: ACTION_TYPES.SET_PAGINATION, payload: {
            pageNumber: result.pageNumber,
            pageSize: result.pageSize,
            totalCount: result.totalCount,
            totalPages: result.totalPages,
            hasPreviousPage: result.hasPreviousPage,
            hasNextPage: result.hasNextPage,
          }});
        } catch (error) {
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
        }
      },

      createEmployee: async (employeeData) => {
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
          const newEmployee = await employeeService.createEmployee(employeeData);
          
          dispatch({ type: ACTION_TYPES.ADD_EMPLOYEE, payload: newEmployee });
          toast.success('Employee created successfully!', { type: TOAST_TYPES.SUCCESS });
        } catch (error) {
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
        }
      },

      updateEmployee: async (id, employeeData) => {
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
          const updatedEmployee = await employeeService.updateEmployee(id, employeeData);
          
          dispatch({ type: ACTION_TYPES.UPDATE_EMPLOYEE, payload: updatedEmployee });
          toast.success('Employee updated successfully!', { type: TOAST_TYPES.SUCCESS });
        } catch (error) {
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
        }
      },

      deleteEmployee: async (id) => {
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
          await employeeService.deleteEmployee(id);
          
          dispatch({ type: ACTION_TYPES.REMOVE_EMPLOYEE, payload: id });
          toast.success('Employee deleted successfully!', { type: TOAST_TYPES.SUCCESS });
        } catch (error) {
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
        }
      },

      fetchDeletedEmployees: async (pageNumber = 1, pageSize = 10) => {
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
          const result = await employeeService.getDeletedEmployees(pageNumber, pageSize);
          
          dispatch({ type: ACTION_TYPES.SET_DELETED_EMPLOYEES, payload: result.data });
          dispatch({ type: ACTION_TYPES.SET_PAGINATION, payload: {
            pageNumber: result.pageNumber,
            pageSize: result.pageSize,
            totalCount: result.totalCount,
            totalPages: result.totalPages,
            hasPreviousPage: result.hasPreviousPage,
            hasNextPage: result.hasNextPage,
          }});
        } catch (error) {
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
        }
      },

      restoreEmployee: async (id) => {
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
          await employeeService.restoreEmployee(id);
          
          dispatch({ type: ACTION_TYPES.RESTORE_EMPLOYEE, payload: id });
          toast.success('Employee restored successfully!', { type: TOAST_TYPES.SUCCESS });
        } catch (error) {
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
        }
      },

      fetchEmployeeById: async (id) => {
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
          const employee = await employeeService.getEmployeeById(id);
          
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.SUCCESS });
          return employee;
        } catch (error) {
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
          throw error;
        }
      },

      fetchAllDepartments: async () => {
        try {
          const departments = await departmentService.getAllDepartments();
          dispatch({ type: ACTION_TYPES.SET_DEPARTMENTS, payload: departments });
          return departments;
        } catch (error) {
          console.error('Error fetching departments:', error);
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error('Failed to load departments', { type: TOAST_TYPES.ERROR });
          throw error;
        }
      },

      updateSearchFilters: (filters) => {
        dispatch({ type: ACTION_TYPES.SET_SEARCH_FILTERS, payload: filters });
      },

      updatePagination: (pagination) => {
        dispatch({ type: ACTION_TYPES.SET_PAGINATION, payload: pagination });
      },

      setLoading: (loadingState) => {
        dispatch({ type: ACTION_TYPES.SET_LOADING, payload: loadingState });
      },

      setError: (error) => {
        dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error });
        toast.error(error, { type: TOAST_TYPES.ERROR });
      },

      clearError: () => {
        dispatch({ type: ACTION_TYPES.CLEAR_ERROR });
      },
    };
  }, []); // Empty dependency array - these functions never change

  const value = useMemo(() => {

    return {
      ...state,
      ...actions,
    };
  }, [state, actions]);

  return (
    <EmployeeContext.Provider value={value}>
      {children}
    </EmployeeContext.Provider>
  );
}

export function useEmployeeContext() {
  const context = useContext(EmployeeContext);
  if (!context) {
    throw new Error('useEmployeeContext must be used within an EmployeeProvider');
  }
  return context;
}