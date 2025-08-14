import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { useNavigate, useParams } from 'react-router-dom';
import { Form, Button, Row, Col, Card, Alert } from 'react-bootstrap';
import { Formik } from 'formik';
import * as Yup from 'yup';
import DatePicker from 'react-datepicker';
import Select from 'react-select';
import { toast } from 'react-toastify';
import { useEmployeeContext } from '../../context/EmployeeContext';
import { EMPLOYEE_VALIDATION_SCHEMA } from '../../types';
import LoadingSpinner from '../common/LoadingSpinner';

/**
 * Employee form component for creating and editing employees
 * @returns {JSX.Element} Employee form component
 */
function EmployeeForm() {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEditMode = Boolean(id);
  
  const {
    createEmployee,
    updateEmployee,
    fetchEmployeeById,
    fetchAllDepartments,
    departments,
    loadingState,
  } = useEmployeeContext();

  const [employee, setEmployee] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isFetching, setIsFetching] = useState(false);

  // Fetch employee data for edit mode
  useEffect(() => {
    const fetchEmployeeData = async () => {
      if (isEditMode && id) {
        try {
          setIsFetching(true);
          const employeeData = await fetchEmployeeById(id);
          setEmployee(employeeData);
        } catch (error) {
          console.error('Error fetching employee data:', error);
          toast.error('Failed to load employee data');
          navigate('/employees');
        } finally {
          setIsFetching(false);
        }
      }
    };

    fetchEmployeeData();
  }, [id, isEditMode, fetchEmployeeById, navigate]);

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

  // Validation schema
  const validationSchema = Yup.object({
    name: Yup.string()
      .required(EMPLOYEE_VALIDATION_SCHEMA.name.required)
      .min(2, EMPLOYEE_VALIDATION_SCHEMA.name.minLength.message)
      .max(100, EMPLOYEE_VALIDATION_SCHEMA.name.maxLength.message),
    email: Yup.string()
      .required(EMPLOYEE_VALIDATION_SCHEMA.email.required)
      .email(EMPLOYEE_VALIDATION_SCHEMA.email.pattern.message),
    dateOfBirth: Yup.date()
      .required(EMPLOYEE_VALIDATION_SCHEMA.dateOfBirth.required)
      .max(new Date(), 'Date of birth cannot be in the future')
      .test('age', 'Employee must be between 18 and 100 years old', function(value) {
        if (!value) return false;
        const today = new Date();
        const birthDate = new Date(value);
        const age = today.getFullYear() - birthDate.getFullYear();
        const monthDiff = today.getMonth() - birthDate.getMonth();
        if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
          age--;
        }
        return age >= 18 && age <= 100;
      }),
    departmentId: Yup.string()
      .required(EMPLOYEE_VALIDATION_SCHEMA.departmentId.required),
  });

  // Department options for react-select
  const departmentOptions = departments.map(dept => ({
    value: dept.id,
    label: dept.name,
  }));

  // Custom validation function for date of birth
  const validateDateOfBirth = (date) => {
    if (!date) return 'Date of birth is required';
    
    const today = new Date();
    if (date > today) return 'Date of birth cannot be in the future';
    
    const age = today.getFullYear() - date.getFullYear();
    const monthDiff = today.getMonth() - date.getMonth();
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < date.getDate())) {
      age--;
    }
    
    if (age < 18) return 'Employee must be at least 18 years old';
    if (age > 100) return 'Employee cannot be older than 100 years';
    
    return null; // No error
  };

  // Initial values
  const initialValues = {
    name: employee?.name || employee?.Name || '',
    email: employee?.email || employee?.Email || '',
    dateOfBirth: employee?.dateOfBirth || employee?.DateOfBirth ? new Date(employee.dateOfBirth || employee.DateOfBirth) : null,
    departmentId: (employee?.departmentId || employee?.DepartmentId || '').toUpperCase(),
  };

  /**
   * Handle form submission
   * @param {Object} values - Form values
   * @param {Object} actions - Formik actions
   */
  const handleSubmit = async (values, actions) => {
    try {
      setIsLoading(true);
      
      const employeeData = {
        Name: values.name.trim(),
        Email: values.email.trim().toLowerCase(),
        DateOfBirth: values.dateOfBirth.toISOString(),
        DepartmentId: values.departmentId,
      };

      if (isEditMode) {
        await updateEmployee(id, employeeData);
      } else {
        await createEmployee(employeeData);
      }
      
      navigate('/employees');
    } catch (error) {
      toast.error(error.message || 'An error occurred');
    } finally {
      setIsLoading(false);
      actions.setSubmitting(false);
    }
  };

  /**
   * Handle form cancellation
   */
  const handleCancel = () => {
    navigate('/employees');
  };

  // Show loading spinner while fetching employee data
  if (isFetching || (isEditMode && !employee && !loadingState)) {
    return <LoadingSpinner text="Loading employee data..." />;
  }

  return (
    <div className="container">
      {/* Modern Header */}
      <div className="mb-4">
        <h1 className="h2 mb-2 fw-bold" style={{ color: 'var(--text-primary)' }}>
          {isEditMode ? 'Edit Employee' : 'Add New Employee'}
        </h1>
        <p className="text-secondary mb-0 fs-6">
          {isEditMode ? 'Update employee information' : 'Fill in the details to add a new team member'}
        </p>
      </div>

      <div className="row justify-content-center">
        <div className="col-lg-8">
          <Card className="shadow-md border-0">
            <Card.Body className="p-0">
              <Formik
                initialValues={initialValues}
                validationSchema={validationSchema}
                onSubmit={handleSubmit}
                enableReinitialize
              >
                {({
                  values,
                  errors,
                  touched,
                  handleChange,
                  handleBlur,
                  handleSubmit,
                  setFieldValue,
                  setFieldError,
                  clearFieldError,
                  setFieldTouched,
                  isSubmitting,
                  isValid,
                }) => (
                  <Form onSubmit={handleSubmit} className="p-4">
                    <Row className="g-4">
                      <Col md={6}>
                        <Form.Group className="mb-0">
                          <Form.Label htmlFor="name" className="form-label mb-2 fw-semibold">
                            Full Name *
                          </Form.Label>
                          <Form.Control
                            id="name"
                            type="text"
                            name="name"
                            value={values.name}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            isInvalid={touched.name && errors.name}
                            placeholder="Enter full name"
                            disabled={isLoading}
                            style={{ height: '44px' }}
                          />
                          <Form.Control.Feedback type="invalid">
                            {errors.name}
                          </Form.Control.Feedback>
                        </Form.Group>
                      </Col>
                      
                      <Col md={6}>
                        <Form.Group className="mb-0">
                          <Form.Label htmlFor="email" className="form-label mb-2 fw-semibold">
                            Email Address *
                          </Form.Label>
                          <Form.Control
                            id="email"
                            type="email"
                            name="email"
                            value={values.email}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            isInvalid={touched.email && errors.email}
                            placeholder="Enter email address"
                            disabled={isLoading}
                            style={{ height: '44px' }}
                          />
                          <Form.Control.Feedback type="invalid">
                            {errors.email}
                          </Form.Control.Feedback>
                        </Form.Group>
                      </Col>
                    </Row>

                    <Row className="g-4">
                      <Col md={6}>
                        <Form.Group className="mb-0">
                          <Form.Label htmlFor="dateOfBirth" className="form-label mb-2 fw-semibold">
                            Date of Birth *
                          </Form.Label>
                          <DatePicker
                            selected={values.dateOfBirth}
                            onChange={(date) => {
                              setFieldValue('dateOfBirth', date);
                              // Trigger validation immediately when date changes
                              if (date) {
                                setFieldTouched('dateOfBirth', true, false);
                                // Run custom validation immediately
                                const validationError = validateDateOfBirth(date);
                                if (validationError) {
                                  setFieldError('dateOfBirth', validationError);
                                } else {
                                  clearFieldError('dateOfBirth');
                                }
                              } else {
                                // Clear error when date is cleared
                                clearFieldError('dateOfBirth');
                              }
                            }}
                            onBlur={() => setFieldTouched('dateOfBirth', true, false)}
                            showYearDropdown
                            scrollableYearDropdown
                            yearDropdownItemNumber={100}
                            maxDate={new Date()}
                            dateFormat="MM/dd/yyyy"
                            placeholderText="Select date of birth"
                            disabled={isLoading}
                            className={`form-control ${errors.dateOfBirth ? 'is-invalid' : ''}`}
                            style={{ 
                              height: '44px',
                              borderColor: errors.dateOfBirth ? '#dc3545' : undefined
                            }}
                          />
                          {errors.dateOfBirth && (
                            <div className="invalid-feedback d-block">
                              <i className="bi bi-x-circle me-1"></i>
                              {errors.dateOfBirth}
                            </div>
                          )}
                        </Form.Group>
                      </Col>
                      
                      <Col md={6}>
                        <Form.Group className="mb-0">
                          <Form.Label htmlFor="departmentId" className="form-label mb-2 fw-semibold">
                            Department *
                          </Form.Label>
                          <Select
                            id="departmentId"
                            value={departmentOptions.find(option => option.value === values.departmentId)}
                            onChange={(option) => setFieldValue('departmentId', option?.value || '')}
                            options={departmentOptions}
                            isDisabled={isLoading}
                            isClearable={false}
                            className="react-select-container"
                            classNamePrefix="react-select"
                            placeholder="Select department..."
                            isInvalid={touched.departmentId && errors.departmentId}
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
                          {touched.departmentId && errors.departmentId && (
                            <div className="invalid-feedback d-block">
                              {errors.departmentId}
                            </div>
                          )}
                        </Form.Group>
                      </Col>
                    </Row>

                    <div className="d-flex justify-content-end gap-3 mt-5 pt-4 border-top" style={{ borderColor: 'var(--border-light) !important' }}>
                      <Button
                        type="button"
                        variant="outline-secondary"
                        onClick={handleCancel}
                        disabled={isLoading || isSubmitting}
                        style={{ height: '44px' }}
                        className="px-4"
                      >
                        Cancel
                      </Button>
                      
                      <Button
                        type="submit"
                        variant="primary"
                        disabled={isLoading || isSubmitting || !isValid}
                        style={{ height: '44px' }}
                        className="px-4"
                      >
                        {isLoading || isSubmitting ? (
                          <>
                            <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                            {isEditMode ? 'Updating...' : 'Creating...'}
                          </>
                        ) : (
                          <>
                            <i className={`bi ${isEditMode ? 'bi-check-circle' : 'bi-plus-circle'} me-2`}></i>
                            {isEditMode ? 'Update Employee' : 'Create Employee'}
                          </>
                        )}
                      </Button>
                    </div>
                  </Form>
                )}
              </Formik>
            </Card.Body>
          </Card>
        </div>
      </div>
    </div>
  );
}

EmployeeForm.propTypes = {};

export default EmployeeForm; 