import 'devextreme/dist/css/dx.common.css';
import './themes/generated/theme.base.dark.css';
import './themes/generated/theme.base.css';
import './themes/generated/theme.additional.dark.css';
import './themes/generated/theme.additional.css';
import './dx-styles.scss';
import { createBrowserRouter, RouterProvider, Navigate } from 'react-router-dom';
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

function CosPage() {
  const screenSizeClass = useScreenSizeClass();
  return (
    <div className={`app ${screenSizeClass}`}>
      <CosValidation />
    </div>
  );
}

const router = createBrowserRouter([
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
  { path: '/', element: <CosPage /> },
  { path: '*', element: <Navigate to="/" replace /> },
]);

export default function Root() {
  const themeContext = useThemeContext();

  return (
    <ThemeContext.Provider value={themeContext}>
      <RouterProvider router={router} />
    </ThemeContext.Provider>
  );
}
