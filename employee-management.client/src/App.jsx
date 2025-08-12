import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'react-datepicker/dist/react-datepicker.css';

import Navbar from './components/layout/Navbar';
import EmployeeList from './components/employee/EmployeeList';
import EmployeeForm from './components/employee/EmployeeForm';
import DepartmentList from './components/department/DepartmentList';
import DepartmentForm from './components/department/DepartmentForm';
import DeletedItems from './components/common/DeletedItems';
import { EmployeeProvider } from './context/EmployeeContext';
import { DepartmentProvider } from './context/DepartmentContext';

import './App.css';

/**
 * Main App component for the Employee Management system
 * @returns {JSX.Element} The main application component
 */
function App() {
  return (
    <EmployeeProvider>
      <DepartmentProvider>
        <Router>
          <div className="App">
            <Navbar />
            <main className="container mt-4">
              <Routes>
                <Route path="/" element={<EmployeeList />} />
                <Route path="/employees" element={<EmployeeList />} />
                <Route path="/employees/new" element={<EmployeeForm />} />
                <Route path="/employees/edit/:id" element={<EmployeeForm />} />
                <Route path="/departments" element={<DepartmentList />} />
                <Route path="/departments/new" element={<DepartmentForm />} />
                <Route path="/departments/edit/:id" element={<DepartmentForm />} />
                <Route path="/deleted-items" element={<DeletedItems />} />
              </Routes>
            </main>
            <ToastContainer
              position="top-right"
              autoClose={5000}
              hideProgressBar={false}
              newestOnTop={false}
              closeOnClick
              rtl={false}
              pauseOnFocusLoss
              draggable
              pauseOnHover
              theme="light"
            />
          </div>
        </Router>
      </DepartmentProvider>
    </EmployeeProvider>
  );
}

export default App;