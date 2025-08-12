import React from 'react';
import ReactDOM from 'react-dom/client';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import 'bootstrap-icons/font/bootstrap-icons.css';
import './index.css';
import App from './App';

ReactDOM.createRoot(document.getElementById('root')).render(
  // Temporarily disable StrictMode to test infinite request issue
  // <React.StrictMode>
    <App />
  // </React.StrictMode>,
);
