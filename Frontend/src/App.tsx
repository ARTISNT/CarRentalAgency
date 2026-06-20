import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ConfigProvider, App as AntApp, theme } from 'antd';
import ruRU from 'antd/locale/ru_RU';
import HeaderLayout from './components/HeaderLayout';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/auth/LoginPage';
import RegisterPage from './pages/auth/RegisterPage';
import VerifyEmailPage from './pages/auth/VerifyEmailPage';
import LandingPage from './pages/LandingPage';
import CarCatalogPage from './pages/cars/CarCatalogPage';
import CarDetailPage from './pages/cars/CarDetailPage';
import CreateRentalPage from './pages/rentals/CreateRentalPage';
import MyRentalsPage from './pages/rentals/MyRentalsPage';
import RentalDetailPage from './pages/rentals/RentalDetailPage';
import PaymentPage from './pages/rentals/PaymentPage';
import PaymentCallback from './pages/rentals/PaymentCallback';
import PassportPage from './pages/profile/PassportPage';
import MyContractsPage from './pages/contracts/MyContractsPage';
import ContractSignPage from './pages/contracts/ContractSignPage';
import AdminCarsPage from './pages/admin/AdminCarsPage';
import AdminRentalsPage from './pages/admin/AdminRentalsPage';
import AdminContractsPage from './pages/admin/AdminContractsPage';
import AdminTemplatesPage from './pages/admin/AdminTemplatesPage';
import AdminUsersPage from './pages/admin/AdminUsersPage';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
      staleTime: 30000,
    },
  },
});

const themeConfig = {
  algorithm: theme.darkAlgorithm,
  token: {
    colorPrimary: '#f97316',
    colorBgContainer: '#1a1a1a',
    colorBgElevated: '#242424',
    colorBorder: '#2e2e2e',
    borderRadius: 8,
    colorBgLayout: '#0a0a0a',
    fontFamily: "'Inter', -apple-system, BlinkMacSystemFont, sans-serif",
  },
  components: {
    Layout: {
      headerBg: '#111111',
      bodyBg: '#0a0a0a',
    },
    Card: {
      colorBgContainer: '#1a1a1a',
    },
    Button: {
      primaryShadow: '0 4px 14px 0 rgba(249,115,22,0.35)',
    },
    Menu: {
      colorBgContainer: 'transparent',
      itemHoverBg: 'rgba(249,115,22,0.1)',
    },
  },
};

function App() {
  return (
    <ConfigProvider locale={ruRU} theme={themeConfig}>
      <AntApp>
        <QueryClientProvider client={queryClient}>
          <BrowserRouter>
            <Routes>
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route path="/verify-email" element={<VerifyEmailPage />} />

              <Route element={<HeaderLayout />}>
                <Route path="/" element={<LandingPage />} />
                <Route path="/cars" element={<CarCatalogPage />} />
                <Route path="/cars/:id" element={<CarDetailPage />} />

                <Route
                  path="/rentals/new/:carId"
                  element={
                    <ProtectedRoute>
                      <CreateRentalPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/my-rentals"
                  element={
                    <ProtectedRoute>
                      <MyRentalsPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/my-rentals/:id"
                  element={
                    <ProtectedRoute>
                      <RentalDetailPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/my-rentals/:id/pay"
                  element={
                    <ProtectedRoute>
                      <PaymentPage />
                    </ProtectedRoute>
                  }
                />

                <Route
                  path="/profile"
                  element={
                    <ProtectedRoute>
                      <PassportPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/my-contracts"
                  element={
                    <ProtectedRoute>
                      <MyContractsPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/my-contracts/:id/sign"
                  element={
                    <ProtectedRoute>
                      <ContractSignPage />
                    </ProtectedRoute>
                  }
                />

                <Route
                  path="/admin/cars"
                  element={
                    <ProtectedRoute permission="ViewCars">
                      <AdminCarsPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/rentals"
                  element={
                    <ProtectedRoute permission="ViewRents">
                      <AdminRentalsPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/contracts"
                  element={
                    <ProtectedRoute permission="ViewContracts">
                      <AdminContractsPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/templates"
                  element={
                    <ProtectedRoute roles={['Manager', 'Admin']}>
                      <AdminTemplatesPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/users"
                  element={
                    <ProtectedRoute permission="ViewUsers">
                      <AdminUsersPage />
                    </ProtectedRoute>
                  }
                />
              </Route>

              <Route path="/payment/callback" element={<PaymentCallback />} />

              <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
          </BrowserRouter>
        </QueryClientProvider>
      </AntApp>
    </ConfigProvider>
  );
}

export default App;
