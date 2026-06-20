import axios from 'axios';

const apiClient = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    const status = error.response?.status;
    const reason = (error.response?.data as { error?: string } | undefined)?.error;
    const isBlob = error.config?.responseType === 'blob';

    if (status === 403 && reason === 'email_not_verified' && !isBlob) {
      const userRaw = localStorage.getItem('user');
      const email = userRaw ? (JSON.parse(userRaw) as { email?: string }).email : undefined;
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      const target = email
        ? `/verify-email?email=${encodeURIComponent(email)}`
        : '/verify-email';
      window.location.href = target;
      return Promise.reject(error);
    }

    if (status === 401 && !isBlob) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  },
);

export default apiClient;
