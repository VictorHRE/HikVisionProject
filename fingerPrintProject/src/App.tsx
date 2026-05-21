import React, { useEffect, type ReactNode } from 'react';
import { BrowserRouter, Routes, Route, Navigate, useLocation } from 'react-router-dom';
import './App.css';
import Sidebar from './components/Sidebar.tsx';
import LoginPage from './pages/Login.tsx';
import EmployeesPage from './pages/Employees.tsx';
import BranchesPage from './pages/Branches.tsx';
// Historial deshabilitado — import comentado para evitar mostrar la pestaña
// import HistoryPage from './pages/History.tsx';

// PrivateRoute reactivo a eventos de autenticación
function PrivateRoute({ children }: { children: ReactNode }) {
  const [token, setToken] = React.useState<string | null>(() => localStorage.getItem('fp_token'))

  // Escuchar eventos de login / expiración para actualizar estado
  useEffect(() => {
    const update = () => setToken(localStorage.getItem('fp_token'))
    window.addEventListener('auth:login', update)
    return () => {
      window.removeEventListener('auth:login', update)
    }
  }, [])

  if (!token) return <Navigate to="/login" replace />
  return <>{children}</>
}

// AuthEvents removed: redirection on expiry handled no longer by app.

function AppContent() {
  const location = useLocation();
  const showSidebar = location.pathname !== '/login';

  return (
    <div className={showSidebar ? 'app-layout' : ''}>
      {showSidebar && <Sidebar />}
      <main className="main-content">
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route
            path="/employees"
            element={<PrivateRoute><EmployeesPage /></PrivateRoute>}
          />
          <Route
            path="/branches"
            element={<PrivateRoute><BranchesPage /></PrivateRoute>}
          />
          {/* Historial deshabilitado: ruta comentada */}
          {/**
          <Route
            path="/history"
            element={<PrivateRoute><HistoryPage /></PrivateRoute>}
          />
          */}
          <Route path="/" element={<Navigate to="/employees" replace />} />
        </Routes>
      </main>
    </div>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      {/* AuthEvents removed */}
      <AppContent />
    </BrowserRouter>
  );
}
