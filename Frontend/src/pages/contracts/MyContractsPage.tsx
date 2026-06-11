import { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import {
  Table,
  Tag,
  Typography,
  Spin,
  Button,
  Space,
  Input,
  Select,
  DatePicker,
  Empty,
  message,
} from 'antd';
import {
  FileTextOutlined,
  EyeOutlined,
  DownloadOutlined,
  ArrowLeftOutlined,
} from '@ant-design/icons';
import { contractApi } from '../../api/endpoints';
import apiClient from '../../api/client';
import { useAuthStore } from '../../stores/authStore';
import type { ContractResponse, ContractStatus } from '../../types';
import dayjs, { type Dayjs } from 'dayjs';

const { Title, Text } = Typography;

const statusColors: Record<ContractStatus, string> = {
  AwaitingSignature: '#f97316',
  Active: '#22c55e',
  Ended: '#666',
  Cancelled: '#ef4444',
};

const statusLabels: Record<ContractStatus, string> = {
  AwaitingSignature: 'Ожидает подписания',
  Active: 'Активен',
  Ended: 'Завершён',
  Cancelled: 'Отменён',
};

export default function MyContractsPage() {
  const navigate = useNavigate();
  const user = useAuthStore((s) => s.user);
  const [searchText, setSearchText] = useState('');
  const [statusFilter, setStatusFilter] = useState<ContractStatus | 'all'>('all');
  const [dateRange, setDateRange] = useState<[Dayjs | null, Dayjs | null] | null>(null);

  const { data: contracts, isLoading } = useQuery({
    queryKey: ['my-contracts', user?.id],
    queryFn: () => contractApi.getAll(),
  });

  const filteredContracts = useMemo(() => {
    if (!contracts) return [];
    return contracts.filter(c => {
      if (statusFilter !== 'all' && c.status !== statusFilter) return false;
      if (searchText) {
        const q = searchText.toLowerCase();
        if (!c.car.toLowerCase().includes(q)) return false;
      }
      if (dateRange?.[0] && dateRange?.[1]) {
        const start = dayjs(c.startDate);
        const end = dayjs(c.endDate);
        if (end.isBefore(dateRange[0].startOf('day')) || start.isAfter(dateRange[1].endOf('day'))) return false;
      }
      return true;
    });
  }, [contracts, searchText, statusFilter, dateRange]);

  const openContractPdf = async (contractId: string) => {
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

  const columns = [
    {
      title: <Text style={{ color: '#888' }}>ID</Text>,
      dataIndex: 'id',
      key: 'id',
      width: 120,
      render: (v: string) => <Text style={{ color: '#ccc' }}>#{v.slice(0, 8)}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Автомобиль</Text>,
      dataIndex: 'car',
      key: 'car',
      render: (v: string) => <Text style={{ color: '#fff' }}>{v}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Период</Text>,
      key: 'period',
      render: (_: unknown, r: ContractResponse) => (
        <Text style={{ color: '#ccc' }}>
          {dayjs(r.startDate).format('DD.MM.YYYY')} — {dayjs(r.endDate).format('DD.MM.YYYY')}
        </Text>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Стоимость</Text>,
      dataIndex: 'estimatedPrice',
      key: 'price',
      render: (v: number) => <Text style={{ color: '#fff' }}>{v.toFixed(2)} Br</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Статус</Text>,
      dataIndex: 'status',
      key: 'status',
      render: (v: ContractStatus) => (
        <Tag style={{ backgroundColor: statusColors[v], color: '#fff', border: 'none' }}>
          {statusLabels[v]}
        </Tag>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Действия</Text>,
      key: 'actions',
      render: (_: unknown, record: ContractResponse) => (
        <Space>
          {record.status === 'AwaitingSignature' && (
            <Button
              type="primary"
              icon={<FileTextOutlined />}
              size="small"
              onClick={() => navigate(`/my-contracts/${record.id}/sign`)}
            >
              Подписать
            </Button>
          )}
          {record.status === 'Active' && (
            <Button
              type="link"
              icon={<EyeOutlined />}
              style={{ color: '#3b82f6' }}
              size="small"
              onClick={() => navigate(`/my-rentals/${record.rentalId}`)}
            >
              Перейти к аренде
            </Button>
          )}
          <Button
            type="link"
            icon={<DownloadOutlined />}
            style={{ color: '#888' }}
            size="small"
            onClick={() => openContractPdf(record.id)}
          >
            PDF
          </Button>
        </Space>
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
      <Button
        type="text"
        icon={<ArrowLeftOutlined />}
        onClick={() => navigate('/')}
        style={{ color: '#888', marginBottom: 16, padding: 0 }}
      >
        На главную
      </Button>

      <Title level={3} style={{ color: '#fff', marginBottom: 24 }}>
        <FileTextOutlined style={{ marginRight: 8, color: '#f97316' }} />
        Мои договоры
      </Title>

      {!contracts || contracts.length === 0 ? (
        <div
          style={{
            background: '#1a1a1a',
            borderRadius: 12,
            border: '1px solid rgba(255,255,255,0.06)',
            padding: 64,
          }}
        >
          <Empty
            image={Empty.PRESENTED_IMAGE_SIMPLE}
            description={<Text style={{ color: '#666' }}>У вас пока нет договоров</Text>}
          />
        </div>
      ) : (
        <div
          style={{
            background: '#1a1a1a',
            borderRadius: 12,
            border: '1px solid rgba(255,255,255,0.06)',
            overflow: 'hidden',
            padding: 24,
          }}
        >
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
            <Text style={{ color: '#888' }}>Найдено: {filteredContracts.length}</Text>
          </Space>

          <Table
            dataSource={filteredContracts}
            columns={columns}
            rowKey="id"
            pagination={{ pageSize: 10 }}
            scroll={{ x: 800 }}
            style={{ background: 'transparent' }}
          />
        </div>
      )}
    </div>
  );
}
