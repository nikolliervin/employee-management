import React, { useState, useEffect, useRef, useCallback } from 'react';
import PropTypes from 'prop-types';
import { useNavigate, useParams } from 'react-router-dom';
import { Form, Button, Row, Col, Card, Alert } from 'react-bootstrap';
import { Formik } from 'formik';
import * as Yup from 'yup';
import { toast } from 'react-toastify';
import { useDepartmentContext } from '../../context/DepartmentContext';
import { LOADING_STATES } from '../../types';
import LoadingSpinner from '../common/LoadingSpinner';

/**
 * Department form component for creating and editing departments
 * @returns {JSX.Element} Department form component
 */
function DepartmentForm() {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEditMode = Boolean(id);
  
  const {
    createDepartment,
    updateDepartment,
    fetchDepartmentById,
    loadingState,
  } = useDepartmentContext();

  const [department, setDepartment] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isFetching, setIsFetching] = useState(false);

  // Fetch department data for edit mode
  useEffect(() => {
    const fetchDepartmentData = async () => {
      if (isEditMode && id) {
        try {
          setIsFetching(true);
          const departmentData = await fetchDepartmentById(id);
          setDepartment(departmentData);
        } catch (error) {
          console.error('Error fetching department data:', error);
          toast.error('Failed to load department data');
          navigate('/departments');
        } finally {
          setIsFetching(false);
        }
      }
    };

    fetchDepartmentData();
  }, [id, isEditMode, fetchDepartmentById, navigate]);

  // Validation schema
  const validationSchema = Yup.object({
    name: Yup.string()
      .required('Department name is required')
      .min(2, 'Name must be at least 2 characters')
      .max(100, 'Name cannot exceed 100 characters'),
    description: Yup.string()
      .max(500, 'Description cannot exceed 500 characters'),
  });

  // Initial values
  const initialValues = {
    name: department?.name || department?.Name || '',
    description: department?.description || department?.Description || '',
  };

  /**
   * Handle form submission
   * @param {Object} values - Form values
   * @param {Object} actions - Formik actions
   */
  const handleSubmit = async (values, actions) => {
    try {
      setIsLoading(true);
      
      const departmentData = {
        Name: values.name.trim(),
        Description: values.description ? values.description.trim() : null,
      };

      if (isEditMode) {
        await updateDepartment(id, departmentData);
      } else {
        await createDepartment(departmentData);
      }
      
      navigate('/departments');
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
    navigate('/departments');
  };

  // Show loading spinner while fetching department data
  if (isFetching || (isEditMode && !department && !loadingState)) {
    return <LoadingSpinner text="Loading department data..." />;
  }

  return (
    <div className="container">
      {/* Modern Header */}
      <div className="mb-4">
        <h1 className="h2 mb-2 fw-bold" style={{ color: 'var(--text-primary)' }}>
          {isEditMode ? 'Edit Department' : 'Add New Department'}
        </h1>
        <p className="text-secondary mb-0 fs-6">
          {isEditMode ? 'Update department information' : 'Fill in the details to add a new department'}
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
                  isSubmitting,
                  isValid,
                }) => (
                  <Form onSubmit={handleSubmit} className="p-4">
                    <Row className="g-4">
                      <Col md={12}>
                        <Form.Group className="mb-0">
                          <Form.Label htmlFor="name" className="form-label mb-2 fw-semibold">
                            Department Name *
                          </Form.Label>
                          <Form.Control
                            id="name"
                            type="text"
                            name="name"
                            value={values.name}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            isInvalid={touched.name && errors.name}
                            placeholder="Enter department name"
                            disabled={isLoading}
                            style={{ height: '44px' }}
                          />
                          <Form.Control.Feedback type="invalid">
                            {errors.name}
                          </Form.Control.Feedback>
                        </Form.Group>
                      </Col>
                    </Row>

                    <Row className="g-4">
                      <Col md={12}>
                        <Form.Group className="mb-0">
                          <Form.Label htmlFor="description" className="form-label mb-2 fw-semibold">
                            Description
                          </Form.Label>
                          <Form.Control
                            id="description"
                            as="textarea"
                            rows={4}
                            name="description"
                            value={values.description}
                            onChange={handleChange}
                            onBlur={handleBlur}
                            isInvalid={touched.description && errors.description}
                            placeholder="Enter department description (optional)"
                            disabled={isLoading}
                          />
                          <Form.Control.Feedback type="invalid">
                            {errors.description}
                          </Form.Control.Feedback>
                        </Form.Group>
                      </Col>
                    </Row>

                    {/* Form Actions */}
                    <hr className="my-4" />
                    <div className="d-flex justify-content-end gap-3">
                      <Button
                        variant="outline-secondary"
                        onClick={handleCancel}
                        disabled={isLoading || isSubmitting}
                        className="px-4"
                      >
                        <i className="bi bi-x-circle me-2"></i>
                        Cancel
                      </Button>
                      
                      <Button
                        type="submit"
                        variant="primary"
                        disabled={!isValid || isLoading || isSubmitting}
                        className="px-4"
                      >
                        {isLoading || isSubmitting ? (
                          <>
                            <div className="spinner-border spinner-border-sm me-2" role="status">
                              <span className="visually-hidden">
                                {isEditMode ? 'Updating...' : 'Creating...'}
                              </span>
                            </div>
                            {isEditMode ? 'Updating...' : 'Creating...'}
                          </>
                        ) : (
                          <>
                            <i className={`bi ${isEditMode ? 'bi-check-circle' : 'bi-plus-circle'} me-2`}></i>
                            {isEditMode ? 'Update Department' : 'Create Department'}
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

DepartmentForm.propTypes = {};

export default DepartmentForm;