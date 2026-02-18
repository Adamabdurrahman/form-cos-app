import { useState, type FormEvent } from 'react';
import './login.css';

const API_BASE = '/api';

interface ResetPasswordPageProps {
    onBack: () => void;
    onSuccess: (message: string) => void;
}

export function ResetPasswordPage({ onBack, onSuccess }: ResetPasswordPageProps) {
    // Step 1: validate NPK
    const [step, setStep] = useState<1 | 2>(1);
    const [npk, setNpk] = useState('');
    const [fullName, setFullName] = useState('');
    const [currentUsername, setCurrentUsername] = useState('');

    // Step 2: new credentials
    const [newUsername, setNewUsername] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');

    const [error, setError] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [showPassword, setShowPassword] = useState(false);

    async function handleValidateNpk(e: FormEvent) {
        e.preventDefault();
        setError('');
        setIsSubmitting(true);

        try {
            const res = await fetch(`${API_BASE}/auth/validate-npk`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ npk: npk.trim() }),
            });

            const data = await res.json();

            if (data.success) {
                setFullName(data.fullName ?? '');
                setCurrentUsername(data.currentUsername ?? '');
                setNewUsername(data.currentUsername ?? '');
                setStep(2);
            } else {
                setError(data.message || 'NPK tidak ditemukan atau belum punya akun.');
            }
        } catch {
            setError('Terjadi kesalahan koneksi. Coba lagi nanti.');
        } finally {
            setIsSubmitting(false);
        }
    }

    async function handleResetPassword(e: FormEvent) {
        e.preventDefault();
        setError('');

        if (newPassword.length < 6) {
            setError('Password baru minimal 6 karakter.');
            return;
        }
        if (newPassword !== confirmPassword) {
            setError('Password baru dan konfirmasi tidak cocok.');
            return;
        }

        setIsSubmitting(true);

        try {
            const res = await fetch(`${API_BASE}/auth/reset-password`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    npk: npk.trim(),
                    newUsername: newUsername.trim() !== currentUsername ? newUsername.trim() : null,
                    newPassword,
                    confirmNewPassword: confirmPassword,
                }),
            });

            const data = await res.json();

            if (data.success) {
                onSuccess(data.message || 'Password berhasil direset. Silakan login kembali.');
            } else {
                setError(data.message || 'Reset password gagal.');
            }
        } catch {
            setError('Terjadi kesalahan koneksi. Coba lagi nanti.');
        } finally {
            setIsSubmitting(false);
        }
    }

    return (
        <div className="login-container">
            <div className="login-bg">
                <div className="blob blob-1" />
                <div className="blob blob-2" />
                <div className="blob blob-3" />
            </div>

            <div className="login-card">
                <div className="login-header">
                    <div className="login-logo">
                        <svg viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <rect width="48" height="48" rx="12" fill="url(#grad-reset)" />
                            <path d="M14 24a10 10 0 0118-6" stroke="white" strokeWidth="2.5" strokeLinecap="round" />
                            <path d="M34 24a10 10 0 01-18 6" stroke="white" strokeWidth="2.5" strokeLinecap="round" />
                            <path d="M32 14l0 4h-4" stroke="white" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" />
                            <path d="M16 34l0-4h4" stroke="white" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" />
                            <defs>
                                <linearGradient id="grad-reset" x1="0" y1="0" x2="48" y2="48">
                                    <stop stopColor="#f59e0b" />
                                    <stop offset="1" stopColor="#d97706" />
                                </linearGradient>
                            </defs>
                        </svg>
                    </div>
                    <h1 className="login-title">Reset Password</h1>
                    <p className="login-subtitle">
                        {step === 1
                            ? 'Masukkan NPK untuk verifikasi identitas'
                            : `Halo, ${fullName}. Silakan atur ulang kredensial Anda.`}
                    </p>
                </div>

                {/* STEP 1: Verify NPK */}
                {step === 1 && (
                    <form className="login-form" onSubmit={handleValidateNpk}>
                        <div className="input-group">
                            <label htmlFor="reset-npk">NPK Karyawan</label>
                            <div className="input-wrapper">
                                <svg className="input-icon" viewBox="0 0 20 20" fill="currentColor">
                                    <path fillRule="evenodd" d="M4 4a2 2 0 012-2h4.586A2 2 0 0112 2.586L15.414 6A2 2 0 0116 7.414V16a2 2 0 01-2 2H6a2 2 0 01-2-2V4z" clipRule="evenodd" />
                                </svg>
                                <input
                                    id="reset-npk"
                                    type="text"
                                    placeholder="Masukkan NPK Anda..."
                                    value={npk}
                                    onChange={(e) => setNpk(e.target.value)}
                                    required
                                    autoFocus
                                />
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
                                'Verifikasi NPK'
                            )}
                        </button>

                        <button type="button" className="auth-back-btn" onClick={onBack}>
                            ← Kembali ke Login
                        </button>
                    </form>
                )}

                {/* STEP 2: New credentials */}
                {step === 2 && (
                    <form className="login-form" onSubmit={handleResetPassword}>
                        {/* Optionally change username */}
                        <div className="input-group">
                            <label htmlFor="reset-username">Username</label>
                            <div className="input-wrapper">
                                <svg className="input-icon" viewBox="0 0 20 20" fill="currentColor">
                                    <path fillRule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clipRule="evenodd" />
                                </svg>
                                <input
                                    id="reset-username"
                                    type="text"
                                    placeholder="Username baru (opsional)"
                                    value={newUsername}
                                    onChange={(e) => setNewUsername(e.target.value)}
                                    minLength={4}
                                />
                            </div>
                            <span className="input-hint">Kosongkan jika tidak ingin mengganti username</span>
                        </div>

                        {/* New password */}
                        <div className="input-group">
                            <label htmlFor="reset-new-pw">Password Baru</label>
                            <div className="input-wrapper">
                                <svg className="input-icon" viewBox="0 0 20 20" fill="currentColor">
                                    <path fillRule="evenodd" d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z" clipRule="evenodd" />
                                </svg>
                                <input
                                    id="reset-new-pw"
                                    type={showPassword ? 'text' : 'password'}
                                    placeholder="Minimal 6 karakter"
                                    value={newPassword}
                                    onChange={(e) => setNewPassword(e.target.value)}
                                    minLength={6}
                                    required
                                    autoFocus
                                />
                                <button type="button" className="toggle-password"
                                    onClick={() => setShowPassword(!showPassword)} tabIndex={-1}>
                                    {showPassword ? (
                                        <svg viewBox="0 0 20 20" fill="currentColor"><path fillRule="evenodd" d="M3.707 2.293a1 1 0 00-1.414 1.414l14 14a1 1 0 001.414-1.414l-1.473-1.473A10.014 10.014 0 0019.542 10C18.268 5.943 14.478 3 10 3a9.958 9.958 0 00-4.512 1.074l-1.78-1.781zm4.261 4.26l1.514 1.515a2.003 2.003 0 012.45 2.45l1.514 1.514a4 4 0 00-5.478-5.478z" clipRule="evenodd" /><path d="M12.454 16.697L9.75 13.992a4 4 0 01-3.742-3.741L2.335 6.578A9.98 9.98 0 00.458 10c1.274 4.057 5.065 7 9.542 7 .847 0 1.669-.105 2.454-.303z" /></svg>
                                    ) : (
                                        <svg viewBox="0 0 20 20" fill="currentColor"><path d="M10 12a2 2 0 100-4 2 2 0 000 4z" /><path fillRule="evenodd" d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z" clipRule="evenodd" /></svg>
                                    )}
                                </button>
                            </div>
                        </div>

                        {/* Confirm new password */}
                        <div className="input-group">
                            <label htmlFor="reset-confirm">Konfirmasi Password Baru</label>
                            <div className="input-wrapper">
                                <svg className="input-icon" viewBox="0 0 20 20" fill="currentColor">
                                    <path fillRule="evenodd" d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z" clipRule="evenodd" />
                                </svg>
                                <input
                                    id="reset-confirm"
                                    type={showPassword ? 'text' : 'password'}
                                    placeholder="Ulangi password baru"
                                    value={confirmPassword}
                                    onChange={(e) => setConfirmPassword(e.target.value)}
                                    required
                                />
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

                        <button type="submit" className="login-btn reset-submit-btn" disabled={isSubmitting}>
                            {isSubmitting ? (
                                <>
                                    <span className="spinner" />
                                    Mereset...
                                </>
                            ) : (
                                'Reset Password'
                            )}
                        </button>

                        <button type="button" className="auth-back-btn" onClick={() => { setStep(1); setError(''); }}>
                            ← Kembali ke Verifikasi NPK
                        </button>
                    </form>
                )}
            </div>
        </div>
    );
}
