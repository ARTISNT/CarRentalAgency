import { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Table, Tag, Typography, Spin, Button, Space, Input, Select, DatePicker, Empty } from 'antd';
import { EyeOutlined, PlusOutlined } from '@ant-design/icons';
import { rentalApi } from '../../api/endpoints';
import type { RentActivityStatus, RentalListItem } from '../../types';
import dayjs, { type Dayjs } from 'dayjs';

const { Title, Text } = Typography;

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

export default function MyRentalsPage() {
  const navigate = useNavigate();
  const [searchText, setSearchText] = useState('');
  const [statusFilter, setStatusFilter] = useState<RentActivityStatus | 'all'>('all');
  const [dateRange, setDateRange] = useState<[Dayjs | null, Dayjs | null] | null>(null);

  const { data: rentals, isLoading } = useQuery({
    queryKey: ['rentals'],
    queryFn: () => rentalApi.getAll(),
  });

  const filteredRentals = useMemo(() => {
    if (!rentals) return [];
    return rentals.filter(r => {
      const statusName = r.activityStatus.name as RentActivityStatus;
      if (statusFilter !== 'all' && statusName !== statusFilter) return false;
      if (searchText) {
        const q = searchText.toLowerCase();
        if (!r.car.toLowerCase().includes(q)) return false;
      }
      if (dateRange?.[0] && dateRange?.[1]) {
        const start = dayjs(r.startDate);
        if (start.isBefore(dateRange[0].startOf('day')) || start.isAfter(dateRange[1].endOf('day'))) return false;
      }
      return true;
    });
  }, [rentals, searchText, statusFilter, dateRange]);

  const columns = [
    {
      title: <Text style={{ color: '#888' }}>Автомобиль</Text>,
      dataIndex: 'car',
      key: 'car',
      render: (v: string) => (
        <Text style={{ color: '#fff' }}>{v}</Text>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Начало</Text>,
      dataIndex: 'startDate',
      key: 'start',
      render: (v: string) => <Text style={{ color: '#ccc' }}>{dayjs(v).format('DD.MM.YYYY')}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Окончание</Text>,
      dataIndex: 'endDate',
      key: 'end',
      render: (v: string) => <Text style={{ color: '#ccc' }}>{dayjs(v).format('DD.MM.YYYY')}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Статус</Text>,
      dataIndex: 'activityStatus',
      key: 'status',
      render: (v: { name: string; id: number }) => (
        <Tag style={{ backgroundColor: statusColors[v.name as RentActivityStatus], color: '#fff', border: 'none' }}>
          {statusLabels[v.name as RentActivityStatus]}
        </Tag>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Действия</Text>,
      key: 'actions',
      render: (_: unknown, record: RentalListItem) => (
        <Button
          type="link"
          icon={<EyeOutlined />}
          style={{ color: '#f97316' }}
          onClick={() => navigate(`/my-rentals/${record.id}`)}
        >
          Подробнее
        </Button>
      ),
    },
  ];

  if (isLoading) {
    return (
      <div style={{ textAlign: 'center', padding: 120 }}>
        <Spin size="large" />
      </div>
    );
  }

  return (
    <div style={{ maxWidth: 1200, margin: '0 auto', padding: '32px' }}>
      <Space style={{ justifyContent: 'space-between', width: '100%', marginBottom: 24 }}>
        <div>
          <Title level={2} style={{ color: '#fff', margin: 0 }}>Мои аренды</Title>
          <Text style={{ color: '#888' }}>История и текущие аренды</Text>
        </div>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => navigate('/cars')}
          style={{ height: 40 }}
        >
          Новая аренда
        </Button>
      </Space>

      {rentals && rentals.length > 0 ? (
        <div style={{ background: '#1a1a1a', borderRadius: 12, border: '1px solid rgba(255,255,255,0.06)', overflow: 'hidden' }}>
          <Space wrap style={{ marginBottom: 16, width: '100%', justifyContent: 'space-between' }}>
            <Space wrap>
              <Input.Search
                placeholder="Поиск по автомобилю..."
                value={searchText}
                onChange={e => setSearchText(e.target.value)}
                onSearch={setSearchText}
                allowClear
                style={{ width: 280 }}
              />
              <Select
                value={statusFilter}
                onChange={v => setStatusFilter(v)}
                style={{ width: 180 }}
                options={[
                  { value: 'all', label: 'Все статусы' },
                  ...Object.entries(statusLabels).map(([value, label]) => ({ value, label })),
                ]}
              />
              <DatePicker.RangePicker
                value={dateRange}
                onChange={dates => setDateRange(dates as [Dayjs | null, Dayjs | null] | null)}
                placeholder={['Дата с', 'Дата по']}
                style={{ width: 240 }}
              />
              <Button onClick={() => { setSearchText(''); setStatusFilter('all'); setDateRange(null); }}>
                Сбросить
              </Button>
            </Space>
            <Text style={{ color: '#888' }}>Найдено: {filteredRentals.length}</Text>
          </Space>

          <Table
            dataSource={filteredRentals}
            columns={columns}
            rowKey="id"
            pagination={{ pageSize: 10 }}
            style={{ background: 'transparent' }}
          />
        </div>
      ) : (
        <div style={{ textAlign: 'center', padding: 80, background: '#1a1a1a', borderRadius: 12, border: '1px solid rgba(255,255,255,0.06)' }}>
          <Empty description={<Text style={{ color: '#888' }}>У вас пока нет аренд</Text>}>
            <Button type="primary" onClick={() => navigate('/cars')}>
              Выбрать автомобиль
            </Button>
          </Empty>
        </div>
      )}
    </div>
  );
}
