import { NavLink, useNavigate } from 'react-router-dom';
import { FiUsers, FiGitBranch, FiClock, FiLogOut } from 'react-icons/fi';
import { MdFingerprint } from 'react-icons/md';
import './Sidebar.css';

export default function Sidebar() {
  const navigate = useNavigate();

  function handleLogout() {
    // Eliminar token de autenticación
    localStorage.removeItem('fp_token');
    navigate('/login');
  }

  return (
    <aside className="sidebar">
      <div className="sidebar-brand">
        <div className="brand-icon">
          <MdFingerprint />
        </div>
        <h1 className="brand-text">FingerPrint</h1>
      </div>
      <nav className="sidebar-nav">
        <NavLink to="/employees" className="nav-item">
          <FiUsers />
          <span>Gestión Empleados</span>
        </NavLink>
        <NavLink to="/branches" className="nav-item">
          <FiGitBranch />
          <span>Gestión de Sucursales</span>
        </NavLink>
        {/* Historial deshabilitado: enlace comentado */}
        {/**
        <NavLink to="/history" className="nav-item">
          <FiClock />
          <span>Historial E/S</span>
        </NavLink>
        */}
      </nav>
      <div className="sidebar-footer">
        <button onClick={handleLogout} className="nav-item logout-btn">
          <FiLogOut />
          <span>Cerrar sesión</span>
        </button>
      </div>
    </aside>
  );
}
