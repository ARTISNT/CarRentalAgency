import { useState } from 'react';
import { useParams, useNavigate, useSearchParams } from 'react-router-dom';
import { useQuery, useMutation } from '@tanstack/react-query';
import {
  Card,
  Typography,
  Spin,
  Button,
  Descriptions,
  Tag,
  message,
  Row,
  Col,
  Result,
  Divider,
  Space,
  Alert,
} from 'antd';
import {
  ArrowLeftOutlined,
  CreditCardOutlined,
  CheckCircleOutlined,
  LoadingOutlined,
  WarningOutlined,
} from '@ant-design/icons';
import dayjs from 'dayjs';
import { rentalApi, paymentApi } from '../../api/endpoints';
import type { RentActivityStatus } from '../../types';

const { Title, Text } = Typography;

const statusColors: Record<string, string> = {
  AwaitingConfirmation: '#f97316',
  Active: '#3b82f6',
  Completed: '#22c55e',
  Cancelled: '#ef4444',
};

const statusLabels: Record<string, string> = {
  AwaitingConfirmation: 'Ожидает',
  Active: 'Активна',
  Completed: 'Завершена',
  Cancelled: 'Отменена',
};

const paymentStatusLabels: Record<string, string> = {
  Pending: 'Ожидает оплаты',
  'Partially paid': 'Частично оплачено',
  Paid: 'Оплачено',
  Refunded: 'Возвращено',
  Failed: 'Ошибка',
};

const paymentStatusColors: Record<string, string> = {
  Pending: '#f97316',
  'Partially paid': '#3b82f6',
  Paid: '#22c55e',
  Refunded: '#888',
  Failed: '#ef4444',
};

export default function PaymentPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const paymentKind = searchParams.get('type') ?? 'remaining';
  const [paid, setPaid] = useState(false);

  const { data: rental, isLoading } = useQuery({
    queryKey: ['rental', id],
    queryFn: () => rentalApi.getById(id!),
    enabled: !!id,
    refetchInterval: paid ? 3000 : false,
  });

  const payMutation = useMutation({
    mutationFn: async (type: 'Deposit' | 'FullPayment' | 'fine' | 'additional' | 'remaining') => {
      if (type === 'fine') {
        return paymentApi.payFine(id!, {
          amount: rental?.fineOutstanding ?? 0,
          reason: 'Penalty',
        });
      }
      if (type === 'additional') {
        return paymentApi.payAdditional(id!, {
          amount: rental?.additionalOutstanding ?? 0,
          reason: 'Продление аренды',
        });
      }
      if (type === 'remaining') {
        return paymentApi.payRemaining(id!);
      }
      return paymentApi.pay(id!, type);
    },
    onSuccess: (redirectUrl) => {
      if (redirectUrl) {
        window.location.href = redirectUrl;
      } else {
        message.error('Не удалось получить ссылку на оплату');
      }
    },
    onError: (err: unknown) => {
      const e = err as { response?: { data?: { message?: string } }; message?: string };
      const msg = e?.response?.data?.message ?? e?.message ?? 'Ошибка при создании платежа';
      message.error(msg);
    },
  });

  const handleCheckStatus = () => {
    setPaid(true);
    setTimeout(() => {
      setPaid(false);
    }, 15000);
  };

  if (isLoading) {
    return (
      <div style={{ textAlign: 'center', padding: 120 }}>
        <Spin size="large" />
      </div>
    );
  }

  if (!rental) {
    return <div style={{ textAlign: 'center', padding: 120, color: '#888' }}>Аренда не найдена</div>;
  }

  const statusName = rental.activityStatus.name as RentActivityStatus;

  if (paymentKind === 'remaining'
    && statusName !== 'Active'
    && statusName !== 'Completed'
    && (rental.fineOutstanding ?? 0) === 0
    && (rental.additionalOutstanding ?? 0) === 0) {
    return (
      <div style={{ maxWidth: 600, margin: '0 auto', padding: '64px 32px' }}>
        <Result
          status="warning"
          icon={<WarningOutlined style={{ color: '#f97316', fontSize: 72 }} />}
          title={<Text style={{ color: '#fff', fontSize: 24 }}>Сначала оплатите депозит</Text>}
          subTitle={
            <Text style={{ color: '#888' }}>
              Аренда #{rental.id.slice(0, 8)} ещё не активна. Доплата будет доступна после оплаты депозита и подписания договора.
            </Text>
          }
          extra={
            <Space direction="vertical" size="middle" style={{ width: '100%' }}>
              <Button
                type="primary"
                size="large"
                icon={<CreditCardOutlined />}
                loading={payMutation.isPending}
                onClick={() => payMutation.mutate('Deposit')}
                block
              >
                Оплатить депозит {(rental.depositAmount ?? 0).toFixed(2)} Br
              </Button>
              <Button block onClick={() => navigate(`/my-rentals/${id}`)}>
                К аренде
              </Button>
            </Space>
          }
        />
      </div>
    );
  }

  if (paymentKind === 'remaining' && (rental.remainingAmount ?? 0) <= 0) {
    return (
      <div style={{ maxWidth: 600, margin: '0 auto', padding: '64px 32px' }}>
        <Result
          status="success"
          icon={<CheckCircleOutlined style={{ color: '#22c55e', fontSize: 72 }} />}
          title={<Text style={{ color: '#fff', fontSize: 24 }}>Нечего доплачивать</Text>}
          subTitle={
            <Text style={{ color: '#888' }}>
              Аренда #{rental.id.slice(0, 8)} полностью оплачена
            </Text>
          }
          extra={
            <Space>
              <Button type="primary" onClick={() => navigate(`/my-rentals/${id}`)}>
                К аренде
              </Button>
              <Button onClick={() => navigate('/my-rentals')}>
                Все аренды
              </Button>
            </Space>
          }
        />
      </div>
    );
  }

  return (
    <div style={{ maxWidth: 800, margin: '0 auto', padding: '32px' }}>
      <Button
        type="text"
        icon={<ArrowLeftOutlined />}
        onClick={() => navigate(`/my-rentals/${id}`)}
        style={{ color: '#888', marginBottom: 16, padding: 0 }}
      >
        Назад к аренде
      </Button>

      <Title level={3} style={{ color: '#fff', marginBottom: 24 }}>
        <CreditCardOutlined style={{ marginRight: 8, color: '#f97316' }} />
        {paymentKind === 'fine' ? 'Оплата штрафа' :
         paymentKind === 'additional' ? 'Доплата за продление' :
         paymentKind === 'remaining' ? 'Доплата по аренде' :
         'Оплата аренды'}
      </Title>

      <Row gutter={24}>
        <Col xs={24} lg={16}>
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
              <Descriptions.Item label="ID аренды">
                #{rental.id.slice(0, 8)}
              </Descriptions.Item>
              <Descriptions.Item label="Статус">
                <Tag style={{ backgroundColor: statusColors[statusName], color: '#fff', border: 'none' }}>
                  {statusLabels[statusName]}
                </Tag>
              </Descriptions.Item>
              <Descriptions.Item label="Автомобиль">
                {rental.car.brand} {rental.car.model} ({rental.car.licensePlate})
              </Descriptions.Item>
              <Descriptions.Item label="Стоимость">
                {rental.totalCost.toFixed(2)} Br
              </Descriptions.Item>
              <Descriptions.Item label="Начало">
                {dayjs(rental.startDate).format('DD.MM.YYYY HH:mm')}
              </Descriptions.Item>
              <Descriptions.Item label="Окончание">
                {dayjs(rental.endDate).format('DD.MM.YYYY HH:mm')}
              </Descriptions.Item>
            </Descriptions>
          </Card>
        </Col>

        <Col xs={24} lg={8}>
          <Card
            style={{
              background: '#1a1a1a',
              border: '1px solid rgba(255,255,255,0.06)',
              marginBottom: 24,
            }}
          >
            <div style={{ marginBottom: 16, padding: 12, background: 'rgba(255,255,255,0.03)', borderRadius: 8 }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 8 }}>
                <Text style={{ color: '#888' }}>Статус оплаты</Text>
                <Tag style={{ backgroundColor: paymentStatusColors[rental.paymentStatus ?? ''] || '#888', color: '#fff', border: 'none' }}>
                  {paymentStatusLabels[rental.paymentStatus ?? ''] ?? rental.paymentStatus ?? ''}
                </Tag>
              </div>
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 4 }}>
                <Text style={{ color: '#888' }}>Оплачено</Text>
                <Text style={{ color: '#22c55e' }}>{(rental.paidAmount ?? 0).toFixed(2)} Br</Text>
              </div>
              {(rental.remainingAmount ?? 0) > 0 && (
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 4 }}>
                  <Text style={{ color: '#888' }}>Осталось</Text>
                  <Text style={{ color: '#f97316' }}>{(rental.remainingAmount ?? 0).toFixed(2)} Br</Text>
                </div>
              )}
              <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                <Text style={{ color: '#888' }}>Всего</Text>
                <Text style={{ color: '#fff' }}>{(rental.requiredAmount ?? 0).toFixed(2)} Br</Text>
              </div>
            </div>

            {(rental.paymentStatus ?? '') === 'Paid' && paymentKind !== 'remaining' ? (
              <Result
                status="success"
                icon={<CheckCircleOutlined style={{ color: '#22c55e', fontSize: 48 }} />}
                title={<Text style={{ color: '#fff', fontSize: 16 }}>Оплачено полностью</Text>}
              />
            ) : (
              <>
                {paymentKind === 'fine' && (rental.fineOutstanding ?? 0) > 0 && (
                  <Alert
                    type="error"
                    showIcon
                    icon={<WarningOutlined />}
                    message={`К оплате штраф: ${(rental.fineOutstanding ?? 0).toFixed(2)} Br`}
                    description="После оплаты штрафа станет доступно продление аренды."
                    style={{ marginBottom: 12 }}
                  />
                )}

                {paymentKind === 'additional' && (rental.additionalOutstanding ?? 0) > 0 && (
                  <Alert
                    type="warning"
                    showIcon
                    message={`К доплате (продление): ${(rental.additionalOutstanding ?? 0).toFixed(2)} Br`}
                    style={{ marginBottom: 12 }}
                  />
                )}

                {paymentKind === 'remaining' && (rental.remainingAmount ?? 0) > 0 && (
                  <Alert
                    type="info"
                    showIcon
                    message={`К доплате: ${(rental.remainingAmount ?? 0).toFixed(2)} Br`}
                    style={{ marginBottom: 12 }}
                  />
                )}

                <Title level={5} style={{ color: '#fff', marginBottom: 16 }}>
                  Способ оплаты
                </Title>

                {paymentKind === 'fine' ? (
                  <Button
                    danger
                    type="primary"
                    size="large"
                    icon={<CreditCardOutlined />}
                    onClick={() => payMutation.mutate('fine')}
                    loading={payMutation.isPending}
                    block
                    disabled={(rental.fineOutstanding ?? 0) <= 0}
                  >
                    Оплатить штраф — {(rental.fineOutstanding ?? 0).toFixed(2)} Br
                  </Button>
                ) : paymentKind === 'additional' ? (
                  <Button
                    type="primary"
                    size="large"
                    icon={<CreditCardOutlined />}
                    onClick={() => payMutation.mutate('additional')}
                    loading={payMutation.isPending}
                    block
                    disabled={(rental.additionalOutstanding ?? 0) <= 0}
                  >
                    Доплатить — {(rental.additionalOutstanding ?? 0).toFixed(2)} Br
                  </Button>
                ) : paymentKind === 'remaining' ? (
                  <Button
                    type="primary"
                    size="large"
                    icon={<CreditCardOutlined />}
                    onClick={() => payMutation.mutate('remaining')}
                    loading={payMutation.isPending}
                    block
                    disabled={(rental.remainingAmount ?? 0) <= 0}
                  >
                    Доплатить — {(rental.remainingAmount ?? 0).toFixed(2)} Br
                  </Button>
                ) : (
                  <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
                    <Button
                      type="primary"
                      size="large"
                      icon={<CreditCardOutlined />}
                      onClick={() => payMutation.mutate('Deposit')}
                      loading={payMutation.isPending && payMutation.variables === 'Deposit'}
                      block
                    >
                      Оплатить депозит — {(rental.depositAmount ?? 0).toFixed(2)} Br
                    </Button>

                    <Button
                      size="large"
                      icon={<CreditCardOutlined />}
                      onClick={() => payMutation.mutate('FullPayment')}
                      loading={payMutation.isPending && payMutation.variables === 'FullPayment'}
                      block
                    >
                      Оплатить полностью — {(rental.requiredAmount ?? 0).toFixed(2)} Br
                    </Button>
                  </div>
                )}

                <Divider style={{ borderColor: 'rgba(255,255,255,0.06)' }} />

                <Button
                  icon={paid ? <LoadingOutlined /> : <CheckCircleOutlined />}
                  onClick={handleCheckStatus}
                  block
                  disabled={paid}
                >
                  {paid ? 'Проверяем статус...' : 'Проверить статус оплаты'}
                </Button>

                <Text style={{ color: '#666', display: 'block', marginTop: 12, fontSize: 13 }}>
                  После оплаты вы будете перенаправлены на страницу платёжной системы BePaid.
                  Для возврата используйте кнопку "Назад" в браузере.
                </Text>
              </>
            )}
          </Card>
        </Col>
      </Row>
    </div>
  );
}
