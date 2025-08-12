import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { Row, Col, Form, Button, Card } from 'react-bootstrap';

/**
 * Search filters component for departments
 * @param {Object} props - Component props
 * @param {Object} props.searchFilters - Current search filters
 * @param {Function} props.onFiltersChange - Callback when filters change
 * @param {Function} props.onSearch - Callback when search is executed
 * @returns {JSX.Element} Search filters component
 */
function SearchFilters({ searchFilters, onFiltersChange, onSearch }) {
  const [localFilters, setLocalFilters] = useState(searchFilters);

  /**
   * Handle filter value change
   * @param {string} field - Filter field name
   * @param {any} value - New value
   */
  const handleFilterChange = (field, value) => {
    const newFilters = { ...localFilters, [field]: value };
    setLocalFilters(newFilters);
    onFiltersChange(newFilters);
  };

  /**
   * Handle search execution
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
    };
    setLocalFilters(clearedFilters);
    onFiltersChange(clearedFilters);
    onSearch(clearedFilters);
  };

  /**
   * Handle Enter key press for search
   * @param {KeyboardEvent} e - Keyboard event
   */
  const handleKeyPress = (e) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      handleSearch();
    }
  };

  return (
    <Card className="mb-4 border-0 shadow-sm">
      <Card.Body className="bg-light">
        <Form onSubmit={(e) => { e.preventDefault(); handleSearch(); }}>
          <Row className="g-3 align-items-end">
            <Col md={8}>
              <Form.Group>
                <Form.Label className="fw-semibold text-dark">
                  Search Departments
                </Form.Label>
                <Form.Control
                  type="text"
                  placeholder="Search by name or description..."
                  value={localFilters.searchTerm}
                  onChange={(e) => handleFilterChange('searchTerm', e.target.value)}
                  onKeyPress={handleKeyPress}
                  className="border-0 shadow-sm"
                />
              </Form.Group>
            </Col>
            
            <Col md={4}>
              <div className="d-flex gap-2">
                <Button
                  variant="primary"
                  type="submit"
                  className="flex-fill"
                >
                  <i className="bi bi-search me-2"></i>
                  Search
                </Button>
                <Button
                  variant="outline-secondary"
                  onClick={handleClear}
                  className="flex-fill"
                >
                  <i className="bi bi-x-circle me-2"></i>
                  Clear
                </Button>
              </div>
            </Col>
          </Row>
        </Form>
      </Card.Body>
    </Card>
  );
}

SearchFilters.propTypes = {
  searchFilters: PropTypes.object.isRequired,
  onFiltersChange: PropTypes.func.isRequired,
  onSearch: PropTypes.func.isRequired,
};

export default SearchFilters;