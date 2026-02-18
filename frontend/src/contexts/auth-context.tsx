import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';

// ── Types ──
export interface AuthUser {
    empId: string;
    fullName: string;
    npk: string;
    username: string;
    role: 'operator' | 'leader' | 'kasubsie' | 'kasie';
    jobName: string;
    department?: string;
    section?: string;
}

interface AuthContextType {
    user: AuthUser | null;
    token: string | null;
    isLoading: boolean;
    login: (username: string, password: string) => Promise<{ success: boolean; user?: AuthUser; error?: string }>;
    logout: () => void;
    isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

const API_BASE = '/api';

// ── Provider ──
export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<AuthUser | null>(null);
    const [token, setToken] = useState<string | null>(() => localStorage.getItem('auth_token'));
    const [isLoading, setIsLoading] = useState(true);

    // On mount — verify existing token
    useEffect(() => {
        if (token) {
            verifyToken(token)
                .then((u) => { if (u) setUser(u); else clearSession(); })
                .catch(() => clearSession())
                .finally(() => setIsLoading(false));
        } else {
            setIsLoading(false);
        }
    }, []);

    async function verifyToken(t: string): Promise<AuthUser | null> {
        try {
            const res = await fetch(`${API_BASE}/auth/verify`, {
                headers: { Authorization: `Bearer ${t}` },
            });
            if (!res.ok) return null;
            const data = await res.json();
            return data.success ? data.user : null;
        } catch {
            return null;
        }
    }

    async function login(username: string, password: string) {
        const res = await fetch(`${API_BASE}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password }),
        });

        const data = await res.json();

        if (data.success) {
            setUser(data.user);
            setToken(data.token);
            localStorage.setItem('auth_token', data.token);
            return { success: true, user: data.user as AuthUser };
        } else {
            return { success: false, error: data.message || 'Login gagal.' };
        }
    }

    function logout() {
        clearSession();
    }

    function clearSession() {
        setUser(null);
        setToken(null);
        localStorage.removeItem('auth_token');
    }

    return (
        <AuthContext.Provider value={{ user, token, isLoading, login, logout, isAuthenticated: !!user }}>
            {children}
        </AuthContext.Provider>
    );
}

// ── Hook ──
export function useAuth() {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error('useAuth must be used within AuthProvider');
    return ctx;
}
