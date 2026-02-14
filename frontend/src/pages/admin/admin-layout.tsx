import { NavLink, Outlet } from 'react-router-dom';
import './admin-layout.scss';

const navItems = [
  { text: 'Dashboard', path: '/admin', icon: 'dx-icon-home', end: true },
  { text: 'Forms', path: '/admin/forms', icon: 'dx-icon-fields', end: false },
  { text: 'Submissions', path: '/admin/submissions', icon: 'dx-icon-doc', end: false },
  { text: 'Personnel', path: '/admin/personnel', icon: 'dx-icon-group', end: false },
  { text: 'Battery Types', path: '/admin/battery-types', icon: 'dx-icon-product', end: false },
];

export function AdminLayout() {
  return (
    <div className="admin-layout">
      <aside className="admin-sidebar">
        <div className="admin-sidebar-header">Admin Panel</div>
        <nav className="admin-sidebar-nav">
          {navItems.map((item) => (
            <NavLink
              key={item.path}
              to={item.path}
              end={item.end}
              className={({ isActive }) => `nav-item${isActive ? ' active' : ''}`}
            >
              <i className={item.icon} />
              <span>{item.text}</span>
            </NavLink>
          ))}
        </nav>
        <div className="admin-sidebar-footer">
          <a href="/">← Back to App</a>
        </div>
      </aside>
      <main className="admin-content">
        <Outlet />
      </main>
    </div>
  );
}
