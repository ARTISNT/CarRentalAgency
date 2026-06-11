import { useMemo, useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table,
  Tag,
  Typography,
  Spin,
  Button,
  Space,
  Modal,
  Input,
  DatePicker,
  Select,
  message,
  Descriptions,
} from 'antd';
import {
  EyeOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
  DownloadOutlined,
  SearchOutlined,
} from '@ant-design/icons';
import { contractApi } from '../../api/endpoints';
import apiClient from '../../api/client';
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
  AwaitingSignature: 'Ожидает',
  Active: 'Активен',
  Ended: 'Завершён',
  Cancelled: 'Отменён',
};

async function openPdf(contractId: string) {
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
}

function ContractDetailModal({ contract, open, onClose }: { contract: ContractResponse | null; open: boolean; onClose: () => void }) {
  if (!contract) return null;

  return (
    <Modal
      title={<Text style={{ color: '#fff' }}>Договор #{contract.id.slice(0, 8)}</Text>}
      open={open}
      onCancel={onClose}
      width={700}
      footer={
        <Space>
          {contract.pdfPath && (
            <Button
              type="primary"
              icon={<DownloadOutlined />}
              onClick={() => openPdf(contract.id)}
            >
              Скачать PDF
            </Button>
          )}
        </Space>
      }
    >
      <div style={{ display: 'flex', flexDirection: 'column', gap: 16, width: '100%' }}>
        <Tag style={{ backgroundColor: statusColors[contract.status], color: '#fff', border: 'none' }}>
          {statusLabels[contract.status]}
        </Tag>

        <Descriptions
          column={2}
          size="small"
          styles={{
            label: { color: '#888' },
            content: { color: '#fff' },
          }}
          bordered
        >
          <Descriptions.Item label="Клиент">
            {contract.clientFullName}
          </Descriptions.Item>
          <Descriptions.Item label="Автомобиль">
            {contract.car}
          </Descriptions.Item>
          <Descriptions.Item label="Начало">
            {dayjs(contract.startDate).format('DD.MM.YYYY')}
          </Descriptions.Item>
          <Descriptions.Item label="Конец">
            {dayjs(contract.endDate).format('DD.MM.YYYY')}
          </Descriptions.Item>
          <Descriptions.Item label="Цена">
            {contract.estimatedPrice.toFixed(2)} Br
          </Descriptions.Item>
          <Descriptions.Item label="Создан">
            {dayjs(contract.createdAt).format('DD.MM.YYYY HH:mm')}
          </Descriptions.Item>
        </Descriptions>
      </div>
    </Modal>
  );
}

export default function AdminContractsPage() {
  const queryClient = useQueryClient();
  const [selectedContract, setSelectedContract] = useState<ContractResponse | null>(null);
  const [detailOpen, setDetailOpen] = useState(false);
  const [searchText, setSearchText] = useState('');
  const [statusFilter, setStatusFilter] = useState<ContractStatus | 'all'>('all');
  const [dateRange, setDateRange] = useState<[Dayjs | null, Dayjs | null] | null>(null);

  const { data: contracts, isLoading: contractsLoading } = useQuery({
    queryKey: ['contracts'],
    queryFn: () => contractApi.getAll(),
  });

  const filteredContracts = useMemo(() => {
    if (!contracts) return [];
    return contracts.filter(c => {
      if (statusFilter !== 'all' && c.status !== statusFilter) return false;
      if (searchText) {
        const q = searchText.toLowerCase();
        if (!c.clientFullName.toLowerCase().includes(q) && !c.car.toLowerCase().includes(q)) return false;
      }
      if (dateRange?.[0] && dateRange?.[1]) {
        const created = dayjs(c.createdAt);
        if (created.isBefore(dateRange[0].startOf('day')) || created.isAfter(dateRange[1].endOf('day'))) return false;
      }
      return true;
    });
  }, [contracts, searchText, statusFilter, dateRange]);

  const signMutation = useMutation({
    mutationFn: (contractId: string) =>
      contractApi.sign({ id: contractId, signatureBase64: 'signed-by-manager' }),
    onSuccess: () => {
      message.success('Договор подписан');
      queryClient.invalidateQueries({ queryKey: ['contracts'] });
    },
    onError: () => message.error('Ошибка при подписании'),
  });

  const cancelMutation = useMutation({
    mutationFn: (contractId: string) => contractApi.cancel(contractId),
    onSuccess: () => {
      message.success('Договор отменён');
      queryClient.invalidateQueries({ queryKey: ['contracts'] });
    },
    onError: () => message.error('Ошибка при отмене'),
  });

  const contractColumns = [
    {
      title: <Text style={{ color: '#888' }}>ID</Text>,
      dataIndex: 'id',
      key: 'id',
      render: (v: string) => <Text style={{ color: '#ccc' }}>{v.slice(0, 8)}...</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Клиент</Text>,
      dataIndex: 'clientFullName',
      key: 'client',
      render: (v: string) => <Text style={{ color: '#fff' }}>{v}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Автомобиль</Text>,
      dataIndex: 'car',
      key: 'car',
      render: (v: string) => <Text style={{ color: '#fff' }}>{v}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Статус</Text>,
      dataIndex: 'status',
      key: 'status',
      render: (v: ContractStatus) => (
        <Tag style={{ backgroundColor: statusColors[v], color: '#fff', border: 'none' }}>{statusLabels[v]}</Tag>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Создан</Text>,
      dataIndex: 'createdAt',
      key: 'createdAt',
      render: (v: string) => <Text style={{ color: '#ccc' }}>{dayjs(v).format('DD.MM.YYYY')}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Действия</Text>,
      key: 'actions',
      render: (_: unknown, record: ContractResponse) => (
        <Space>
          <Button type="link" style={{ color: '#f97316' }} icon={<EyeOutlined />}
            onClick={() => { setSelectedContract(record); setDetailOpen(true); }}>
            Детали
          </Button>
          {record.status === 'AwaitingSignature' && (
            <>
              <Button type="link" icon={<CheckCircleOutlined />} style={{ color: '#22c55e' }}
                onClick={() => Modal.confirm({ title: 'Подписать договор?', onOk: () => signMutation.mutate(record.id) })}>
                Подписать
              </Button>
              <Button type="link" danger icon={<CloseCircleOutlined />}
                onClick={() => Modal.confirm({ title: 'Отменить договор?', onOk: () => cancelMutation.mutate(record.id) })}>
                Отменить
              </Button>
            </>
          )}
          <Button type="link" icon={<DownloadOutlined />} style={{ color: '#888' }}
            onClick={() => openPdf(record.id)}>
            PDF
          </Button>
        </Space>
      ),
    },
  ];

  if (contractsLoading) {
    return (
      <div style={{ textAlign: 'center', padding: 80 }}>
        <Spin size="large" />
      </div>
    );
  }

  return (
    <div style={{ maxWidth: 1200, margin: '0 auto', padding: '32px' }}>
      <Title level={2} style={{ color: '#fff', marginBottom: 24 }}>Управление договорами</Title>

      <Space wrap style={{ marginBottom: 16, width: '100%', justifyContent: 'space-between' }}>
        <Space wrap>
          <Input.Search
            placeholder="Поиск по клиенту или автомобилю..."
            value={searchText}
            onChange={e => setSearchText(e.target.value)}
            onSearch={setSearchText}
            allowClear
            style={{ width: 320 }}
          />
          <Select
            value={statusFilter}
            onChange={v => setStatusFilter(v)}
            style={{ width: 160 }}
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

      <div style={{ background: '#1a1a1a', borderRadius: 12, border: '1px solid rgba(255,255,255,0.06)', overflow: 'hidden', padding: 24 }}>
        <Table
          dataSource={filteredContracts}
          columns={contractColumns}
          rowKey="id"
          pagination={{ pageSize: 10 }}
          scroll={{ x: 900 }}
          style={{ background: 'transparent' }}
        />
      </div>

      <ContractDetailModal contract={selectedContract} open={detailOpen}
        onClose={() => { setDetailOpen(false); setSelectedContract(null); }} />
    </div>
  );
}
