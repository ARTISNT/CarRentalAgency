import { useEffect, useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { Form, Input, Button, Card, Typography, message, Result, Spin } from 'antd';
import { CarOutlined, MailOutlined } from '@ant-design/icons';
import { authApi } from '../../api/endpoints';

const { Title, Text } = Typography;

type Status = 'ok' | 'already_verified' | 'expired' | 'invalid' | 'pending';

function deriveStatus(search: URLSearchParams): Status {
  const status = search.get('status');
  if (status === 'ok') return 'ok';
  if (status === 'already_verified') return 'already_verified';
  if (status === 'expired') return 'expired';
  if (status === 'invalid') return 'invalid';
  return 'pending';
}

export default function VerifyEmailPage() {
  const [search] = useSearchParams();
  const navigate = useNavigate();
  const status: Status = deriveStatus(search);
  const [submitting, setSubmitting] = useState(false);
  const [resent, setResent] = useState(false);

  useEffect(() => {
    if (status === 'ok' || status === 'already_verified') {
      message.success(
        status === 'ok'
          ? 'Email подтверждён! Теперь вы можете войти.'
          : 'Email уже подтверждён. Можно войти.',
      );
    }
  }, [status]);

  const onResend = async (values: { email: string }) => {
    setSubmitting(true);
    try {
      const result = await authApi.resendVerification(values.email);
      if (result.result === 'AlreadyVerified') {
        message.info('Этот email уже подтверждён — войдите в аккаунт.');
      } else if (result.result === 'UserNotFound') {
        message.error('Пользователь с таким email не найден.');
      } else {
        setResent(true);
        message.success('Письмо отправлено повторно. Проверьте почту.');
      }
    } catch {
      message.error('Не удалось отправить письмо. Попробуйте позже.');
    } finally {
      setSubmitting(false);
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
        padding: 24,
      }}
    >
      <Card
        style={{
          width: 460,
          background: '#1a1a1a',
          border: '1px solid rgba(255,255,255,0.06)',
          boxShadow: '0 8px 32px rgba(0,0,0,0.4)',
        }}
      >
        <div style={{ display: 'flex', flexDirection: 'column', gap: 24, width: '100%' }}>
          <div style={{ textAlign: 'center' }}>
            <CarOutlined style={{ fontSize: 40, color: '#f97316', marginBottom: 8 }} />
            <Title level={3} style={{ color: '#fff', margin: 0 }}>Premium Auto</Title>
            <Text style={{ color: '#888' }}>Подтверждение email</Text>
          </div>

          {status === 'pending' && (
            <>
              <Result
                icon={<MailOutlined style={{ color: '#f97316' }} />}
                title={<span style={{ color: '#fff' }}>Проверьте почту</span>}
                subTitle={
                  <span style={{ color: '#aaa' }}>
                    Мы отправили письмо со ссылкой для подтверждения. Откройте её, чтобы активировать аккаунт.
                  </span>
                }
                style={{ padding: 0, background: 'transparent' }}
              />
              {!resent ? (
                <Form layout="vertical" onFinish={onResend} autoComplete="off">
                  <Text style={{ color: '#888', display: 'block', marginBottom: 8 }}>
                    Не пришло письмо? Отправим повторно:
                  </Text>
                  <Form.Item
                    name="email"
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
                  <Form.Item>
                    <Button
                      type="primary"
                      htmlType="submit"
                      loading={submitting}
                      block
                      size="large"
                      style={{ height: 44 }}
                    >
                      Отправить письмо ещё раз
                    </Button>
                  </Form.Item>
                </Form>
              ) : (
                <Result
                  status="success"
                  title={<span style={{ color: '#fff' }}>Письмо отправлено</span>}
                  subTitle={
                    <span style={{ color: '#aaa' }}>Проверьте папку «Спам», если письма нет во входящих.</span>
                  }
                  style={{ padding: 0, background: 'transparent' }}
                />
              )}
            </>
          )}

          {status === 'ok' && (
            <Result
              status="success"
              title={<span style={{ color: '#fff' }}>Email подтверждён</span>}
              subTitle={<span style={{ color: '#aaa' }}>Теперь вы можете войти в аккаунт.</span>}
              extra={[
                <Button
                  key="login"
                  type="primary"
                  size="large"
                  onClick={() => navigate('/login')}
                  style={{ height: 44, minWidth: 160 }}
                >
                  Войти
                </Button>,
              ]}
              style={{ padding: 0, background: 'transparent' }}
            />
          )}

          {status === 'already_verified' && (
            <Result
              status="info"
              title={<span style={{ color: '#fff' }}>Email уже подтверждён</span>}
              subTitle={<span style={{ color: '#aaa' }}>Можно войти в аккаунт.</span>}
              extra={[
                <Button
                  key="login"
                  type="primary"
                  size="large"
                  onClick={() => navigate('/login')}
                  style={{ height: 44, minWidth: 160 }}
                >
                  Войти
                </Button>,
              ]}
              style={{ padding: 0, background: 'transparent' }}
            />
          )}

          {(status === 'expired' || status === 'invalid') && (
            <>
              <Result
                status="warning"
                title={
                  <span style={{ color: '#fff' }}>
                    {status === 'expired' ? 'Ссылка истёкла' : 'Ссылка недействительна'}
                  </span>
                }
                subTitle={
                  <span style={{ color: '#aaa' }}>
                    Запросите новое письмо с ссылкой для подтверждения.
                  </span>
                }
                style={{ padding: 0, background: 'transparent' }}
              />
              <Form layout="vertical" onFinish={onResend} autoComplete="off">
                <Form.Item
                  name="email"
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
                <Form.Item>
                  <Button
                    type="primary"
                    htmlType="submit"
                    loading={submitting}
                    block
                    size="large"
                    style={{ height: 44 }}
                  >
                    Отправить новое письмо
                  </Button>
                </Form.Item>
              </Form>
            </>
          )}

          <div style={{ textAlign: 'center' }}>
            <Text style={{ color: '#888' }}>
              <Link to="/login" style={{ color: '#f97316' }}>К странице входа</Link>
            </Text>
          </div>

          {status === 'pending' && !resent ? null : <Spin style={{ display: 'none' }} />}
        </div>
      </Card>
    </div>
  );
}
