import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Card,
  Typography,
  Spin,
  Form,
  Input,
  Button,
  DatePicker,
  Descriptions,
  Divider,
  message,
  Space,
  Tag,
  Row,
  Col,
} from 'antd';
import {
  UserOutlined,
  SaveOutlined,
  ArrowLeftOutlined,
  CheckCircleOutlined,
  ExclamationCircleOutlined,
} from '@ant-design/icons';
import dayjs from 'dayjs';
import { userApi, authApi } from '../../api/endpoints';
import { useAuthStore } from '../../stores/authStore';
import type { PassportRequest } from '../../types';

const { Title, Text } = Typography;

export default function PassportPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const user = useAuthStore((s) => s.user);
  const [form] = Form.useForm<PassportRequest>();

  const userId = user?.id;

  const { data: userData, isLoading } = useQuery({
    queryKey: ['user-passport', userId],
    queryFn: () => userApi.getWithPassport(userId!),
    enabled: !!userId,
  });

  const passport = userData?.passportDto;
  const isReadonly = !!passport;

  useEffect(() => {
    if (passport) {
      form.setFieldsValue({
        ...passport,
        passportIssueDate: passport.passportIssueDate
          ? dayjs(passport.passportIssueDate).format('YYYY-MM-DD')
          : undefined,
        birthDate: passport.birthDate
          ? dayjs(passport.birthDate).format('YYYY-MM-DD')
          : undefined,
      });
    }
  }, [passport, form]);

  const saveMutation = useMutation({
    mutationFn: (data: PassportRequest) => authApi.addPassport(userId!, data),
    onSuccess: () => {
      message.success('Паспортные данные сохранены');
      queryClient.invalidateQueries({ queryKey: ['user-passport', userId] });
    },
    onError: () => message.error('Ошибка при сохранении'),
  });

  const handleSave = (values: PassportRequest) => {
    saveMutation.mutate({
      ...values,
      passportIssueDate: dayjs(values.passportIssueDate).format('YYYY-MM-DD'),
      birthDate: dayjs(values.birthDate).format('YYYY-MM-DD'),
    });
  };

  if (isLoading) {
    return (
      <div style={{ textAlign: 'center', padding: 120 }}>
        <Spin size="large" />
      </div>
    );
  }

  return (
    <div style={{ maxWidth: 800, margin: '0 auto', padding: '32px' }}>
      <Button
        type="text"
        icon={<ArrowLeftOutlined />}
        onClick={() => navigate('/')}
        style={{ color: '#888', marginBottom: 16, padding: 0 }}
      >
        Назад
      </Button>

      <Title level={3} style={{ color: '#fff', marginBottom: 24 }}>
        <UserOutlined style={{ marginRight: 8, color: '#f97316' }} />
        Мой профиль
      </Title>

      <Row gutter={24}>
        <Col xs={24} lg={24}>
          <Card
            style={{
              background: '#1a1a1a',
              border: '1px solid rgba(255,255,255,0.06)',
              marginBottom: 24,
            }}
          >
            <Descriptions
              column={2}
              size="small"
              styles={{
                label: { color: '#888' },
                content: { color: '#fff' },
              }}
              bordered
            >
              <Descriptions.Item label="Email">
                <Space>
                  <Text style={{ color: '#fff' }}>{userData?.email || user?.email}</Text>
                  {userData?.emailVerified ? (
                    <CheckCircleOutlined style={{ color: '#22c55e' }} />
                  ) : (
                    <ExclamationCircleOutlined style={{ color: '#f97316' }} />
                  )}
                </Space>
              </Descriptions.Item>
              <Descriptions.Item label="Телефон">
                <Text style={{ color: '#fff' }}>{userData?.phoneNumber || '—'}</Text>
              </Descriptions.Item>
              <Descriptions.Item label="Роль">
                <Tag style={{ backgroundColor: '#f97316', color: '#fff', border: 'none' }}>
                  {userData?.role === 'Client' ? 'Клиент' : userData?.role}
                </Tag>
              </Descriptions.Item>
              <Descriptions.Item label="Статус">
                <Tag
                  style={{
                    backgroundColor: userData?.isActive ? '#22c55e' : '#ef4444',
                    color: '#fff',
                    border: 'none',
                  }}
                >
                  {userData?.isActive ? 'Активен' : 'Неактивен'}
                </Tag>
              </Descriptions.Item>
            </Descriptions>
          </Card>

          <Card
            style={{
              background: '#1a1a1a',
              border: '1px solid rgba(255,255,255,0.06)',
            }}
          >
            <Title level={5} style={{ color: '#fff', marginBottom: 20 }}>
              Паспортные данные
              {passport && (
                <Tag style={{ marginLeft: 12, backgroundColor: '#22c55e', color: '#fff', border: 'none' }}>
                  Заполнены
                </Tag>
              )}
            </Title>

            <Form
              form={form}
              layout="vertical"
              onFinish={handleSave}
              disabled={isReadonly}
              style={{ maxWidth: 600 }}
            >
              <Row gutter={16}>
                <Col xs={24} sm={8}>
                  <Form.Item
                    name="surname"
                    label={<Text style={{ color: '#ccc' }}>Фамилия</Text>}
                    rules={[{ required: true, message: 'Введите фамилию' }]}
                  >
                    <Input
                      style={{
                        background: '#0a0a0a',
                        border: '1px solid rgba(255,255,255,0.1)',
                        color: '#fff',
                      }}
                    />
                  </Form.Item>
                </Col>
                <Col xs={24} sm={8}>
                  <Form.Item
                    name="name"
                    label={<Text style={{ color: '#ccc' }}>Имя</Text>}
                    rules={[{ required: true, message: 'Введите имя' }]}
                  >
                    <Input
                      style={{
                        background: '#0a0a0a',
                        border: '1px solid rgba(255,255,255,0.1)',
                        color: '#fff',
                      }}
                    />
                  </Form.Item>
                </Col>
                <Col xs={24} sm={8}>
                  <Form.Item
                    name="patronymic"
                    label={<Text style={{ color: '#ccc' }}>Отчество</Text>}
                  >
                    <Input
                      style={{
                        background: '#0a0a0a',
                        border: '1px solid rgba(255,255,255,0.1)',
                        color: '#fff',
                      }}
                    />
                  </Form.Item>
                </Col>
              </Row>

              <Row gutter={16}>
                <Col xs={24} sm={12}>
                  <Form.Item
                    name="passportNumber"
                    label={<Text style={{ color: '#ccc' }}>Номер паспорта</Text>}
                    rules={[{ required: true, message: 'Введите номер паспорта' }]}
                  >
                    <Input
                      style={{
                        background: '#0a0a0a',
                        border: '1px solid rgba(255,255,255,0.1)',
                        color: '#fff',
                      }}
                    />
                  </Form.Item>
                </Col>
                <Col xs={24} sm={12}>
                  <Form.Item
                    name="identityNumber"
                    label={<Text style={{ color: '#ccc' }}>Идентификационный номер</Text>}
                    rules={[{ required: true, message: 'Введите идентификационный номер' }]}
                  >
                    <Input
                      style={{
                        background: '#0a0a0a',
                        border: '1px solid rgba(255,255,255,0.1)',
                        color: '#fff',
                      }}
                    />
                  </Form.Item>
                </Col>
              </Row>

              <Row gutter={16}>
                <Col xs={24} sm={12}>
                  <Form.Item
                    name="birthDate"
                    label={<Text style={{ color: '#ccc' }}>Дата рождения</Text>}
                    rules={[{ required: true, message: 'Выберите дату рождения' }]}
                    getValueFromEvent={(date: dayjs.Dayjs | null) => date}
                    getValueProps={(value: string) => ({
                      value: value ? dayjs(value) : null,
                    })}
                  >
                    <DatePicker
                      style={{
                        width: '100%',
                        background: '#0a0a0a',
                        border: '1px solid rgba(255,255,255,0.1)',
                      }}
                      placeholder="Выберите дату"
                    />
                  </Form.Item>
                </Col>
                <Col xs={24} sm={12}>
                  <Form.Item
                    name="passportIssueDate"
                    label={<Text style={{ color: '#ccc' }}>Дата выдачи паспорта</Text>}
                    rules={[{ required: true, message: 'Выберите дату выдачи' }]}
                    getValueFromEvent={(date: dayjs.Dayjs | null) => date}
                    getValueProps={(value: string) => ({
                      value: value ? dayjs(value) : null,
                    })}
                  >
                    <DatePicker
                      style={{
                        width: '100%',
                        background: '#0a0a0a',
                        border: '1px solid rgba(255,255,255,0.1)',
                      }}
                      placeholder="Выберите дату"
                    />
                  </Form.Item>
                </Col>
              </Row>

              <Divider style={{ borderColor: 'rgba(255,255,255,0.06)' }} />

              {!isReadonly && (
                <Button
                  type="primary"
                  icon={<SaveOutlined />}
                  htmlType="submit"
                  loading={saveMutation.isPending}
                  size="large"
                >
                  Сохранить
                </Button>
              )}
            </Form>
          </Card>
        </Col>
      </Row>
    </div>
  );
}
