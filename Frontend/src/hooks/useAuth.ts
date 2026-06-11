import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../stores/authStore';
import { authApi } from '../api/endpoints';
import type { LoginRequest, RegisterRequest } from '../types';

export function useAuth() {
  const navigate = useNavigate();
  const { login: storeLogin, logout: storeLogout, user, isAuthenticated, hasPermission, hasRole } = useAuthStore();

  const login = async (data: LoginRequest) => {
    const token = await authApi.login(data);
    storeLogin(token);
    navigate('/');
  };

  const register = async (data: RegisterRequest) => {
    await authApi.register(data);
    navigate('/login');
  };

  const logout = () => {
    storeLogout();
    navigate('/login');
  };

  return { login, register, logout, user, isAuthenticated, hasPermission, hasRole };
}
