import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Form, Input, Button, Card, Typography, message } from 'antd';
import { MailOutlined, LockOutlined, PhoneOutlined, CarOutlined } from '@ant-design/icons';
import { authApi } from '../../api/endpoints';

const { Title, Text } = Typography;

export default function RegisterPage() {
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const onFinish = async (values: { email: string; phoneNumber: string; password: string }) => {
    setLoading(true);
    try {
      await authApi.register(values);
      message.success('Регистрация успешна! Проверьте почту для подтверждения email.');
      navigate('/verify-email', { state: { email: values.email } });
    } catch {
      message.error('Ошибка при регистрации. Возможно, email уже используется.');
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
            <Text style={{ color: '#888' }}>Регистрация</Text>
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
              <Input prefix={<MailOutlined style={{ color: '#666' }} />} placeholder="email@example.com" size="large" variant="filled" />
            </Form.Item>
            <Form.Item
              name="phoneNumber"
              label={<Text style={{ color: '#ccc' }}>Номер телефона</Text>}
              rules={[{ required: true, message: 'Введите номер телефона' }]}
            >
              <Input prefix={<PhoneOutlined style={{ color: '#666' }} />} placeholder="+375291234567" size="large" variant="filled" />
            </Form.Item>
            <Form.Item
              name="password"
              label={<Text style={{ color: '#ccc' }}>Пароль</Text>}
              rules={[
                { required: true, message: 'Введите пароль' },
                { min: 6, message: 'Минимум 6 символов' },
              ]}
            >
              <Input.Password prefix={<LockOutlined style={{ color: '#666' }} />} placeholder="Пароль" size="large" variant="filled" />
            </Form.Item>
            <Form.Item
              name="confirmPassword"
              label={<Text style={{ color: '#ccc' }}>Подтвердите пароль</Text>}
              dependencies={['password']}
              rules={[
                { required: true, message: 'Подтвердите пароль' },
                ({ getFieldValue }) => ({
                  validator(_, value) {
                    if (!value || getFieldValue('password') === value) return Promise.resolve();
                    return Promise.reject(new Error('Пароли не совпадают'));
                  },
                }),
              ]}
            >
              <Input.Password prefix={<LockOutlined style={{ color: '#666' }} />} placeholder="Подтвердите пароль" size="large" variant="filled" />
            </Form.Item>
            <Form.Item>
              <Button type="primary" htmlType="submit" loading={loading} block size="large" style={{ height: 44 }}>
                Зарегистрироваться
              </Button>
            </Form.Item>
          </Form>
          <div style={{ textAlign: 'center' }}>
            <Text style={{ color: '#888' }}>
              Уже есть аккаунт? <Link to="/login" style={{ color: '#f97316' }}>Войти</Link>
            </Text>
          </div>
        </div>
      </Card>
    </div>
  );
}
