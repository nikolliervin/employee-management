import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import PropTypes from 'prop-types';

/**
 * Navigation bar component for the Employee Management system
 * @returns {JSX.Element} Navigation bar component
 */
function Navbar() {
  const location = useLocation();

  /**
   * Check if a navigation item is active
   * @param {string} path - Path to check
   * @returns {boolean} True if path is active
   */
  const isActive = (path) => {
    if (path === '/') {
      return location.pathname === '/' || location.pathname === '/employees';
    }
    return location.pathname.startsWith(path);
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-primary shadow-sm">
      <div className="container">
        <Link className="navbar-brand fw-bold" to="/">
          Employee Management
        </Link>
        
        <button
          className="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#navbarNav"
          aria-controls="navbarNav"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>
        
        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav me-auto">
            <li className="nav-item">
              <Link
                className={`nav-link ${isActive('/employees') ? 'active' : ''}`}
                to="/employees"
              >
                Employees
              </Link>
            </li>
            <li className="nav-item">
              <Link
                className={`nav-link ${isActive('/departments') ? 'active' : ''}`}
                to="/departments"
              >
                Departments
              </Link>
            </li>
            <li className="nav-item">
              <Link
                className={`nav-link ${isActive('/deleted-items') ? 'active' : ''}`}
                to="/deleted-items"
              >
                Deleted Items
              </Link>
            </li>
          </ul>
          

        </div>
      </div>
    </nav>
  );
}

Navbar.propTypes = {};

export default Navbar; 