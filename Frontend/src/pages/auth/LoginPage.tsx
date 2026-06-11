import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Form, Input, Button, Card, Typography, message } from 'antd';
import { MailOutlined, LockOutlined, CarOutlined } from '@ant-design/icons';
import { useAuthStore } from '../../stores/authStore';
import { authApi } from '../../api/endpoints';

const { Title, Text } = Typography;

export default function LoginPage() {
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const login = useAuthStore((s) => s.login);

  const onFinish = async (values: { email: string; password: string }) => {
    setLoading(true);
    try {
      const token = await authApi.login(values);
      login(token);
      message.success('Добро пожаловать!');
      navigate('/');
    } catch {
      message.error('Неверный email или пароль');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      style={{
        minHeight: '100vh',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        background: '#0a0a0a',
      }}
    >
      <Card
        style={{
          width: 420,
          background: '#1a1a1a',
          border: '1px solid rgba(255,255,255,0.06)',
          boxShadow: '0 8px 32px rgba(0,0,0,0.4)',
        }}
      >
        <div style={{ display: 'flex', flexDirection: 'column', gap: 24, width: '100%' }}>
          <div style={{ textAlign: 'center' }}>
            <CarOutlined style={{ fontSize: 40, color: '#f97316', marginBottom: 8 }} />
            <Title level={3} style={{ color: '#fff', margin: 0 }}>Premium Auto</Title>
            <Text style={{ color: '#888' }}>Вход в систему</Text>
          </div>
          <Form layout="vertical" onFinish={onFinish} autoComplete="off">
            <Form.Item
              name="email"
              label={<Text style={{ color: '#ccc' }}>Email</Text>}
              rules={[
                { required: true, message: 'Введите email' },
                { type: 'email', message: 'Неверный формат email' },
              ]}
            >
              <Input
                prefix={<MailOutlined style={{ color: '#666' }} />}
                placeholder="email@example.com"
                size="large"
                variant="filled"
              />
            </Form.Item>
            <Form.Item
              name="password"
              label={<Text style={{ color: '#ccc' }}>Пароль</Text>}
              rules={[{ required: true, message: 'Введите пароль' }]}
            >
              <Input.Password
                prefix={<LockOutlined style={{ color: '#666' }} />}
                placeholder="Пароль"
                size="large"
                variant="filled"
              />
            </Form.Item>
            <Form.Item>
              <Button type="primary" htmlType="submit" loading={loading} block size="large" style={{ height: 44 }}>
                Войти
              </Button>
            </Form.Item>
          </Form>
          <div style={{ textAlign: 'center' }}>
            <Text style={{ color: '#888' }}>
              Нет аккаунта? <Link to="/register" style={{ color: '#f97316' }}>Зарегистрироваться</Link>
            </Text>
          </div>
        </div>
      </Card>
    </div>
  );
}
