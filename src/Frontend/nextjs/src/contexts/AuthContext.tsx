'use client';

import { createContext, useContext, useEffect, useState, useCallback, ReactNode } from 'react';
import { UserInfo } from '@/types/api';
import { getUser, isAuthenticated as checkAuth, initiateLogin, initiateLogout, hasRole } from '@/lib/auth';

interface AuthContextType {
  user: UserInfo | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: () => void;
  logout: () => void;
  hasRole: (role: string) => boolean;
  refreshUser: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserInfo | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const refreshUser = useCallback(() => {
    const u = getUser();
    setUser(u);
    setIsLoading(false);
  }, []);

  useEffect(() => {
    refreshUser();
    // Listen for storage changes (multi-tab support)
    const handler = () => refreshUser();
    window.addEventListener('storage', handler);
    return () => window.removeEventListener('storage', handler);
  }, [refreshUser]);

  const login = useCallback(() => initiateLogin(), []);
  const logout = useCallback(() => initiateLogout(), []);
  const checkRole = useCallback((role: string) => hasRole(role), []);

  return (
    <AuthContext.Provider value={{
      user,
      isAuthenticated: checkAuth(),
      isLoading,
      login,
      logout,
      hasRole: checkRole,
      refreshUser,
    }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
