import { useRef, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Alert,
  Button,
  Card,
  Col,
  Collapse,
  DatePicker,
  Descriptions,
  Divider,
  Form,
  Input,
  InputNumber,
  Modal,
  Row,
  Space,
  Spin,
  Statistic,
  Tag,
  Typography,
  message,
} from 'antd';
import {
  ArrowLeftOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
  CreditCardOutlined,
  DownloadOutlined,
  EyeOutlined,
  FileTextOutlined,
  RollbackOutlined,
  WarningOutlined,
  HistoryOutlined,
} from '@ant-design/icons';
import dayjs from 'dayjs';
import { rentalApi, contractApi, paymentApi } from '../../api/endpoints';
import apiClient from '../../api/client';
import { useAuthStore } from '../../stores/authStore';
import type {
  EndRentalRequest,
  RentActivityStatus,
  ContractStatus,
  ContractResponse,
  PaymentType,
  TransactionStatus,
} from '../../types';

const { Title, Text } = Typography;

const statusColors: Record<string, string> = {
  AwaitingConfirmation: '#f97316',
  Scheduled: '#eab308',
  Active: '#3b82f6',
  Completed: '#22c55e',
  Cancelled: '#ef4444',
};

const statusLabels: Record<string, string> = {
  AwaitingConfirmation: 'Ожидает',
  Scheduled: 'Запланирована',
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

const MAX_CONTRACT_POLL_ATTEMPTS = 20;

export default function RentalDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const pollCountRef = useRef(0);
  const [pollExpired, setPollExpired] = useState(false);

  const { data: rental, isLoading } = useQuery({
    queryKey: ['rental', id],
    queryFn: () => rentalApi.getById(id!),
    enabled: !!id,
  });

  const { data: contracts } = useQuery({
    queryKey: ['contract-by-rental', id],
    queryFn: () => contractApi.getByRental(id!),
    enabled: !!id && !pollExpired,
    refetchInterval: (query) => {
      const data = query.state.data;
      const contractExists = data && data.length > 0;
      if (contractExists) {
        pollCountRef.current = 0;
        return false;
      }
      pollCountRef.current += 1;
      if (pollCountRef.current >= MAX_CONTRACT_POLL_ATTEMPTS) {
        setPollExpired(true);
        return false;
      }
      return 3000;
    },
  });

  const contract = contracts?.[0] as ContractResponse | undefined;

  const openPdf = async (contractId: string) => {
    try {
      const response = await apiClient.get<Blob>(
        `/Contract/get-contract-${contractId}/pdf`,
        { responseType: 'blob' },
      );
      const blob = new Blob([response.data], { type: 'application/pdf' });
      const url = URL.createObjectURL(blob);
      window.open(url, '_blank', 'noopener,noreferrer');
      setTimeout(() => URL.revokeObjectURL(url), 60_000);
    } catch {
      message.error('Не удалось открыть PDF');
    }
  };

  const cancelMutation = useMutation({
    mutationFn: () => rentalApi.cancel(id!, { reason: null }),
    onSuccess: () => {
      message.success('Аренда отменена');
      queryClient.invalidateQueries({ queryKey: ['rental', id] });
    },
    onError: () => message.error('Ошибка при отмене'),
  });

  const [isRenewModalOpen, setIsRenewModalOpen] = useState(false);
  const [renewForm] = Form.useForm();
  const renewMutation = useMutation({
    mutationFn: (data: { newDate: string }) => rentalApi.renew(id!, data),
    onSuccess: () => {
      message.success('Аренда продлена. Если требуется доплата, выполните её.');
      queryClient.invalidateQueries({ queryKey: ['rental', id] });
      setIsRenewModalOpen(false);
    },
    onError: (err: unknown) => {
      const e = err as { response?: { data?: { message?: string } }; message?: string };
      const msg = e?.response?.data?.message ?? e?.message ?? 'Ошибка при продлении';
      message.error(msg);
    },
  });

  const handleRenew = async () => {
    const values = await renewForm.validateFields();
    renewMutation.mutate({ newDate: values.newDate.toISOString() });
  };

  const { hasPermission } = useAuthStore();
  const [isEndModalOpen, setIsEndModalOpen] = useState(false);
  const [endForm] = Form.useForm();

  const endMutation = useMutation({
    mutationFn: (data: EndRentalRequest) => rentalApi.end(id!, data),
    onSuccess: () => {
      message.success('Аренда завершена');
      queryClient.invalidateQueries({ queryKey: ['rental', id] });
      queryClient.invalidateQueries({ queryKey: ['rentals'] });
      setIsEndModalOpen(false);
    },
    onError: () => message.error('Ошибка при завершении аренды'),
  });

  const handleEndRental = async () => {
    const values = await endForm.validateFields();
    endMutation.mutate({
      returnDate: values.returnDate.toISOString(),
      mileage: values.mileage,
      fuelLevel: values.fuelLevel / 100,
      penaltyAmount: values.penaltyAmount || 0,
      damageDescription: values.damageDescription || null,
    });
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
  const contractStatus = contract?.status as ContractStatus | undefined;

  return (
    <div style={{ maxWidth: 1200, margin: '0 auto', padding: '32px' }}>
      <Button
        type="text"
        icon={<ArrowLeftOutlined />}
        onClick={() => navigate('/my-rentals')}
        style={{ color: '#888', marginBottom: 16, padding: 0 }}
      >
        Назад
      </Button>

      <Row gutter={24}>
        <Col xs={24} lg={16}>
          <Card
            style={{
              background: '#1a1a1a',
              border: '1px solid rgba(255,255,255,0.06)',
              marginBottom: 24,
            }}
          >
            <div style={{ display: 'flex', flexDirection: 'column', gap: 24, width: '100%' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <Title level={4} style={{ color: '#fff', margin: 0 }}>
                  Аренда #{rental.id.slice(0, 8)}
                </Title>
                <Tag style={{ backgroundColor: statusColors[statusName], color: '#fff', border: 'none', padding: '2px 12px' }}>
                  {statusLabels[statusName]}
                </Tag>
              </div>

              <Descriptions
                column={2}
                size="small"
                styles={{
                  label: { color: '#888' },
                  content: { color: '#fff' },
                }}
                bordered
              >
                <Descriptions.Item label="Начало">
                  {dayjs(rental.startDate).format('DD.MM.YYYY HH:mm')}
                </Descriptions.Item>
                <Descriptions.Item label="Окончание">
                  {dayjs(rental.endDate).format('DD.MM.YYYY HH:mm')}
                </Descriptions.Item>
                <Descriptions.Item label="Стоимость">
                  {rental.totalCost.toFixed(2)} Br
                </Descriptions.Item>
              </Descriptions>

              <Descriptions
                title={<Text style={{ color: '#fff' }}>Автомобиль</Text>}
                column={1}
                size="small"
                styles={{
                  label: { color: '#888' },
                  content: { color: '#fff' },
                }}
                bordered
              >
                <Descriptions.Item label="Автомобиль">
                  {rental.car.brand} {rental.car.model} ({rental.car.licensePlate})
                </Descriptions.Item>
              </Descriptions>

              <Descriptions
                title={<Text style={{ color: '#fff' }}>Арендатор</Text>}
                column={1}
                size="small"
                styles={{
                  label: { color: '#888' },
                  content: { color: '#fff' },
                }}
                bordered
              >
                <Descriptions.Item label="Арендатор">
                  {rental.renter.surName} {rental.renter.name}
                </Descriptions.Item>
              </Descriptions>

              {(statusName === 'AwaitingConfirmation' || statusName === 'Scheduled') && hasPermission('EditRent') && (
                <>
                  <Divider style={{ borderColor: 'rgba(255,255,255,0.06)' }} />
                  <Space>
                    <Button
                      danger
                      icon={<CloseCircleOutlined />}
                      onClick={() =>
                        Modal.confirm({
                          title: 'Отменить аренду?',
                          content: statusName === 'Scheduled'
                            ? 'Депозит будет возвращён'
                            : 'Вы уверены?',
                          onOk: () => cancelMutation.mutate(),
                        })
                      }
                    >
                      Отменить аренду
                    </Button>
                  </Space>
                </>
              )}

              {statusName === 'Active' && hasPermission('EditRent') && (
                <>
                  <Divider style={{ borderColor: 'rgba(255,255,255,0.06)' }} />
                  <Space wrap>
                    <Button
                      type="primary"
                      icon={<RollbackOutlined />}
                      style={{ background: '#22c55e', borderColor: '#22c55e' }}
                      onClick={() => {
                        endForm.resetFields();
                        setIsEndModalOpen(true);
                      }}
                    >
                      Завершить аренду
                    </Button>
                    <Button
                      icon={<HistoryOutlined />}
                      onClick={() => {
                        renewForm.resetFields();
                        renewForm.setFieldsValue({
                          newDate: dayjs(rental.endDate).add(1, 'day'),
                        });
                        setIsRenewModalOpen(true);
                      }}
                      disabled={(rental.fineOutstanding ?? 0) > 0 || (rental.remainingAmount ?? 0) > 0}
                      title={(rental.fineOutstanding ?? 0) > 0
                        ? 'Сначала оплатите штраф'
                        : (rental.remainingAmount ?? 0) > 0
                          ? 'Сначала погасите задолженность'
                          : 'Продлить аренду'}
                    >
                      Продлить аренду
                    </Button>
                  </Space>
                </>
              )}
            </div>
          </Card>

          {statusName === 'AwaitingConfirmation' && (
            <Card
              style={{
                background: '#1a1a1a',
                border: '1px solid rgba(255,255,255,0.06)',
                marginBottom: 24,
              }}
            >
              <Title level={5} style={{ color: '#fff', marginBottom: 20 }}>
                Шаги для начала аренды
              </Title>

              <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
                {/* Step 1: Contract */}
                <div
                  style={{
                    display: 'flex',
                    alignItems: 'flex-start',
                    gap: 16,
                    padding: 16,
                    background: '#111',
                    borderRadius: 8,
                    border: '1px solid rgba(255,255,255,0.06)',
                  }}
                >
                  {!contract ? (
                    <FileTextOutlined style={{ fontSize: 24, color: '#666', marginTop: 2 }} />
                  ) : contractStatus === 'AwaitingSignature' ? (
                    <FileTextOutlined style={{ fontSize: 24, color: '#f97316', marginTop: 2 }} />
                  ) : (
                    <CheckCircleOutlined style={{ fontSize: 24, color: '#22c55e', marginTop: 2 }} />
                  )}
                  <div style={{ flex: 1 }}>
                    <Text style={{ color: '#fff', fontWeight: 600 }}>Договор аренды</Text>
                    <br />
                    <Text style={{ color: '#888', fontSize: 13 }}>
                      {!contract
                        ? 'Договор создаётся...'
                        : contractStatus === 'AwaitingSignature'
                          ? 'Договор создан, ожидает подписания'
                          : contractStatus === 'Active'
                            ? 'Договор подписан'
                            : contractStatus === 'Ended'
                              ? 'Договор завершён'
                              : 'Договор отменён'}
                    </Text>
                    <div style={{ marginTop: 8 }}>
                      {!contract && !pollExpired && (
                        <Spin size="small" style={{ display: 'block', marginTop: 8 }} />
                      )}
                      {!contract && pollExpired && (
                        <Alert
                          type="error"
                          message="Не удалось создать договор"
                          description="Договор не был создан. Пожалуйста, обратитесь в поддержку или попробуйте отменить аренду и создать её заново."
                          showIcon
                          style={{ marginTop: 8 }}
                        />
                      )}
                      {contract?.status === 'AwaitingSignature' && (
                        <Space>
                          <Button
                            type="primary"
                            size="small"
                            icon={<FileTextOutlined />}
                          onClick={() => navigate(`/my-contracts/${contract!.id}/sign`)}
                          >
                            Подписать
                          </Button>
                          <Button
                            size="small"
                            icon={<DownloadOutlined />}
                            onClick={() => openPdf(contract!.id)}
                          >
                            PDF
                          </Button>
                        </Space>
                      )}
                      {contractStatus === 'Active' && (
                        <Button
                          size="small"
                          icon={<EyeOutlined />}
                          style={{ color: '#3b82f6' }}
                          onClick={() => navigate(`/my-contracts/${contract!.id}/sign`)}
                        >
                          Просмотреть
                        </Button>
                      )}
                    </div>
                  </div>
                </div>

                {/* Step 2: Payment */}
                <div
                  style={{
                    display: 'flex',
                    alignItems: 'flex-start',
                    gap: 16,
                    padding: 16,
                    background: '#111',
                    borderRadius: 8,
                    border: '1px solid rgba(255,255,255,0.06)',
                    opacity: contractStatus !== 'Active' ? 0.5 : 1,
                  }}
                >
                  <CreditCardOutlined
                    style={{
                      fontSize: 24,
                      color: contractStatus === 'Active' ? '#f97316' : '#666',
                      marginTop: 2,
                    }}
                  />
                  <div style={{ flex: 1 }}>
                    <Text style={{ color: '#fff', fontWeight: 600 }}>Оплата депозита</Text>
                    <br />
                    <Text style={{ color: '#888', fontSize: 13 }}>
                      {contractStatus !== 'Active'
                        ? 'Сначала подпишите договор'
                        : 'Внесите депозит для начала аренды'}
                    </Text>
                    {contractStatus === 'Active' && (
                      <div style={{ marginTop: 8 }}>
                        <Button
                          type="primary"
                          size="small"
                          icon={<CreditCardOutlined />}
                          onClick={() => navigate(`/my-rentals/${id}/pay`)}
                        >
                          Оплатить
                        </Button>
                      </div>
                    )}
                  </div>
                </div>
              </div>
            </Card>
          )}
        </Col>

        <Col xs={24} lg={8}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 16, width: '100%' }}>
            <Card
              style={{
                background: '#1a1a1a',
                border: '1px solid rgba(255,255,255,0.06)',
              }}
            >
              <Statistic
                title={<Text style={{ color: '#888' }}>Длительность</Text>}
                value={Math.max(dayjs(rental.endDate).diff(dayjs(rental.startDate), 'day'), 1)}
                suffix="дн."
                valueStyle={{ color: '#fff' }}
              />
            </Card>

            {contract && (
              <Card
                style={{
                  background: '#1a1a1a',
                  border: '1px solid rgba(255,255,255,0.06)',
                }}
              >
                <Title level={5} style={{ color: '#fff', marginBottom: 12 }}>
                  Договор #{contract.id.slice(0, 8)}
                </Title>
                <Descriptions
                  column={1}
                  size="small"
                  styles={{
                    label: { color: '#888' },
                    content: { color: '#fff' },
                  }}
                >
                  <Descriptions.Item label="Статус">
                    <Tag
                      style={{
                        backgroundColor:
                          contract.status === 'AwaitingSignature' ? '#f97316'
                          : contract.status === 'Active' ? '#22c55e'
                          : contract.status === 'Ended' ? '#666'
                          : '#ef4444',
                        color: '#fff',
                        border: 'none',
                      }}
                    >
                      {contract.status === 'AwaitingSignature' ? 'Ожидает' :
                       contract.status === 'Active' ? 'Активен' :
                       contract.status === 'Ended' ? 'Завершён' : 'Отменён'}
                    </Tag>
                  </Descriptions.Item>
                  <Descriptions.Item label="Цена">
                    {contract.estimatedPrice.toFixed(2)} Br
                  </Descriptions.Item>
                </Descriptions>
              </Card>
            )}

            {(rental.paymentStatus || rental.requiredAmount > 0) && (
              <Card
                style={{
                  background: '#1a1a1a',
                  border: '1px solid rgba(255,255,255,0.06)',
                }}
              >
                <Title level={5} style={{ color: '#fff', marginBottom: 12 }}>
                  Оплата
                </Title>
                <div style={{ marginBottom: 8 }}>
                  <Tag style={{ backgroundColor: paymentStatusColors[rental.paymentStatus] || '#888', color: '#fff', border: 'none' }}>
                    {paymentStatusLabels[rental.paymentStatus] || rental.paymentStatus}
                  </Tag>
                </div>
                <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                    <Text style={{ color: '#888' }}>Оплачено</Text>
                    <Text style={{ color: '#22c55e' }}>{(rental.paidAmount ?? 0).toFixed(2)} Br</Text>
                  </div>
                  {(rental.remainingAmount ?? 0) > 0 && (
                    <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                      <Text style={{ color: '#888' }}>Осталось</Text>
                      <Text style={{ color: '#f97316' }}>{(rental.remainingAmount ?? 0).toFixed(2)} Br</Text>
                    </div>
                  )}
                  <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                    <Text style={{ color: '#888' }}>Всего</Text>
                    <Text style={{ color: '#fff' }}>{(rental.requiredAmount ?? 0).toFixed(2)} Br</Text>
                  </div>
                </div>

                {(rental.fineOutstanding ?? 0) > 0 && (
                  <>
                    <Divider style={{ borderColor: 'rgba(255,255,255,0.06)', margin: '12px 0' }} />
                    <Alert
                      type="error"
                      showIcon
                      icon={<WarningOutlined />}
                      message={
                        <span>
                          Непогашенный штраф: <b>{(rental.fineOutstanding ?? 0).toFixed(2)} Br</b>
                        </span>
                      }
                      description="Продление аренды недоступно, пока штраф не оплачен."
                      style={{ marginBottom: 8 }}
                    />
                    <Button
                      danger
                      block
                      icon={<CreditCardOutlined />}
                      onClick={() => navigate(`/my-rentals/${id}/pay?type=fine`)}
                    >
                      Оплатить штраф
                    </Button>
                  </>
                )}

                {(rental.additionalOutstanding ?? 0) > 0 && (
                  <>
                    {(rental.fineOutstanding ?? 0) === 0 && <Divider style={{ borderColor: 'rgba(255,255,255,0.06)', margin: '12px 0' }} />}
                    <Alert
                      type="warning"
                      showIcon
                      message={
                        <span>
                          К доплате (продление): <b>{(rental.additionalOutstanding ?? 0).toFixed(2)} Br</b>
                        </span>
                      }
                      style={{ marginBottom: 8, marginTop: 8 }}
                    />
                    <Button
                      block
                      icon={<CreditCardOutlined />}
                      onClick={() => navigate(`/my-rentals/${id}/pay?type=additional`)}
                    >
                      Доплатить за продление
                    </Button>
                  </>
                )}

                {(rental.remainingAmount ?? 0) > 0
                  && (rental.fineOutstanding ?? 0) === 0
                  && (rental.additionalOutstanding ?? 0) === 0 && (
                    <Button
                      type="primary"
                      block
                      icon={<CreditCardOutlined />}
                      style={{ marginTop: 12 }}
                      onClick={() => navigate(`/my-rentals/${id}/pay?type=remaining`)}
                    >
                      Доплатить {(rental.remainingAmount ?? 0).toFixed(2)} Br
                    </Button>
                  )}

                <TransactionList rentalId={id!} />
              </Card>
            )}
          </div>
        </Col>
      </Row>

      <Modal
        title="Завершение аренды"
        open={isEndModalOpen}
        onCancel={() => setIsEndModalOpen(false)}
        onOk={handleEndRental}
        confirmLoading={endMutation.isPending}
        okText="Завершить"
        cancelText="Отмена"
        okButtonProps={{ style: { background: '#22c55e', borderColor: '#22c55e' } }}
        destroyOnClose
      >
        <Form
          form={endForm}
          layout="vertical"
          style={{ marginTop: 16 }}
          initialValues={{
            returnDate: dayjs(),
            mileage: undefined,
            fuelLevel: undefined,
            penaltyAmount: 0,
            damageDescription: null,
          }}
        >
          <Form.Item
            name="returnDate"
            label="Дата возврата"
            rules={[{ required: true, message: 'Укажите дату возврата' }]}
          >
            <DatePicker
              showTime
              style={{ width: '100%' }}
              disabledDate={(d) => d && d.isAfter(dayjs())}
            />
          </Form.Item>

          <Form.Item
            name="mileage"
            label="Пробег (км)"
            rules={[{ required: true, message: 'Укажите пробег' }]}
          >
            <InputNumber min={0} style={{ width: '100%' }} placeholder="Текущий пробег" />
          </Form.Item>

          <Form.Item
            name="fuelLevel"
            label="Уровень топлива (%)"
            rules={[{ required: true, message: 'Укажите уровень топлива' }]}
          >
            <InputNumber min={0} max={100} style={{ width: '100%' }} placeholder="0-100" />
          </Form.Item>

          <Form.Item
            name="penaltyAmount"
            label="Штраф (Br)"
          >
            <InputNumber min={0} style={{ width: '100%' }} placeholder="0" />
          </Form.Item>

          <Form.Item
            name="damageDescription"
            label="Описание повреждений"
          >
            <Input.TextArea rows={3} placeholder="Описание повреждений (если есть)" />
          </Form.Item>
        </Form>
      </Modal>

      <Modal
        title="Продление аренды"
        open={isRenewModalOpen}
        onCancel={() => setIsRenewModalOpen(false)}
        onOk={handleRenew}
        confirmLoading={renewMutation.isPending}
        okText="Продлить"
        cancelText="Отмена"
        destroyOnClose
      >
        <Form
          form={renewForm}
          layout="vertical"
          style={{ marginTop: 16 }}
        >
          {(rental.fineOutstanding ?? 0) > 0 && (
            <Alert
              type="error"
              showIcon
              message={`Непогашенный штраф: ${(rental.fineOutstanding ?? 0).toFixed(2)} Br`}
              description="Сначала оплатите штраф, чтобы продлить аренду."
              style={{ marginBottom: 12 }}
            />
          )}
          {(rental.remainingAmount ?? 0) > 0 && (
            <Alert
              type="warning"
              showIcon
              message={`К оплате: ${(rental.remainingAmount ?? 0).toFixed(2)} Br`}
              description="Сначала погасите задолженность."
              style={{ marginBottom: 12 }}
            />
          )}
          <Form.Item
            name="newDate"
            label="Новая дата окончания"
            rules={[{ required: true, message: 'Укажите дату окончания' }]}
          >
            <DatePicker
              showTime
              style={{ width: '100%' }}
              disabledDate={(d) => d && d.isBefore(dayjs(rental.endDate))}
            />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}

const transactionTypeLabels: Record<PaymentType, { label: string; color: string }> = {
  Deposit: { label: 'Депозит', color: '#3b82f6' },
  FullPayment: { label: 'Полная оплата', color: '#22c55e' },
  Fine: { label: 'Штраф', color: '#ef4444' },
  Additional: { label: 'Доплата', color: '#f97316' },
  DepositRefund: { label: 'Возврат депозита', color: '#888' },
  FineRefund: { label: 'Возврат штрафа', color: '#888' },
};

const transactionStatusLabels: Record<TransactionStatus, { label: string; color: string }> = {
  Pending: { label: 'В обработке', color: '#f97316' },
  Success: { label: 'Успешно', color: '#22c55e' },
  Failed: { label: 'Ошибка', color: '#ef4444' },
};

function TransactionList({ rentalId }: { rentalId: string }) {
  const { data: transactions, isLoading } = useQuery({
    queryKey: ['payment-transactions', rentalId],
    queryFn: () => paymentApi.getTransactions(rentalId),
    enabled: !!rentalId,
    refetchInterval: 5000,
  });

  if (isLoading) {
    return <Spin size="small" style={{ display: 'block', margin: '12px auto' }} />;
  }

  if (!transactions || transactions.length === 0) {
    return null;
  }

  return (
    <div style={{ marginTop: 16 }}>
      <Divider style={{ borderColor: 'rgba(255,255,255,0.06)', margin: '8px 0 12px' }} />
      <Collapse
        ghost
        size="small"
        items={[{
          key: 'transactions',
          label: (
            <span style={{ color: '#fff', display: 'flex', alignItems: 'center', gap: 8 }}>
              <HistoryOutlined />
              История транзакций ({transactions.length})
            </span>
          ),
          children: (
            <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
              {transactions.map((t) => {
                const typeInfo = transactionTypeLabels[t.type] ?? { label: t.type, color: '#888' };
                const statusInfo = transactionStatusLabels[t.status] ?? { label: t.status, color: '#888' };
                return (
                  <div
                    key={t.id}
                    style={{
                      padding: 10,
                      background: 'rgba(255,255,255,0.03)',
                      borderRadius: 6,
                      border: '1px solid rgba(255,255,255,0.06)',
                    }}
                  >
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 4 }}>
                      <Tag style={{ backgroundColor: typeInfo.color, color: '#fff', border: 'none' }}>
                        {typeInfo.label}
                      </Tag>
                      <span style={{ color: '#fff', fontWeight: 600 }}>{t.amount.toFixed(2)} Br</span>
                    </div>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <span style={{ color: '#888', fontSize: 12 }}>
                        {dayjs(t.createdAt).format('DD.MM.YYYY HH:mm')}
                      </span>
                      <Tag style={{ backgroundColor: statusInfo.color, color: '#fff', border: 'none', fontSize: 11 }}>
                        {statusInfo.label}
                      </Tag>
                    </div>
                    {t.description && (
                      <div style={{ color: '#888', fontSize: 12, marginTop: 4 }}>{t.description}</div>
                    )}
                  </div>
                );
              })}
            </div>
          ),
        }]}
      />
    </div>
  );
}
