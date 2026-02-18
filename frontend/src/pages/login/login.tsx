import { useState, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/auth-context';
import './login.css';

export function LoginPage() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [showPassword, setShowPassword] = useState(false);
    const [toast, setToast] = useState<{ show: boolean; message: string; type: 'success' | 'error' }>({
        show: false,
        message: '',
        type: 'success',
    });

    const { login } = useAuth();
    const navigate = useNavigate();

    async function handleSubmit(e: FormEvent) {
        e.preventDefault();
        setError('');
        setIsSubmitting(true);

        try {
            const result = await login(username, password);

            if (result.success && result.user) {
                const user = result.user;
                const roleName =
                    user.role === 'operator' ? 'Operator' :
                        user.role === 'leader' ? 'Leader' :
                            user.role === 'kasubsie' ? 'Kasubsie' :
                                user.role === 'kasie' ? 'Kasie' : user.jobName;

                // Show success toast
                setToast({
                    show: true,
                    message: `Selamat datang, ${user.fullName} (${roleName})`,
                    type: 'success',
                });

                // Redirect after short delay for toast visibility
                setTimeout(() => {
                    if (user.role === 'operator') {
                        navigate('/', { replace: true });
                    } else {
                        navigate('/admin', { replace: true });
                    }
                }, 1500);
            } else {
                setError(result.error || 'Login gagal.');
                setToast({ show: true, message: result.error || 'Login gagal.', type: 'error' });
                setTimeout(() => setToast((t) => ({ ...t, show: false })), 4000);
            }
        } catch (err) {
            setError('Terjadi kesalahan. Coba lagi nanti.');
            setToast({ show: true, message: 'Terjadi kesalahan koneksi.', type: 'error' });
            setTimeout(() => setToast((t) => ({ ...t, show: false })), 4000);
        } finally {
            setIsSubmitting(false);
        }
    }

    return (
        <div className="login-container">
            {/* Background animated blobs */}
            <div className="login-bg">
                <div className="blob blob-1" />
                <div className="blob blob-2" />
                <div className="blob blob-3" />
            </div>

            {/* Toast notification */}
            <div className={`login-toast ${toast.show ? 'show' : ''} ${toast.type}`}>
                <span className="toast-icon">{toast.type === 'success' ? '✅' : '❌'}</span>
                <span className="toast-msg">{toast.message}</span>
            </div>

            {/* Login card */}
            <div className="login-card">
                <div className="login-header">
                    <div className="login-logo">
                        <svg viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <rect width="48" height="48" rx="12" fill="url(#grad)" />
                            <path d="M14 24L21 31L34 18" stroke="white" strokeWidth="3.5" strokeLinecap="round" strokeLinejoin="round" />
                            <defs>
                                <linearGradient id="grad" x1="0" y1="0" x2="48" y2="48">
                                    <stop stopColor="#6366f1" />
                                    <stop offset="1" stopColor="#8b5cf6" />
                                </linearGradient>
                            </defs>
                        </svg>
                    </div>
                    <h1 className="login-title">COS Checksheet</h1>
                    <p className="login-subtitle">Masuk untuk melanjutkan ke sistem</p>
                </div>

                <form className="login-form" onSubmit={handleSubmit}>
                    <div className="input-group">
                        <label htmlFor="login-username">Nama Lengkap</label>
                        <div className="input-wrapper">
                            <svg className="input-icon" viewBox="0 0 20 20" fill="currentColor">
                                <path fillRule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clipRule="evenodd" />
                            </svg>
                            <input
                                id="login-username"
                                type="text"
                                placeholder="Masukkan nama lengkap..."
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                                autoComplete="username"
                                autoFocus
                                required
                            />
                        </div>
                    </div>

                    <div className="input-group">
                        <label htmlFor="login-password">NPK (Password)</label>
                        <div className="input-wrapper">
                            <svg className="input-icon" viewBox="0 0 20 20" fill="currentColor">
                                <path fillRule="evenodd" d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z" clipRule="evenodd" />
                            </svg>
                            <input
                                id="login-password"
                                type={showPassword ? 'text' : 'password'}
                                placeholder="Masukkan NPK..."
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                autoComplete="current-password"
                                required
                            />
                            <button
                                type="button"
                                className="toggle-password"
                                onClick={() => setShowPassword(!showPassword)}
                                tabIndex={-1}
                            >
                                {showPassword ? (
                                    <svg viewBox="0 0 20 20" fill="currentColor"><path fillRule="evenodd" d="M3.707 2.293a1 1 0 00-1.414 1.414l14 14a1 1 0 001.414-1.414l-1.473-1.473A10.014 10.014 0 0019.542 10C18.268 5.943 14.478 3 10 3a9.958 9.958 0 00-4.512 1.074l-1.78-1.781zm4.261 4.26l1.514 1.515a2.003 2.003 0 012.45 2.45l1.514 1.514a4 4 0 00-5.478-5.478z" clipRule="evenodd" /><path d="M12.454 16.697L9.75 13.992a4 4 0 01-3.742-3.741L2.335 6.578A9.98 9.98 0 00.458 10c1.274 4.057 5.065 7 9.542 7 .847 0 1.669-.105 2.454-.303z" /></svg>
                                ) : (
                                    <svg viewBox="0 0 20 20" fill="currentColor"><path d="M10 12a2 2 0 100-4 2 2 0 000 4z" /><path fillRule="evenodd" d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z" clipRule="evenodd" /></svg>
                                )}
                            </button>
                        </div>
                    </div>

                    {error && (
                        <div className="login-error">
                            <svg viewBox="0 0 20 20" fill="currentColor">
                                <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                            </svg>
                            {error}
                        </div>
                    )}

                    <button type="submit" className="login-btn" disabled={isSubmitting}>
                        {isSubmitting ? (
                            <>
                                <span className="spinner" />
                                Memverifikasi...
                            </>
                        ) : (
                            'Masuk'
                        )}
                    </button>
                </form>

                <div className="login-footer">
                    <p>© 2026 PT GS Battery — COS Checksheet System</p>
                </div>
            </div>
        </div>
    );
}
