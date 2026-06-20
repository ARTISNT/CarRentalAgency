import { Navigate } from 'react-router-dom';
import { useAuthStore } from '../stores/authStore';
import type { UserRole } from '../types';

interface Props {
  children: React.ReactNode;
  roles?: UserRole[];
  permission?: string;
}

export default function ProtectedRoute({ children, roles, permission }: Props) {
  const { isAuthenticated, hasRole, hasPermission, user } = useAuthStore();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (user && user.emailVerified === false) {
    return <Navigate to="/verify-email" replace state={{ email: user.email }} />;
  }

  if (roles && !hasRole(roles)) {
    return <Navigate to="/" replace />;
  }

  if (permission && !hasPermission(permission)) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
}
