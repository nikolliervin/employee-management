import React, { useState } from 'react';
import { Card, Nav } from 'react-bootstrap';
import DeletedEmployees from '../employee/DeletedEmployees';
import DeletedDepartments from '../department/DeletedDepartments';

/**
 * Unified deleted items component with toggle between employees and departments
 * @returns {JSX.Element} Deleted items component
 */
function DeletedItems() {
  const [activeTab, setActiveTab] = useState('employees');

  /**
   * Handle tab change
   * @param {string} tab - Tab to activate
   */
  const handleTabChange = (tab) => {
    setActiveTab(tab);
  };

  return (
    <div>
      {/* Header */}
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h1 className="h2 mb-2 fw-bold" style={{ color: 'var(--text-primary)' }}>
            Deleted Items
          </h1>
          <p className="text-secondary mb-0 fs-6">
            View and restore deleted employees and departments
          </p>
        </div>
      </div>

      {/* Tab Navigation */}
      <Card className="shadow-sm mb-4">
        <Card.Header className="bg-white border-bottom">
          <Nav variant="tabs" className="card-header-tabs">
            <Nav.Item>
              <Nav.Link
                active={activeTab === 'employees'}
                onClick={() => handleTabChange('employees')}
                className="fw-semibold"
                style={{ cursor: 'pointer' }}
              >
                <i className="bi bi-people me-2"></i>
                Deleted Employees
              </Nav.Link>
            </Nav.Item>
            <Nav.Item>
              <Nav.Link
                active={activeTab === 'departments'}
                onClick={() => handleTabChange('departments')}
                className="fw-semibold"
                style={{ cursor: 'pointer' }}
              >
                <i className="bi bi-building me-2"></i>
                Deleted Departments
              </Nav.Link>
            </Nav.Item>
          </Nav>
        </Card.Header>
      </Card>

      {/* Tab Content */}
      <div className="tab-content">
        {activeTab === 'employees' && (
          <div className="tab-pane fade show active">
            <DeletedEmployees />
          </div>
        )}
        
        {activeTab === 'departments' && (
          <div className="tab-pane fade show active">
            <DeletedDepartments />
          </div>
        )}
      </div>
    </div>
  );
}

export default DeletedItems;