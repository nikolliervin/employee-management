import React from 'react';
import PropTypes from 'prop-types';

/**
 * Loading spinner component with customizable size and text
 * @param {Object} props - Component props
 * @param {string} props.text - Loading text to display
 * @param {string} props.size - Spinner size (sm, default, lg)
 * @param {string} props.className - Additional CSS classes
 * @returns {JSX.Element} Loading spinner component
 */
function LoadingSpinner({ text = 'Loading...', size = 'default', className = '' }) {
  const sizeClass = size === 'sm' ? 'spinner-border-sm' : size === 'lg' ? '' : 'spinner-border-sm';
  
  return (
    <div className={`text-center py-4 ${className}`}>
      <div className={`spinner-border ${sizeClass} text-primary`} role="status">
        <span className="visually-hidden">{text}</span>
      </div>
      {text && (
        <div className="mt-2 text-muted">
          {text}
        </div>
      )}
    </div>
  );
}

LoadingSpinner.propTypes = {
  text: PropTypes.string,
  size: PropTypes.oneOf(['sm', 'default', 'lg']),
  className: PropTypes.string,
};

export default LoadingSpinner; 