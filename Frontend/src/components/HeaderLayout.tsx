import { useState } from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { Layout, Menu, Button, Dropdown, Avatar, Typography, Space, theme } from 'antd';
import {
  CarOutlined,
  UserOutlined,
  LogoutOutlined,
  ScheduleOutlined,
  FileTextOutlined,
  TeamOutlined,
  SnippetsOutlined,
  MenuOutlined,
} from '@ant-design/icons';
import { useAuthStore } from '../stores/authStore';

const { Header, Content, Footer } = Layout;
const { Text } = Typography;

const roleLabels: Record<string, string> = {
  Admin: 'Администратор',
  Manager: 'Менеджер',
  Client: 'Клиент',
};

export default function HeaderLayout() {
  const navigate = useNavigate();
  const location = useLocation();
  const { token } = theme.useToken();
  const { isAuthenticated, user, logout } = useAuthStore();
  const [mobileOpen, setMobileOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  const navItems = [
    { key: '/', label: 'Главная' },
    { key: '/cars', label: 'Автомобили' },
  ];

  const isLanding = location.pathname === '/';

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Header
        style={{
          position: 'fixed',
          top: 0,
          left: 0,
          right: 0,
          zIndex: 100,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          height: 64,
          padding: '0 32px',
          background: 'rgba(17,17,17,0.95)',
          backdropFilter: 'blur(12px)',
          borderBottom: '1px solid rgba(255,255,255,0.06)',
        }}
      >
        <Space size={32}>
          <div
            onClick={() => navigate('/')}
            style={{
              cursor: 'pointer',
              display: 'flex',
              alignItems: 'center',
              gap: 8,
            }}
          >
            <CarOutlined style={{ fontSize: 22, color: token.colorPrimary }} />
            <Text strong style={{ fontSize: 18, color: '#fff', letterSpacing: '-0.5px' }}>
              Premium Auto
            </Text>
          </div>

          <Menu
            mode="horizontal"
            selectedKeys={[navItems.find((i) => location.pathname.startsWith(i.key))?.key || '']}
            items={navItems}
            onClick={({ key }) => {
              navigate(key);
              setMobileOpen(false);
            }}
            style={{
              flex: 1,
              minWidth: 0,
              border: 'none',
              background: 'transparent',
            }}
            className="desktop-menu"
          />
        </Space>

        <Space size={12}>
          {isAuthenticated && user ? (
            <Dropdown
              menu={{
                items: [
                  {
                    key: 'email',
                    label: user.email,
                    disabled: true,
                  },
                  {
                    key: 'role',
                    label: roleLabels[user.role] || user.role,
                    disabled: true,
                  },
                  {
                    key: 'profile',
                    icon: <UserOutlined />,
                    label: 'Профиль',
                    onClick: () => navigate('/profile'),
                  },
                  {
                    key: 'rentals',
                    icon: <ScheduleOutlined />,
                    label: 'Мои аренды',
                    onClick: () => navigate('/my-rentals'),
                  },
                  {
                    key: 'contracts',
                    icon: <FileTextOutlined />,
                    label: 'Мои договоры',
                    onClick: () => navigate('/my-contracts'),
                  },
                  ...((user.role === 'Manager' || user.role === 'Admin')
                    ? [{
                        key: 'admin-group',
                        type: 'group' as const,
                        label: 'Администрирование',
                        children: [
                          ...(user.permissions.includes('ViewCars')
                            ? [{ key: 'admin-cars', icon: <CarOutlined />, label: 'Автомобили', onClick: () => navigate('/admin/cars') }]
                            : []),
                          ...(user.permissions.includes('ViewRents')
                            ? [{ key: 'admin-rentals', icon: <ScheduleOutlined />, label: 'Аренды', onClick: () => navigate('/admin/rentals') }]
                            : []),
                          ...(user.permissions.includes('ViewContracts')
                            ? [{ key: 'admin-contracts', icon: <FileTextOutlined />, label: 'Договоры', onClick: () => navigate('/admin/contracts') }]
                            : []),
                          { key: 'admin-templates', icon: <SnippetsOutlined />, label: 'Шаблоны', onClick: () => navigate('/admin/templates') },
                          ...(user.permissions.includes('ViewUsers')
                            ? [{ key: 'admin-users', icon: <TeamOutlined />, label: 'Пользователи', onClick: () => navigate('/admin/users') }]
                            : []),
                        ].filter(Boolean),
                      }]
                    : []),
                  { type: 'divider' },
                  {
                    key: 'logout',
                    icon: <LogoutOutlined />,
                    label: 'Выйти',
                    onClick: handleLogout,
                    danger: true,
                  },
                ],
              }}
            >
              <Space style={{ cursor: 'pointer', gap: 8 }}>
                <Avatar
                  icon={<UserOutlined />}
                  style={{ backgroundColor: token.colorPrimary }}
                  size="small"
                />
                <Text style={{ color: '#ccc' }}>{user.email}</Text>
              </Space>
            </Dropdown>
          ) : (
            <>
              <Button type="text" style={{ color: '#ccc' }} onClick={() => navigate('/login')}>
                Войти
              </Button>
              <Button type="primary" onClick={() => navigate('/register')}>
                Регистрация
              </Button>
            </>
          )}

          <Button
            type="text"
            icon={<MenuOutlined style={{ color: '#ccc' }} />}
            className="mobile-menu-btn"
            onClick={() => setMobileOpen(!mobileOpen)}
          />
        </Space>
      </Header>

      <Content
        style={{
          marginTop: 64,
          minHeight: 'calc(100vh - 128px)',
          background: isLanding ? '#0a0a0a' : undefined,
        }}
      >
        <Outlet />
      </Content>

      <Footer
        style={{
          background: '#111',
          borderTop: '1px solid rgba(255,255,255,0.06)',
          padding: '24px 32px',
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          flexWrap: 'wrap',
          gap: 12,
        }}
      >
        <Space>
          <CarOutlined style={{ color: token.colorPrimary }} />
          <Text type="secondary">Premium Auto — аренда автомобилей</Text>
        </Space>
        <Text type="secondary" style={{ fontSize: 13 }}>
          © {new Date().getFullYear()} Premium Auto. Все права защищены.
        </Text>
      </Footer>
    </Layout>
  );
}
