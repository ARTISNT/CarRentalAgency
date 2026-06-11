import { useMemo, useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table,
  Tag,
  Typography,
  Spin,
  Button,
  Space,
  Input,
  Select,
  Modal,
  message,
} from 'antd';
import {
  CheckCircleOutlined,
  CloseCircleOutlined,
  DeleteOutlined,
} from '@ant-design/icons';
import { userApi } from '../../api/endpoints';
import type { UserResponse, UserRole } from '../../types';

const { Title, Text } = Typography;

const roleColors: Record<UserRole, string> = {
  Admin: '#ef4444',
  Manager: '#f97316',
  Client: '#22c55e',
};

const roleLabels: Record<UserRole, string> = {
  Admin: 'Администратор',
  Manager: 'Менеджер',
  Client: 'Клиент',
};

export default function AdminUsersPage() {
  const queryClient = useQueryClient();

  const [searchText, setSearchText] = useState('');
  const [roleFilter, setRoleFilter] = useState<UserRole | 'all'>('all');
  const [activeFilter, setActiveFilter] = useState<'all' | 'active' | 'inactive'>('all');

  const { data: users, isLoading } = useQuery({
    queryKey: ['users'],
    queryFn: () => userApi.getAll(),
  });

  const filteredUsers = useMemo(() => {
    if (!users) return [];
    return users.filter(u => {
      if (roleFilter !== 'all' && u.role !== roleFilter) return false;
      if (activeFilter === 'active' && !u.isActive) return false;
      if (activeFilter === 'inactive' && u.isActive) return false;
      if (searchText) {
        const q = searchText.toLowerCase();
        if (!u.email.toLowerCase().includes(q) && !u.phoneNumber.toLowerCase().includes(q)) return false;
      }
      return true;
    });
  }, [users, searchText, roleFilter, activeFilter]);

  const activateMutation = useMutation({
    mutationFn: (userId: string) => userApi.activate(userId),
    onSuccess: () => {
      message.success('Пользователь активирован');
      queryClient.invalidateQueries({ queryKey: ['users'] });
    },
    onError: () => message.error('Ошибка при активации'),
  });

  const deactivateMutation = useMutation({
    mutationFn: (userId: string) => userApi.deactivate(userId),
    onSuccess: () => {
      message.success('Пользователь деактивирован');
      queryClient.invalidateQueries({ queryKey: ['users'] });
    },
    onError: () => message.error('Ошибка при деактивации'),
  });

  const deleteMutation = useMutation({
    mutationFn: (userId: string) => userApi.delete(userId),
    onSuccess: () => {
      message.success('Пользователь удалён');
      queryClient.invalidateQueries({ queryKey: ['users'] });
    },
    onError: () => message.error('Ошибка при удалении'),
  });

  const columns = [
    {
      title: <Text style={{ color: '#888' }}>Email</Text>,
      dataIndex: 'email',
      key: 'email',
      render: (v: string) => <Text style={{ color: '#fff' }}>{v}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Телефон</Text>,
      dataIndex: 'phoneNumber',
      key: 'phone',
      render: (v: string) => <Text style={{ color: '#ccc' }}>{v}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Роль</Text>,
      dataIndex: 'role',
      key: 'role',
      render: (role: UserRole) => (
        <Tag style={{ backgroundColor: roleColors[role], color: '#fff', border: 'none' }}>{roleLabels[role]}</Tag>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Статус</Text>,
      dataIndex: 'isActive',
      key: 'isActive',
      render: (active: boolean) => (
        <Tag style={{ backgroundColor: active ? '#22c55e' : '#ef4444', color: '#fff', border: 'none' }}>
          {active ? 'Активен' : 'Деактивирован'}
        </Tag>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Действия</Text>,
      key: 'actions',
      render: (_: unknown, record: UserResponse) => (
        <Space>
          {record.isActive ? (
            <Button
              type="link"
              icon={<CloseCircleOutlined />}
              style={{ color: '#f97316' }}
              onClick={() =>
                Modal.confirm({
                  title: 'Деактивировать?',
                  content: `Пользователь ${record.email} будет деактивирован.`,
                  onOk: () => deactivateMutation.mutate(record.id),
                })
              }
            >
              Деактивировать
            </Button>
          ) : (
            <Button
              type="link"
              icon={<CheckCircleOutlined />}
              style={{ color: '#22c55e' }}
              onClick={() => activateMutation.mutate(record.id)}
            >
              Активировать
            </Button>
          )}
          <Button
            type="link"
            danger
            icon={<DeleteOutlined />}
            onClick={() =>
              Modal.confirm({
                title: 'Удалить пользователя?',
                content: `Пользователь ${record.email} будет удалён.`,
                onOk: () => deleteMutation.mutate(record.id),
              })
            }
          >
            Удалить
          </Button>
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
      <Title level={2} style={{ color: '#fff', marginBottom: 24 }}>Управление пользователями</Title>

      <Space wrap style={{ marginBottom: 16, width: '100%', justifyContent: 'space-between' }}>
        <Space wrap>
          <Input.Search
            placeholder="Поиск по email или телефону..."
            value={searchText}
            onChange={e => setSearchText(e.target.value)}
            onSearch={setSearchText}
            allowClear
            style={{ width: 320 }}
          />
          <Select
            value={roleFilter}
            onChange={v => setRoleFilter(v)}
            style={{ width: 160 }}
            options={[
              { value: 'all', label: 'Все роли' },
              ...Object.entries(roleLabels).map(([value, label]) => ({ value, label })),
            ]}
          />
          <Select
            value={activeFilter}
            onChange={v => setActiveFilter(v)}
            style={{ width: 140 }}
            options={[
              { value: 'all', label: 'Все' },
              { value: 'active', label: 'Активен' },
              { value: 'inactive', label: 'Деактивирован' },
            ]}
          />
          <Button onClick={() => { setSearchText(''); setRoleFilter('all'); setActiveFilter('all'); }}>
            Сбросить
          </Button>
        </Space>
        <Text style={{ color: '#888' }}>Найдено: {filteredUsers.length}</Text>
      </Space>

      <div style={{ background: '#1a1a1a', borderRadius: 12, border: '1px solid rgba(255,255,255,0.06)', overflow: 'hidden' }}>
        <Table
          dataSource={filteredUsers}
          columns={columns}
          rowKey="id"
          pagination={{ pageSize: 10 }}
          scroll={{ x: 800 }}
          style={{ background: 'transparent' }}
        />
      </div>
    </div>
  );
}
