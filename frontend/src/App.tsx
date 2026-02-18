import 'devextreme/dist/css/dx.common.css';
import './themes/generated/theme.base.dark.css';
import './themes/generated/theme.base.css';
import './themes/generated/theme.additional.dark.css';
import './themes/generated/theme.additional.css';
import './dx-styles.scss';
import { createBrowserRouter, RouterProvider, Navigate, Outlet } from 'react-router-dom';
import { useScreenSizeClass } from './utils/media-query';
import { ThemeContext, useThemeContext } from './theme';
import { CosValidation } from './pages/cos-validation/cos-validation';
import { AdminLayout } from './pages/admin/admin-layout';
import { AdminDashboard } from './pages/admin/dashboard';
import { FormList } from './pages/admin/form-list';
import { FormEditor } from './pages/admin/form-editor';
import { SubmissionList } from './pages/admin/submission-list';
import { SubmissionDetail } from './pages/admin/submission-detail';
import { SubmissionResponse } from './pages/admin/submission-response';
import { PersonnelPage } from './pages/admin/personnel';
import { BatteryTypesPage } from './pages/admin/battery-types';
import { LoginPage } from './pages/login/login';
import { AuthProvider, useAuth } from './contexts/auth-context';

// ── Operator page wrapper ──
function CosPage() {
  const screenSizeClass = useScreenSizeClass();
  return (
    <div className={`app ${screenSizeClass}`}>
      <CosValidation />
    </div>
  );
}

// ── Root Layout: provides AuthProvider context to all routes ──
function RootLayout() {
  return (
    <AuthProvider>
      <Outlet />
    </AuthProvider>
  );
}

// ── Auth Guard: redirects unauthenticated users to /login ──
function RequireAuth({ allowedRoles }: { allowedRoles?: string[] }) {
  const { isAuthenticated, isLoading, user } = useAuth();

  if (isLoading) {
    return (
      <div style={{
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        height: '100vh', background: '#0c0e14', color: '#94a3b8',
        fontFamily: 'Inter, sans-serif', fontSize: 14,
      }}>
        Memuat sesi...
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  // Role check
  if (allowedRoles && user && !allowedRoles.includes(user.role)) {
    if (user.role === 'operator') return <Navigate to="/" replace />;
    return <Navigate to="/admin" replace />;
  }

  return <Outlet />;
}

// ── Redirect if already logged in ──
function LoginGuard() {
  const { isAuthenticated, isLoading, user } = useAuth();

  if (isLoading) return null;

  if (isAuthenticated && user) {
    if (user.role === 'operator') return <Navigate to="/" replace />;
    return <Navigate to="/admin" replace />;
  }

  return <LoginPage />;
}

// ── Data Router (createBrowserRouter) — required for useBlocker ──
const router = createBrowserRouter([
  {
    // Root layout: wraps everything in AuthProvider
    element: <RootLayout />,
    children: [
      // Public: Login
      { path: '/login', element: <LoginGuard /> },

      // Protected: Admin routes (all authenticated users)
      {
        element: <RequireAuth />,
        children: [
          {
            path: '/admin',
            element: <AdminLayout />,
            children: [
              { index: true, element: <AdminDashboard /> },
              { path: 'forms', element: <FormList /> },
              { path: 'forms/:id', element: <FormEditor /> },
              { path: 'submissions', element: <SubmissionList /> },
              { path: 'submissions/:id', element: <SubmissionDetail /> },
              { path: 'submissions/:id/view', element: <SubmissionResponse /> },
              { path: 'personnel', element: <PersonnelPage /> },
              { path: 'battery-types', element: <BatteryTypesPage /> },
            ],
          },
        ],
      },

      // Protected: Operator route (all authenticated users can access)
      {
        element: <RequireAuth />,
        children: [
          { path: '/', element: <CosPage /> },
        ],
      },

      // Catch-all → redirect to login
      { path: '*', element: <Navigate to="/login" replace /> },
    ],
  },
]);

export default function Root() {
  const themeContext = useThemeContext();

  return (
    <ThemeContext.Provider value={themeContext}>
      <RouterProvider router={router} />
    </ThemeContext.Provider>
  );
}
