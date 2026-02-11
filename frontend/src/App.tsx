import 'devextreme/dist/css/dx.common.css';
import './themes/generated/theme.base.dark.css';
import './themes/generated/theme.base.css';
import './themes/generated/theme.additional.dark.css';
import './themes/generated/theme.additional.css';
import './dx-styles.scss';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
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
import { ProfilePage } from './pages';

export default function Root() {
  const screenSizeClass = useScreenSizeClass();
  const themeContext = useThemeContext();

  return (
    <ThemeContext.Provider value={themeContext}>
      <BrowserRouter>
        <Routes>
          {/* Admin routes */}
          <Route path="/admin" element={<AdminLayout />}>
            <Route index element={<AdminDashboard />} />
            <Route path="forms" element={<FormList />} />
            <Route path="forms/:id" element={<FormEditor />} />
            <Route path="submissions" element={<SubmissionList />} />
            <Route path="submissions/:id" element={<SubmissionDetail />} />
            <Route path="submissions/:id/view" element={<SubmissionResponse />} />
            <Route path="personnel" element={<PersonnelPage />} />
            <Route path="battery-types" element={<BatteryTypesPage />} />
          </Route>
          {/* Main app */}
          <Route
            path="/"
            element={
              <div className={`app ${screenSizeClass}`}>
                <CosValidation />
              </div>
            }
          />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </ThemeContext.Provider>
  );
}
