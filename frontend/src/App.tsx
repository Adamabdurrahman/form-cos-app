import 'devextreme/dist/css/dx.common.css';
import './themes/generated/theme.base.dark.css';
import './themes/generated/theme.base.css';
import './themes/generated/theme.additional.dark.css';
import './themes/generated/theme.additional.css';
import './dx-styles.scss';
import { useScreenSizeClass } from './utils/media-query';
import { ThemeContext, useThemeContext} from "./theme";
import { CosValidation } from './pages/cos-validation/cos-validation';

export default function Root() {
  const screenSizeClass = useScreenSizeClass();
  const themeContext = useThemeContext();

  return (
    <ThemeContext.Provider value={themeContext}>
      <div className={`app ${screenSizeClass}`}>
        <CosValidation />
      </div>
    </ThemeContext.Provider>
  );
}
