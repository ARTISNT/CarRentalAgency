import { Button, Card, Result, Typography } from 'antd';
import { StopOutlined, MailOutlined, LogoutOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../stores/authStore';

const { Title, Text } = Typography;

export default function AccountDeactivatedPage() {
  const navigate = useNavigate();
  const logout = useAuthStore((s) => s.logout);

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <div
      style={{
        minHeight: '100vh',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        background: '#0a0a0a',
        padding: 24,
      }}
    >
      <Card
        style={{
          width: 480,
          background: '#1a1a1a',
          border: '1px solid rgba(255,255,255,0.06)',
          boxShadow: '0 8px 32px rgba(0,0,0,0.4)',
        }}
      >
        <div style={{ display: 'flex', flexDirection: 'column', gap: 20, width: '100%' }}>
          <div style={{ textAlign: 'center' }}>
            <StopOutlined style={{ fontSize: 56, color: '#ef4444', marginBottom: 12 }} />
            <Title level={3} style={{ color: '#fff', margin: 0 }}>Аккаунт деактивирован</Title>
          </div>

          <Result
            status="error"
            title={<span style={{ color: '#fff' }}>Доступ заблокирован</span>}
            subTitle={
              <span style={{ color: '#aaa' }}>
                Ваш аккаунт был деактивирован администратором. Свяжитесь со службой поддержки для восстановления доступа.
              </span>
            }
            style={{ padding: 0, background: 'transparent' }}
          />

          <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
            <Button
              type="primary"
              size="large"
              icon={<MailOutlined />}
              href="mailto:support@carrental.agency"
              style={{ height: 44 }}
            >
              Связаться с поддержкой
            </Button>
            <Button
              size="large"
              icon={<LogoutOutlined />}
              onClick={handleLogout}
              style={{ height: 44 }}
            >
              Выйти
            </Button>
          </div>

          <div style={{ textAlign: 'center' }}>
            <Text style={{ color: '#666', fontSize: 12 }}>
              Если вы считаете, что произошла ошибка, напишите нам на{' '}
              <a href="mailto:support@carrental.agency" style={{ color: '#f97316' }}>
                support@carrental.agency
              </a>
            </Text>
          </div>
        </div>
      </Card>
    </div>
  );
}
