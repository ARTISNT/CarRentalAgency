import { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  Button,
  DatePicker,
  Form,
  Input,
  InputNumber,
  Modal,
  Select,
  Space,
  Spin,
  Table,
  Tag,
  Typography,
  message,
} from 'antd';
import type { TablePaginationConfig } from 'antd';
import type { FilterValue, SorterResult } from 'antd/es/table/interface';
import { CloseCircleOutlined, EyeOutlined, RollbackOutlined, CarOutlined, CheckCircleOutlined } from '@ant-design/icons';
import { rentalApi, userApi } from '../../api/endpoints';
import { useAuthStore } from '../../stores/authStore';
import type { EndRentalRequest, RentalListItem, RentActivityStatus } from '../../types';
import dayjs, { type Dayjs } from 'dayjs';

const { Title, Text } = Typography;
const { RangePicker } = DatePicker;

const statusColors: Record<RentActivityStatus, string> = {
  AwaitingConfirmation: '#f97316',
  Scheduled: '#eab308',
  Active: '#3b82f6',
  Completed: '#22c55e',
  Cancelled: '#ef4444',
};

const statusLabels: Record<RentActivityStatus, string> = {
  AwaitingConfirmation: 'Ожидает',
  Scheduled: 'Запланирована',
  Active: 'Активна',
  Completed: 'Завершена',
  Cancelled: 'Отменена',
};

type SortField = 'startDate' | 'endDate' | 'totalCost';
type SortOrder = 'ascend' | 'descend' | null;

const normalizePhone = (s: string) => s.replace(/\D/g, '');

export default function AdminRentalsPage() {
  const navigate = useNavigate();

  const { hasPermission, hasRole } = useAuthStore();
  const queryClient = useQueryClient();

  const [isEndModalOpen, setIsEndModalOpen] = useState(false);
  const [endRentalId, setEndRentalId] = useState<string | null>(null);
  const [endForm] = Form.useForm();

  const isStaff = hasRole(['Manager', 'Admin']);
  const canUseFilters = isStaff;

  const [surnameInput, setSurnameInput] = useState('');
  const [nameInput, setNameInput] = useState('');
  const [phoneInput, setPhoneInput] = useState('');
  const [statusFilter, setStatusFilter] = useState<RentActivityStatus | 'all'>('all');
  const [dateRange, setDateRange] = useState<[Dayjs | null, Dayjs | null] | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [sortField, setSortField] = useState<SortField>('startDate');
  const [sortOrder, setSortOrder] = useState<SortOrder>('descend');

  useEffect(() => {
    setCurrentPage(1);
  }, [surnameInput, nameInput, phoneInput, statusFilter, dateRange]);

  const endMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: EndRentalRequest }) =>
      rentalApi.end(id, data),
    onSuccess: () => {
      message.success('Аренда завершена');
      queryClient.invalidateQueries({ queryKey: ['rentals'] });
      setIsEndModalOpen(false);
      setEndRentalId(null);
    },
    onError: () => message.error('Ошибка при завершении аренды'),
  });

  const cancelMutation = useMutation({
    mutationFn: (id: string) => rentalApi.cancel(id, { reason: null }),
    onSuccess: () => {
      message.success('Аренда отменена');
      queryClient.invalidateQueries({ queryKey: ['rentals'] });
    },
    onError: () => message.error('Ошибка при отмене'),
  });

  const markDepositRefundedMutation = useMutation({
    mutationFn: ({ id, note }: { id: string; note?: string | null }) =>
      rentalApi.markDepositRefunded(id, note),
    onSuccess: () => {
      message.success('Депозит помечен как возвращённый (заглушка: реальная интеграция в разработке)');
      queryClient.invalidateQueries({ queryKey: ['rentals'] });
    },
    onError: () => message.error('Не удалось пометить возврат депозита'),
  });

  const handleEndRental = async () => {
    const values = await endForm.validateFields();
    endMutation.mutate({
      id: endRentalId!,
      data: {
        returnDate: values.returnDate.toISOString(),
        mileage: values.mileage,
        fuelLevel: values.fuelLevel / 100,
        penaltyAmount: values.penaltyAmount || 0,
        damageDescription: values.damageDescription || null,
      },
    });
  };

  const { data: rentals, isLoading } = useQuery({
    queryKey: ['rentals'],
    queryFn: () => rentalApi.getAll(),
  });

  const phoneQuery = normalizePhone(phoneInput);
  const { data: allUsers } = useQuery({
    queryKey: ['users-all-for-phone-search'],
    queryFn: () => userApi.getAll(),
    enabled: canUseFilters && phoneQuery.length > 0,
  });

  const matchingRenterIds = useMemo(() => {
    if (!phoneQuery || !allUsers) return null;
    const set = new Set<string>();
    for (const u of allUsers) {
      if (normalizePhone(u.phoneNumber).includes(phoneQuery)) {
        set.add(u.id);
      }
    }
    return set;
  }, [allUsers, phoneQuery]);

  const filteredRentals = useMemo(() => {
    if (!rentals) return [];
    if (!canUseFilters) return rentals;

    let list = rentals;

    const qSurname = surnameInput.trim().toLowerCase();
    const qName = nameInput.trim().toLowerCase();

    if (qSurname || qName || (phoneQuery && matchingRenterIds)) {
      list = list.filter((r) => {
        const renterLower = (r.renter ?? '').toLowerCase();
        if (qSurname && !renterLower.includes(qSurname)) return false;
        if (qName && !renterLower.includes(qName)) return false;
        if (phoneQuery && matchingRenterIds && !matchingRenterIds.has(r.renterId)) return false;
        return true;
      });
    }

    if (statusFilter !== 'all') {
      list = list.filter((r) => r.activityStatus.name === statusFilter);
    }

    if (dateRange && (dateRange[0] || dateRange[1])) {
      const from = dateRange[0]?.startOf('day').toISOString();
      const to = dateRange[1]?.endOf('day').toISOString();
      list = list.filter((r) => {
        const start = r.startDate;
        if (from && start < from) return false;
        if (to && start > to) return false;
        return true;
      });
    }

    if (sortField && sortOrder) {
      const dir = sortOrder === 'ascend' ? 1 : -1;
      list = [...list].sort((a, b) => {
        const av = a[sortField] ?? '';
        const bv = b[sortField] ?? '';
        if (av < bv) return -1 * dir;
        if (av > bv) return 1 * dir;
        return 0;
      });
    }

    return list;
  }, [rentals, canUseFilters, surnameInput, nameInput, phoneQuery, matchingRenterIds, statusFilter, dateRange, sortField, sortOrder]);

  const pagedRentals = useMemo(() => {
    const start = (currentPage - 1) * pageSize;
    return filteredRentals.slice(start, start + pageSize);
  }, [filteredRentals, currentPage, pageSize]);

  const handleTableChange = (
    pagination: TablePaginationConfig,
    _filters: Record<string, FilterValue | null>,
    sorter: SorterResult<RentalListItem> | SorterResult<RentalListItem>[],
  ) => {
    setCurrentPage(pagination.current ?? 1);
    setPageSize(pagination.pageSize ?? 10);

    const single = Array.isArray(sorter) ? sorter[0] : sorter;
    if (single && (single.field === 'startDate' || single.field === 'endDate' || single.field === 'totalCost')) {
      setSortField(single.field as SortField);
      setSortOrder((single.order ?? null) as SortOrder);
    }
  };

  const handleResetFilters = () => {
    setSurnameInput('');
    setNameInput('');
    setPhoneInput('');
    setStatusFilter('all');
    setDateRange(null);
    setCurrentPage(1);
  };

  const columns = [
    {
      title: <Text style={{ color: '#888' }}>ID</Text>,
      dataIndex: 'id',
      key: 'id',
      render: (v: string) => <Text style={{ color: '#ccc' }}>{v.slice(0, 8)}...</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Арендатор</Text>,
      key: 'renter',
      sorter: false,
      render: (_: unknown, record: RentalListItem) => (
        <div>
          <Text style={{ color: '#fff', display: 'block' }}>{record.renter}</Text>
          <Text style={{ color: '#888', fontSize: 12 }}>{record.phoneNumber}</Text>
        </div>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Автомобиль</Text>,
      key: 'car',
      render: (_: unknown, record: RentalListItem) => (
        <Text style={{ color: '#fff' }}>{record.car}</Text>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Начало</Text>,
      dataIndex: 'startDate',
      key: 'start',
      sorter: canUseFilters,
      sortOrder: sortField === 'startDate' ? sortOrder : null,
      render: (v: string) => <Text style={{ color: '#ccc' }}>{dayjs(v).format('DD.MM.YYYY')}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Окончание</Text>,
      dataIndex: 'endDate',
      key: 'end',
      sorter: canUseFilters,
      sortOrder: sortField === 'endDate' ? sortOrder : null,
      render: (v: string) => <Text style={{ color: '#ccc' }}>{dayjs(v).format('DD.MM.YYYY')}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Статус</Text>,
      dataIndex: 'activityStatus',
      key: 'status',
      render: (v: { name: string; id: number }, record: RentalListItem) => (
        <Space size={4} wrap>
          <Tag style={{ backgroundColor: statusColors[v.name as RentActivityStatus], color: '#fff', border: 'none' }}>{statusLabels[v.name as RentActivityStatus]}</Tag>
          {record.returnRequestedAtUtc && (
            <Tag
              icon={<CarOutlined />}
              style={{ backgroundColor: '#f97316', color: '#fff', border: 'none' }}
            >
              Заявка на возврат
            </Tag>
          )}
          {record.depositRefundedAt && (
            <Tag
              icon={<CheckCircleOutlined />}
              style={{ backgroundColor: '#22c55e', color: '#fff', border: 'none' }}
            >
              Депозит возвращён
            </Tag>
          )}
        </Space>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Стоимость</Text>,
      dataIndex: 'totalCost',
      key: 'cost',
      sorter: canUseFilters,
      sortOrder: sortField === 'totalCost' ? sortOrder : null,
      render: (v: number) => <Text style={{ color: '#ccc' }}>{v.toFixed(2)} Br</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Действия</Text>,
      key: 'actions',
      render: (_: unknown, record: RentalListItem) => (
        <Space>
          <Button
            type="link"
            icon={<EyeOutlined />}
            style={{ color: '#f97316' }}
            onClick={() => navigate(`/my-rentals/${record.id}`)}
          >
            Детали
          </Button>
          {record.activityStatus.name === 'Active' && hasPermission('EditRent') && (
            <Button
              type="link"
              icon={<RollbackOutlined />}
              style={{ color: record.returnRequestedAtUtc ? '#22c55e' : '#666' }}
              disabled={!record.returnRequestedAtUtc}
              title={!record.returnRequestedAtUtc ? 'Дождитесь заявки на возврат от клиента' : undefined}
              onClick={() => {
                setEndRentalId(record.id);
                endForm.resetFields();
                setIsEndModalOpen(true);
              }}
            >
              Завершить
            </Button>
          )}
          {(record.activityStatus.name === 'AwaitingConfirmation' || record.activityStatus.name === 'Scheduled') && hasPermission('EditRent') && (
            <Button
              type="link"
              icon={<CloseCircleOutlined />}
              style={{ color: '#ef4444' }}
              onClick={() =>
                Modal.confirm({
                  title: 'Отменить аренду?',
                  content: record.activityStatus.name === 'Scheduled'
                    ? 'Депозит будет возвращён'
                    : 'Вы уверены?',
                  onOk: () => cancelMutation.mutate(record.id),
                })
              }
            >
              Отменить
            </Button>
          )}
          {(record.activityStatus.name === 'Completed' || record.activityStatus.name === 'Cancelled')
            && !record.depositRefundedAt
            && hasPermission('EditRent') && (
              <Button
                type="link"
                icon={<CheckCircleOutlined />}
                style={{ color: '#22c55e' }}
                loading={markDepositRefundedMutation.isPending && markDepositRefundedMutation.variables?.id === record.id}
                onClick={() => {
                  let noteValue = '';
                  Modal.confirm({
                    title: 'Отметить возврат депозита?',
                    content: (
                      <div>
                        <div style={{ marginBottom: 12 }}>
                          Будет создана отметка о ручном возврате депозита. Реальная интеграция с платёжным провайдером пока не подключена (заглушка).
                        </div>
                        <Input.TextArea
                          rows={3}
                          placeholder="Комментарий (опционально)"
                          onChange={(e) => { noteValue = e.target.value; }}
                        />
                      </div>
                    ),
                    okText: 'Подтвердить',
                    cancelText: 'Отмена',
                    onOk: () => markDepositRefundedMutation.mutate({ id: record.id, note: noteValue || null }),
                  });
                }}
              >
                Отметить возврат депозита
              </Button>
            )}
        </Space>
      ),
    },
  ];

  if (isLoading) {
    return (
      <div style={{ textAlign: 'center', padding: 80 }}>
        <Spin size="large" />
      </div>
    );
  }

  return (
    <div style={{ maxWidth: 1200, margin: '0 auto', padding: '32px' }}>
      <Title level={2} style={{ color: '#fff', marginBottom: 24 }}>Управление арендами</Title>

      {canUseFilters && (
        <div
          style={{
            background: '#1a1a1a',
            borderRadius: 12,
            border: '1px solid rgba(255,255,255,0.06)',
            padding: 16,
            marginBottom: 16,
          }}
        >
          <Space wrap size={12} style={{ width: '100%' }}>
            <Input
              allowClear
              placeholder="Фамилия"
              value={surnameInput}
              onChange={(e) => setSurnameInput(e.target.value)}
              style={{ width: 180 }}
            />
            <Input
              allowClear
              placeholder="Имя"
              value={nameInput}
              onChange={(e) => setNameInput(e.target.value)}
              style={{ width: 160 }}
            />
            <Input
              allowClear
              placeholder="Телефон"
              value={phoneInput}
              onChange={(e) => setPhoneInput(e.target.value)}
              style={{ width: 180 }}
            />
            <Select
              value={statusFilter}
              onChange={(v) => setStatusFilter(v)}
              style={{ width: 200 }}
              options={[
                { value: 'all', label: 'Все статусы' },
                ...(Object.keys(statusLabels) as RentActivityStatus[]).map((s) => ({
                  value: s,
                  label: statusLabels[s],
                })),
              ]}
            />
            <RangePicker
              value={dateRange ?? undefined}
              onChange={(values) => setDateRange(values as [Dayjs | null, Dayjs | null] | null)}
              placeholder={['Дата начала от', 'Дата начала до']}
              style={{ width: 320 }}
            />
            <Button onClick={handleResetFilters}>Сбросить</Button>
            <Text style={{ color: '#888' }}>
              Найдено: {filteredRentals.length}
            </Text>
          </Space>
        </div>
      )}

      <div style={{ background: '#1a1a1a', borderRadius: 12, border: '1px solid rgba(255,255,255,0.06)', overflow: 'hidden' }}>
        <Table
          dataSource={pagedRentals}
          columns={columns}
          rowKey="id"
          onChange={handleTableChange}
          pagination={{
            current: currentPage,
            pageSize,
            total: filteredRentals.length,
            showSizeChanger: true,
            pageSizeOptions: ['10', '20', '50'],
            showTotal: (total) => `Всего: ${total}`,
          }}
          scroll={{ x: 1000 }}
          style={{ background: 'transparent' }}
        />
      </div>

      <Modal
        title="Завершение аренды"
        open={isEndModalOpen}
        onCancel={() => {
          setIsEndModalOpen(false);
          setEndRentalId(null);
        }}
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
    </div>
  );
}
