import { useEffect, useRef, useState } from 'react';
import { useParams, useNavigate, useSearchParams } from 'react-router-dom';
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
  CarOutlined,
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
import { OUTSTANDING_FINES_QUERY_KEY, useOutstandingFines } from '../../hooks/useOutstandingFines';
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
  const [searchParams, setSearchParams] = useSearchParams();
  const queryClient = useQueryClient();
  const pollCountRef = useRef(0);
  const [pollExpired, setPollExpired] = useState(false);

  const { data: rental, isLoading, refetch: refetchRental } = useQuery({
    queryKey: ['rental', id],
    queryFn: () => rentalApi.getById(id!),
    enabled: !!id,
  });

  const renterId = rental?.carRenterId;
  const { data: outstandingFinesData } = useOutstandingFines(renterId);
  const outstandingFines = outstandingFinesData?.outstandingFines ?? 0;

  useEffect(() => {
    if (searchParams.get('paid') === '1') {
      message.success('Оплата прошла успешно');
      void refetchRental();
      queryClient.invalidateQueries({ queryKey: ['payment-transactions', id] });
      queryClient.invalidateQueries({ queryKey: ['rentals'] });
      queryClient.invalidateQueries({ queryKey: OUTSTANDING_FINES_QUERY_KEY(renterId) });
      const next = new URLSearchParams(searchParams);
      next.delete('paid');
      setSearchParams(next, { replace: true });
    }
  }, [searchParams, setSearchParams, refetchRental, queryClient, id, renterId]);

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
    const newDate = values.newDate.minute(0).second(0).millisecond(0).toDate();
    renewMutation.mutate({ newDate: newDate.toISOString() });
  };

  const { hasPermission } = useAuthStore();
  const isStaff = hasPermission('EditRent');

  const requestReturnMutation = useMutation({
    mutationFn: () => rentalApi.requestReturn(id!),
    onSuccess: () => {
      message.success('Заявка на возврат отправлена');
      queryClient.invalidateQueries({ queryKey: ['rental', id] });
      queryClient.invalidateQueries({ queryKey: ['rentals'] });
    },
    onError: (err: unknown) => {
      const e = err as { response?: { data?: { message?: string } }; message?: string };
      const msg = e?.response?.data?.message ?? e?.message ?? 'Ошибка при отправке заявки';
      message.error(msg);
    },
  });

  const [isEndModalOpen, setIsEndModalOpen] = useState(false);
  const [endForm] = Form.useForm();
  const [endReturnDate, setEndReturnDate] = useState<dayjs.Dayjs | null>(null);

  const endMutation = useMutation({
    mutationFn: (data: EndRentalRequest) => rentalApi.end(id!, data),
    onSuccess: () => {
      message.success('Аренда завершена');
      queryClient.invalidateQueries({ queryKey: ['rental', id] });
      queryClient.invalidateQueries({ queryKey: ['rentals'] });
      setIsEndModalOpen(false);
    },
    onError: (err: unknown) => {
      const e = err as { response?: { data?: { message?: string } }; message?: string };
      const msg = e?.response?.data?.message ?? e?.message ?? 'Ошибка при завершении аренды';
      if (msg.includes('exceeds estimated')) {
        message.warning(msg, 6);
        queryClient.invalidateQueries({ queryKey: ['rental', id] });
        queryClient.invalidateQueries({ queryKey: ['payment-transactions', id] });
      } else {
        message.error(msg);
      }
    },
  });

  const previewFinalCostQuery = useQuery({
    queryKey: ['previewFinalCost', id, endReturnDate?.toISOString()],
    queryFn: () => rentalApi.previewFinalCost(
      id!,
      endReturnDate!.minute(0).second(0).millisecond(0).toDate().toISOString()),
    enabled: !!id && !!endReturnDate && isEndModalOpen,
    staleTime: 0,
  });

  const [isRequestReturnModalOpen, setIsRequestReturnModalOpen] = useState(false);
  const [requestReturnDate, setRequestReturnDate] = useState<dayjs.Dayjs | null>(null);

  const requestReturnPreviewQuery = useQuery({
    queryKey: ['previewFinalCost', 'requestReturn', id, requestReturnDate?.toISOString()],
    queryFn: () => rentalApi.previewFinalCost(
      id!,
      requestReturnDate!.minute(0).second(0).millisecond(0).toDate().toISOString()),
    enabled: !!id && !!requestReturnDate && isRequestReturnModalOpen,
    staleTime: 0,
  });

  const handleEndRental = async () => {
    const values = await endForm.validateFields();
    const returnDate = values.returnDate.minute(0).second(0).millisecond(0).toDate();
    endMutation.mutate({
      returnDate: returnDate.toISOString(),
      mileage: values.mileage,
      fuelLevel: values.fuelLevel / 100,
      penaltyAmount: values.penaltyAmount || 0,
      damageDescription: values.damageDescription || null,
    });
  };

  const { data: transactions } = useQuery({
    queryKey: ['payment-transactions', id],
    queryFn: () => paymentApi.getTransactions(id!),
    enabled: !!id,
    refetchInterval: 5000,
  });

  const depositTransaction = transactions?.find((t) => t.type === 'Deposit');
  const depositRefundTransaction = transactions?.find((t) => t.type === 'DepositRefund');
  const depositRefunded = depositTransaction?.isRefunded ?? !!depositRefundTransaction;

  const markDepositRefundedMutation = useMutation({
    mutationFn: ({ note }: { note?: string | null }) =>
      rentalApi.markDepositRefunded(id!, note),
    onSuccess: () => {
      message.success('Депозит помечен как возвращённый (заглушка: реальная интеграция в разработке)');
      queryClient.invalidateQueries({ queryKey: ['rental', id] });
      queryClient.invalidateQueries({ queryKey: ['payment-transactions', id] });
    },
    onError: () => message.error('Не удалось пометить возврат депозита'),
  });

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

              {statusName === 'Active' && (
                <>
                  <Divider style={{ borderColor: 'rgba(255,255,255,0.06)' }} />
                  {rental.returnRequestedAtUtc && (
                    <Alert
                      type={
                        outstandingFines > 0
                        || (rental.additionalOutstanding ?? 0) > 0
                        || (rental.remainingAmount ?? 0) > 0
                          ? 'warning'
                          : 'success'
                      }
                      showIcon
                      icon={<CarOutlined />}
                      style={{ marginBottom: 16 }}
                      message={
                        <span>
                          Заявка на возврат отправлена{' '}
                          {dayjs(rental.returnRequestedAtUtc).format('DD.MM.YYYY HH:mm')}
                        </span>
                      }
                      description={
                        outstandingFines > 0
                        || (rental.additionalOutstanding ?? 0) > 0
                        || (rental.remainingAmount ?? 0) > 0 ? (
                          <Space direction="vertical" size={4}>
                            <span>Перед завершением аренды погасите все задолженности:</span>
                            {outstandingFines > 0 && (
                              <span>• Штраф: <b>{outstandingFines.toFixed(2)} Br</b></span>
                            )}
                            {(rental.additionalOutstanding ?? 0) > 0 && (
                              <span>• Продление: <b>{(rental.additionalOutstanding ?? 0).toFixed(2)} Br</b></span>
                            )}
                            {(rental.remainingAmount ?? 0) > 0 && (
                              <span>• Остаток: <b>{(rental.remainingAmount ?? 0).toFixed(2)} Br</b></span>
                            )}
                          </Space>
                        ) : (
                          <span>Все задолженности погашены. Менеджер скоро завершит аренду, после чего депозит будет возвращён.</span>
                        )
                      }
                    />
                  )}
                  <Space wrap>
                    {isStaff ? (
                      <>
                        {!rental.returnRequestedAtUtc && (
                          <Button
                            type="default"
                            icon={<RollbackOutlined />}
                            loading={requestReturnMutation.isPending}
                            onClick={() => {
                              setRequestReturnDate(dayjs());
                              setIsRequestReturnModalOpen(true);
                            }}
                          >
                            Подать заявку на возврат
                          </Button>
                        )}
                        {rental.returnRequestedAtUtc && (
                          <Button
                            type="primary"
                            icon={<RollbackOutlined />}
                            style={{ background: '#22c55e', borderColor: '#22c55e' }}
                            disabled={
                              outstandingFines > 0
                              || (rental.additionalOutstanding ?? 0) > 0
                              || (rental.remainingAmount ?? 0) > 0
                            }
                            title={
                              outstandingFines > 0
                              || (rental.additionalOutstanding ?? 0) > 0
                              || (rental.remainingAmount ?? 0) > 0
                                ? 'Клиент должен погасить все задолженности перед завершением аренды'
                                : undefined
                            }
                            onClick={() => {
                              endForm.resetFields();
                              setEndReturnDate(dayjs());
                              setIsEndModalOpen(true);
                            }}
                          >
                            Завершить аренду
                          </Button>
                        )}
                      </>
                    ) : (
                      !rental.returnRequestedAtUtc && (
                        <Button
                          type="primary"
                           icon={<RollbackOutlined />}
                           loading={requestReturnMutation.isPending}
                           onClick={() => {
                             setRequestReturnDate(dayjs());
                             setIsRequestReturnModalOpen(true);
                           }}
                        >
                          Вернуть авто
                        </Button>
                      )
                    )}
                    {!rental.returnRequestedAtUtc && (
                      <Button
                        icon={<HistoryOutlined />}
                        onClick={() => {
                          renewForm.resetFields();
                          renewForm.setFieldsValue({
                            newDate: dayjs(rental.endDate).add(1, 'day'),
                          });
                          setIsRenewModalOpen(true);
                        }}
                        disabled={outstandingFines > 0 || (rental.remainingAmount ?? 0) > 0}
                        title={outstandingFines > 0
                          ? 'Сначала оплатите штраф'
                          : (rental.remainingAmount ?? 0) > 0
                            ? 'Сначала погасите задолженность'
                            : 'Продлить аренду'}
                      >
                        Продлить аренду
                      </Button>
                    )}
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
                styles={{ content: { color: '#fff' } }}
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
                <div style={{ marginBottom: 8, display: 'flex', gap: 8, alignItems: 'center', flexWrap: 'wrap' }}>
                  <Tag style={{ backgroundColor: paymentStatusColors[rental.paymentStatus] || '#888', color: '#fff', border: 'none' }}>
                    {paymentStatusLabels[rental.paymentStatus] || rental.paymentStatus}
                  </Tag>
                  {rental.paymentStatus === 'Paid' && (rental.remainingAmount ?? 0) <= 0 && (
                    <Tag style={{ backgroundColor: '#22c55e', color: '#fff', border: 'none' }}>
                      <CheckCircleOutlined style={{ marginRight: 4 }} />
                      Оплачено полностью
                    </Tag>
                  )}
                  {outstandingFines > 0 ? (
                    <Tag style={{ backgroundColor: '#ef4444', color: '#fff', border: 'none' }}>
                      <WarningOutlined style={{ marginRight: 4 }} />
                      Штраф: {outstandingFines.toFixed(2)} Br
                    </Tag>
                  ) : (
                    <Tag style={{ backgroundColor: '#22c55e', color: '#fff', border: 'none' }}>
                      <CheckCircleOutlined style={{ marginRight: 4 }} />
                      Штрафов нет
                    </Tag>
                  )}
                  {depositRefunded && (
                    <Tag style={{ backgroundColor: '#22c55e', color: '#fff', border: 'none' }}>
                      <RollbackOutlined style={{ marginRight: 4 }} />
                      Депозит возвращён
                    </Tag>
                  )}
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

                {outstandingFines > 0 && (
                  <>
                    <Divider style={{ borderColor: 'rgba(255,255,255,0.06)', margin: '12px 0' }} />
                    <Alert
                      type="error"
                      showIcon
                      icon={<WarningOutlined />}
                      message={
                        <span>
                          Непогашенный штраф: <b>{outstandingFines.toFixed(2)} Br</b>
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
                    {outstandingFines === 0 && <Divider style={{ borderColor: 'rgba(255,255,255,0.06)', margin: '12px 0' }} />}
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
                  && outstandingFines === 0
                  && (rental.additionalOutstanding ?? 0) === 0
                  && (statusName === 'Active' || statusName === 'Completed') && (
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

                {isStaff
                  && (statusName === 'Completed' || statusName === 'Cancelled')
                  && !rental.depositRefundedAt
                  && (rental.depositAmount ?? 0) > 0 && (
                    <Button
                      block
                      icon={<RollbackOutlined />}
                      style={{ marginTop: 12 }}
                      loading={markDepositRefundedMutation.isPending}
                      onClick={() => {
                        let noteValue = '';
                        Modal.confirm({
                          title: 'Вернуть депозит?',
                          content: (
                            <div>
                              <div style={{ marginBottom: 12 }}>
                                Будет возвращено <b>{(rental.depositAmount ?? 0).toFixed(2)} Br</b>.
                                Реальная интеграция с платёжным провайдером пока не подключена (заглушка).
                              </div>
                              <Input.TextArea
                                rows={3}
                                placeholder="Комментарий (опционально)"
                                onChange={(e) => { noteValue = e.target.value; }}
                              />
                            </div>
                          ),
                          okText: 'Подтвердить',
                          okType: 'primary',
                          okButtonProps: { style: { background: '#22c55e', borderColor: '#22c55e' } },
                          cancelText: 'Отмена',
                          onOk: () => markDepositRefundedMutation.mutate({ note: noteValue || null }),
                        });
                      }}
                    >
                      Вернуть депозит {(rental.depositAmount ?? 0).toFixed(2)} Br
                    </Button>
                  )}

                <TransactionList rentalId={id!} />
              </Card>
            )}
          </div>
        </Col>
      </Row>

      <Modal
        title={isStaff ? 'Подать заявку на возврат?' : 'Вернуть авто?'}
        open={isRequestReturnModalOpen}
        onCancel={() => setIsRequestReturnModalOpen(false)}
        onOk={() => {
          setIsRequestReturnModalOpen(false);
          requestReturnMutation.mutate();
        }}
        confirmLoading={requestReturnMutation.isPending}
        okText="Отправить заявку"
        okButtonProps={{ style: { background: '#22c55e', borderColor: '#22c55e' } }}
        cancelText="Отмена"
      >
        <Space direction="vertical" size={8} style={{ marginTop: 8 }}>
          <span>
            {isStaff
              ? 'Будет создана заявка на возврат. После её подачи станет доступно завершение аренды.'
              : 'Менеджер свяжется с вами для проверки авто. После этого аренда будет завершена.'}
          </span>
          {requestReturnPreviewQuery.isLoading && <Spin size="small" />}
          {requestReturnPreviewQuery.data && requestReturnPreviewQuery.data.refundAmount > 0 && (
            <span>
              К возврату (с учётом депозита): <b>{requestReturnPreviewQuery.data.refundAmount.toFixed(2)} {requestReturnPreviewQuery.data.currency}</b>
            </span>
          )}
          {requestReturnPreviewQuery.data && requestReturnPreviewQuery.data.diff > 0 && (
            <span>
              Доплата: <b>{requestReturnPreviewQuery.data.diff.toFixed(2)} {requestReturnPreviewQuery.data.currency}</b>
            </span>
          )}
          {requestReturnPreviewQuery.data && requestReturnPreviewQuery.data.diff === 0 && (
            <span>
              Стоимость совпадает с предоплатой. К возврату депозит: <b>{requestReturnPreviewQuery.data.depositAmount.toFixed(2)} {requestReturnPreviewQuery.data.currency}</b>
            </span>
          )}
          <Alert
            type="warning"
            showIcon
            message="Если вы не вернёте машину в указанный срок, будут начисляться штрафы за просрочку."
          />
        </Space>
      </Modal>

      <Modal
        title="Завершение аренды"
        open={isEndModalOpen}
        onCancel={() => setIsEndModalOpen(false)}
        onOk={handleEndRental}
        confirmLoading={endMutation.isPending}
        okText="Завершить"
        cancelText="Отмена"
        okButtonProps={{ style: { background: '#22c55e', borderColor: '#22c55e' } }}
        destroyOnHidden
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
          onValuesChange={(_changed, all) => {
            if ('returnDate' in all) setEndReturnDate(all.returnDate ?? null);
          }}
        >
          <Form.Item
            name="returnDate"
            label="Дата возврата"
            rules={[{ required: true, message: 'Укажите дату возврата' }]}
          >
            <DatePicker
              showTime={{ format: 'HH:mm', defaultValue: dayjs().startOf('hour') }}
              format="DD.MM.YYYY HH:mm"
              style={{ width: '100%' }}
              disabledDate={(d) => d && d.isAfter(dayjs())}
              onChange={(v) => {
                if (v) {
                  endForm.setFieldValue('returnDate', v.minute(0).second(0).millisecond(0));
                  setEndReturnDate(v.minute(0).second(0).millisecond(0));
                }
              }}
            />
          </Form.Item>

          {previewFinalCostQuery.data && (
            <Alert
              type={previewFinalCostQuery.data.diff > 0 ? 'warning' : 'success'}
              showIcon
              style={{ marginBottom: 12 }}
              message={
                previewFinalCostQuery.data.diff > 0
                  ? `К доплате: ${previewFinalCostQuery.data.diff.toFixed(2)} ${previewFinalCostQuery.data.currency}`
                  : previewFinalCostQuery.data.refundAmount > 0
                    ? `К возврату клиенту: ${previewFinalCostQuery.data.refundAmount.toFixed(2)} ${previewFinalCostQuery.data.currency} (включая депозит и переплату)`
                    : 'Стоимость совпадает с предоплатой'
              }
              description={
                previewFinalCostQuery.data.refundAmount > 0
                  ? `Деньги поступят на карту, с которой производилась оплата, в течение 3-5 рабочих дней.`
                  : `Предварительная стоимость: ${previewFinalCostQuery.data.finalCost.toFixed(2)} ${previewFinalCostQuery.data.currency} (оценка: ${previewFinalCostQuery.data.estimated.toFixed(2)})`
              }
            />
          )}

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
        destroyOnHidden
      >
        <Form
          form={renewForm}
          layout="vertical"
          style={{ marginTop: 16 }}
        >
          {outstandingFines > 0 && (
            <Alert
              type="error"
              showIcon
              message={`Непогашенный штраф: ${outstandingFines.toFixed(2)} Br`}
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
              showTime={{ format: 'HH:mm', defaultValue: dayjs(rental.endDate).startOf('hour') }}
              format="DD.MM.YYYY HH:mm"
              style={{ width: '100%' }}
              disabledDate={(d) => d && d.isBefore(dayjs(rental.endDate))}
              onChange={(v) => {
                if (v) {
                  renewForm.setFieldValue('newDate', v.minute(0).second(0).millisecond(0));
                }
              }}
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
