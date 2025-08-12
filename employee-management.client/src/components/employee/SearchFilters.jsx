import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Form, Row, Col, Button } from 'react-bootstrap';
import Select from 'react-select';
import { useEmployeeContext } from '../../context/EmployeeContext';

/**
 * Search filters component for employee list
 * @param {Object} props - Component props
 * @param {Object} props.filters - Current search filters
 * @param {Function} props.onFiltersChange - Function to call when filters change
 * @param {Function} props.onSearch - Function to call when search is triggered
 * @param {boolean} props.isLoading - Whether search is in progress
 * @returns {JSX.Element} Search filters component
 */
function SearchFilters({ filters, onFiltersChange, onSearch, isLoading }) {
  const { departments, fetchAllDepartments } = useEmployeeContext();
  const [localFilters, setLocalFilters] = useState(filters);

  // Update local filters when props change
  useEffect(() => {
    setLocalFilters(filters);
  }, [filters]);

  // Fetch departments for dropdown
  useEffect(() => {
    const loadDepartments = async () => {
      if (!departments || departments.length === 0) {
        try {
          await fetchAllDepartments();
        } catch (error) {
          console.error('Error loading departments:', error);
        }
      }
    };

    loadDepartments();
  }, [departments, fetchAllDepartments]);

  /**
   * Handle filter change
   * @param {string} field - Field name
   * @param {any} value - New value
   */
  const handleFilterChange = (field, value) => {
    const newFilters = { ...localFilters, [field]: value };
    setLocalFilters(newFilters);
    onFiltersChange(newFilters);
  };

  /**
   * Handle search button click
   */
  const handleSearch = () => {
    onSearch(localFilters);
  };

  /**
   * Handle clear filters
   */
  const handleClear = () => {
    const clearedFilters = {
      searchTerm: '',
      departmentId: '',
    };
    setLocalFilters(clearedFilters);
    onFiltersChange(clearedFilters);
    onSearch(clearedFilters);
  };

  /**
   * Handle form submission
   * @param {Event} e - Form submit event
   */
  const handleSubmit = (e) => {
    e.preventDefault();
    handleSearch();
  };

  // Convert departments to react-select format
  const departmentOptions = [
    { value: '', label: 'All Departments' },
    ...departments.map(dept => ({
      value: dept.id,
      label: dept.name,
    })),
  ];

  return (
    <div className="search-filters">
      <Form onSubmit={handleSubmit}>
        <Row className="g-3 align-items-end">
          <Col md={4}>
            <Form.Group className="mb-0">
              <Form.Label htmlFor="searchTerm" className="form-label mb-2">
                Search
              </Form.Label>
              <Form.Control
                id="searchTerm"
                type="text"
                placeholder="Search by name or email..."
                value={localFilters.searchTerm}
                onChange={(e) => handleFilterChange('searchTerm', e.target.value)}
                disabled={isLoading}
                style={{ height: '44px' }}
              />
            </Form.Group>
          </Col>
          
          <Col md={4}>
            <Form.Group className="mb-0">
              <Form.Label htmlFor="departmentFilter" className="form-label mb-2">
                Department
              </Form.Label>
              <Select
                id="departmentFilter"
                value={departmentOptions.find(option => option.value === localFilters.departmentId)}
                onChange={(option) => handleFilterChange('departmentId', option?.value || '')}
                options={departmentOptions}
                isDisabled={isLoading}
                isClearable={false}
                className="react-select-container"
                classNamePrefix="react-select"
                placeholder="Select department..."
                styles={{
                  control: (provided) => ({
                    ...provided,
                    height: '44px',
                    minHeight: '44px',
                    border: '2px solid var(--border-light)',
                    borderRadius: '8px',
                    '&:hover': {
                      borderColor: 'var(--border-medium)',
                    },
                    '&:focus-within': {
                      borderColor: 'var(--primary)',
                      boxShadow: '0 0 0 3px rgba(99, 102, 241, 0.1)',
                    }
                  }),
                  valueContainer: (provided) => ({
                    ...provided,
                    height: '42px',
                    padding: '0 12px',
                  }),
                  input: (provided) => ({
                    ...provided,
                    margin: '0',
                    padding: '0',
                  }),
                  indicatorsContainer: (provided) => ({
                    ...provided,
                    height: '42px',
                  }),
                }}
              />
            </Form.Group>
          </Col>
          
          <Col md={4}>
            <div className="d-flex gap-2">
              <Button
                type="submit"
                variant="primary"
                disabled={isLoading}
                style={{ height: '44px' }}
                className="px-4"
              >
                {isLoading ? (
                  <>
                    <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                    Searching...
                  </>
                ) : (
                  'Search'
                )}
              </Button>
              
              <Button
                type="button"
                variant="outline-secondary"
                onClick={handleClear}
                disabled={isLoading}
                style={{ height: '44px' }}
                className="px-4"
              >
                Clear
              </Button>
            </div>
          </Col>
        </Row>
      </Form>
    </div>
  );
}

SearchFilters.propTypes = {
  filters: PropTypes.shape({
    searchTerm: PropTypes.string,
    departmentId: PropTypes.string,
  }).isRequired,
  onFiltersChange: PropTypes.func.isRequired,
  onSearch: PropTypes.func.isRequired,
  isLoading: PropTypes.bool,
};

export default SearchFilters; 