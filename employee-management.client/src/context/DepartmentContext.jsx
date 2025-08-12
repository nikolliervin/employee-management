import React, { createContext, useContext, useReducer, useMemo } from 'react';
import { toast } from 'react-toastify';
import { departmentService } from '../services/departmentService';
import { LOADING_STATES, TOAST_TYPES, ACTION_TYPES } from '../types';

const initialState = {
  departments: [],
  deletedDepartments: [],
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
  },
};

function departmentReducer(state, action) {
  
  switch (action.type) {
    case ACTION_TYPES.SET_LOADING:
      return { ...state, loadingState: action.payload, error: null };
    case ACTION_TYPES.SET_ERROR:
      return { ...state, error: action.payload, loadingState: LOADING_STATES.ERROR };
    case ACTION_TYPES.CLEAR_ERROR:
      return { ...state, error: null };
    case ACTION_TYPES.SET_DEPARTMENTS:
      return { ...state, departments: action.payload, loadingState: LOADING_STATES.SUCCESS };
    case ACTION_TYPES.SET_DELETED_DEPARTMENTS:
      return { ...state, deletedDepartments: action.payload, loadingState: LOADING_STATES.SUCCESS };
    case ACTION_TYPES.ADD_DEPARTMENT:
      return { ...state, departments: [...state.departments, action.payload], loadingState: LOADING_STATES.SUCCESS };
    case ACTION_TYPES.UPDATE_DEPARTMENT:
      return { ...state, departments: state.departments.map(dept => dept.id === action.payload.id ? action.payload : dept), loadingState: LOADING_STATES.SUCCESS };
    case ACTION_TYPES.REMOVE_DEPARTMENT:
      return { ...state, departments: state.departments.filter(dept => dept.id !== action.payload), loadingState: LOADING_STATES.SUCCESS };
    case ACTION_TYPES.RESTORE_DEPARTMENT:
      return { ...state, deletedDepartments: state.deletedDepartments.filter(dept => dept.id !== action.payload), loadingState: LOADING_STATES.SUCCESS };
    case ACTION_TYPES.SET_PAGINATION:
      return { ...state, pagination: { ...state.pagination, ...action.payload } };
    case ACTION_TYPES.SET_SEARCH_FILTERS:
      return { ...state, searchFilters: { ...state.searchFilters, ...action.payload } };
    default:
      return state;
  }
}

const DepartmentContext = createContext();

export function DepartmentProvider({ children }) {

  const [state, dispatch] = useReducer(departmentReducer, initialState);


  const actions = useMemo(() => {
  
    return {
      fetchDepartments: async (pageNumber = 1, pageSize = 10) => {
        
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
    
          const result = await departmentService.getDepartments(pageNumber, pageSize);
    
          
          dispatch({ type: ACTION_TYPES.SET_DEPARTMENTS, payload: result.data });
          dispatch({ type: ACTION_TYPES.SET_PAGINATION, payload: {
            pageNumber: result.pageNumber,
            pageSize: result.pageSize,
            totalCount: result.totalCount,
            totalPages: result.totalPages,
            hasPreviousPage: result.hasPreviousPage,
            hasNextPage: result.hasNextPage,
          }});
        } catch (error) {
          console.error('CONTEXT: fetchDepartments error:', error);
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
        }
      },

      searchDepartments: async (filters) => {
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
          const result = await departmentService.searchDepartments(filters);
          
          dispatch({ type: ACTION_TYPES.SET_DEPARTMENTS, payload: result.data });
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

      createDepartment: async (departmentData) => {
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
          const newDepartment = await departmentService.createDepartment(departmentData);
          
          dispatch({ type: ACTION_TYPES.ADD_DEPARTMENT, payload: newDepartment });
          toast.success('Department created successfully!', { type: TOAST_TYPES.SUCCESS });
        } catch (error) {
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
        }
      },

      updateDepartment: async (id, departmentData) => {
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
          const updatedDepartment = await departmentService.updateDepartment(id, departmentData);
          
          dispatch({ type: ACTION_TYPES.UPDATE_DEPARTMENT, payload: updatedDepartment });
          toast.success('Department updated successfully!', { type: TOAST_TYPES.SUCCESS });
        } catch (error) {
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
        }
      },

      deleteDepartment: async (id) => {
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
          await departmentService.deleteDepartment(id);
          
          dispatch({ type: ACTION_TYPES.REMOVE_DEPARTMENT, payload: id });
          toast.success('Department deleted successfully!', { type: TOAST_TYPES.SUCCESS });
        } catch (error) {
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
        }
      },

      fetchDeletedDepartments: async (pageNumber = 1, pageSize = 10) => {
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
          const result = await departmentService.getDeletedDepartments(pageNumber, pageSize);
          
          dispatch({ type: ACTION_TYPES.SET_DELETED_DEPARTMENTS, payload: result });
        } catch (error) {
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
        }
      },

      restoreDepartment: async (id) => {
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
          await departmentService.restoreDepartment(id);
          
          dispatch({ type: ACTION_TYPES.RESTORE_DEPARTMENT, payload: id });
          toast.success('Department restored successfully!', { type: TOAST_TYPES.SUCCESS });
        } catch (error) {
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
        }
      },

      fetchDepartmentById: async (id) => {
        try {
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.LOADING });
          const department = await departmentService.getDepartmentById(id);
          
          dispatch({ type: ACTION_TYPES.SET_LOADING, payload: LOADING_STATES.SUCCESS });
          return department;
        } catch (error) {
          dispatch({ type: ACTION_TYPES.SET_ERROR, payload: error.message });
          toast.error(error.message, { type: TOAST_TYPES.ERROR });
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
    <DepartmentContext.Provider value={value}>
      {children}
    </DepartmentContext.Provider>
  );
}

export function useDepartmentContext() {
  const context = useContext(DepartmentContext);
  if (!context) {
    throw new Error('useDepartmentContext must be used within a DepartmentProvider');
  }
  return context;
}