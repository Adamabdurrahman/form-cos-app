import { useState } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/auth-context';
import './admin-layout.scss';

const navItems = [
  { text: 'Dashboard', path: '/admin', icon: 'dx-icon-home', end: true },
  { text: 'Forms', path: '/admin/forms', icon: 'dx-icon-fields', end: false },
  { text: 'Submissions', path: '/admin/submissions', icon: 'dx-icon-doc', end: false },
  { text: 'Personnel', path: '/admin/personnel', icon: 'dx-icon-group', end: false },
  { text: 'Battery Types', path: '/admin/battery-types', icon: 'dx-icon-product', end: false },
];

export function AdminLayout() {
  const [collapsed, setCollapsed] = useState(false);
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate('/login', { replace: true });
  }

  return (
    <div className={`admin-layout${collapsed ? ' sidebar-collapsed' : ''}`}>
      <aside className="admin-sidebar">
        <div className="admin-sidebar-header">
          {!collapsed && <span>Admin Panel</span>}
          <button
            className="sidebar-toggle-btn"
            onClick={() => setCollapsed((c) => !c)}
            title={collapsed ? 'Expand sidebar' : 'Collapse sidebar'}
            type="button"
          >
            <i className={collapsed ? 'dx-icon-chevronnext' : 'dx-icon-chevronprev'} />
          </button>
        </div>
        <nav className="admin-sidebar-nav">
          {navItems.map((item) => (
            <NavLink
              key={item.path}
              to={item.path}
              end={item.end}
              className={({ isActive }) => `nav-item${isActive ? ' active' : ''}`}
              title={collapsed ? item.text : undefined}
            >
              <i className={item.icon} />
              {!collapsed && <span>{item.text}</span>}
            </NavLink>
          ))}
        </nav>

        {/* ── User Info & Logout ── */}
        <div className="admin-sidebar-footer">
          {!collapsed && user && (
            <div className="sidebar-user-info">
              <div className="sidebar-user-name" title={user.fullName}>
                {user.fullName}
              </div>
              <div className="sidebar-user-role">
                {user.role.charAt(0).toUpperCase() + user.role.slice(1)}
              </div>
            </div>
          )}
          <div className="sidebar-footer-actions">
            <a href="/" className="sidebar-back-link" title="Back to Form">
              <i className="dx-icon-arrowleft" />
              {!collapsed && <span>Back to Form</span>}
            </a>
            <button
              type="button"
              className="sidebar-logout-btn"
              onClick={handleLogout}
              title="Logout"
            >
              <i className="dx-icon-runner" />
              {!collapsed && <span>Logout</span>}
            </button>
          </div>
        </div>
      </aside>
      <main className="admin-content">
        <Outlet />
      </main>
    </div>
  );
}
