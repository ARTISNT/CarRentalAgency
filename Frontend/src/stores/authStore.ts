import { create } from 'zustand';
import type { UserRole } from '../types';

interface DecodedToken {
  sub?: string;
  nameid?: string;
  email?: string;
  role?: string;
  permissions?: string[];
}

function decodeToken(token: string): { nameid: string; email: string; role: UserRole; permissions: string[] } | null {
  try {
    const payload = token.split('.')[1];
    const decoded: DecodedToken = JSON.parse(atob(payload));
    return {
      nameid: decoded.sub || decoded.nameid || '',
      email: decoded.email || '',
      role: (decoded.role as UserRole) || 'Client',
      permissions: decoded.permissions || [],
    };
  } catch {
    return null;
  }
}

interface AuthState {
  token: string | null;
  user: { id: string; email: string; role: UserRole; permissions: string[] } | null;
  isAuthenticated: boolean;
  login: (token: string) => void;
  logout: () => void;
  hasPermission: (permission: string) => boolean;
  hasRole: (role: UserRole | UserRole[]) => boolean;
}

export const useAuthStore = create<AuthState>((set, get) => ({
  token: localStorage.getItem('token'),
  user: (() => {
    const stored = localStorage.getItem('user');
    if (stored) {
      try {
        return JSON.parse(stored);
      } catch {
        return null;
      }
    }
    const token = localStorage.getItem('token');
    if (token) {
      const decoded = decodeToken(token);
      if (decoded) {
        const user = { id: decoded.nameid, email: decoded.email, role: decoded.role, permissions: decoded.permissions };
        localStorage.setItem('user', JSON.stringify(user));
        return user;
      }
    }
    return null;
  })(),
  isAuthenticated: !!localStorage.getItem('token'),

  login: (token: string) => {
    localStorage.setItem('token', token);
    const decoded = decodeToken(token);
    if (decoded) {
      const user = { id: decoded.nameid, email: decoded.email, role: decoded.role, permissions: decoded.permissions };
      localStorage.setItem('user', JSON.stringify(user));
      set({ token, user, isAuthenticated: true });
    }
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    set({ token: null, user: null, isAuthenticated: false });
  },

  hasPermission: (permission: string) => {
    const { user } = get();
    return user?.permissions.includes(permission) ?? false;
  },

  hasRole: (role: UserRole | UserRole[]) => {
    const { user } = get();
    if (!user) return false;
    const roles = Array.isArray(role) ? role : [role];
    return roles.includes(user.role);
  },
}));
