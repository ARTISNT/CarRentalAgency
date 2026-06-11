import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import {
  Row,
  Col,
  Card,
  Typography,
  Tag,
  Select,
  Space,
  Input,
  Spin,
  Button,
} from 'antd';
import {
  CarOutlined,
  DollarOutlined,
  DashboardOutlined,
  SearchOutlined,
} from '@ant-design/icons';
import { carApi } from '../../api/endpoints';
import { useAuthStore } from '../../stores/authStore';
import type { CarClass } from '../../types';

const { Title, Text } = Typography;

const statusLabels: Record<string, string> = {
  Available: 'Доступен',
  Rented: 'Арендован',
  Maintenance: 'На обслуживании',
  Broken: 'Сломан',
  Returned: 'Возвращён',
};

const classLabels: Record<CarClass, string> = {
  Economy: 'Эконом',
  Standard: 'Стандарт',
  Business: 'Бизнес',
  Premium: 'Премиум',
};

export default function CarCatalogPage() {
  const navigate = useNavigate();
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);
  const [search, setSearch] = useState('');
  const [filterClass, setFilterClass] = useState<string | undefined>(undefined);
  const [filterStatus, setFilterStatus] = useState<string | undefined>('Available');

  const { data: cars, isLoading, error } = useQuery({
    queryKey: ['cars', isAuthenticated ? 'all' : 'available'],
    queryFn: () => isAuthenticated ? carApi.getAll() : carApi.getAvailable(),
  });

  const filtered = (cars || []).filter((car) => {
    const q = search.toLowerCase();
    const matchesSearch =
      !q ||
      car.brand.toLowerCase().includes(q) ||
      car.model.toLowerCase().includes(q) ||
      (car.licensePlate || '').toLowerCase().includes(q);
    const matchesClass = !filterClass || car.class === filterClass;
    const matchesStatus = !filterStatus || car.status === filterStatus;
    return matchesSearch && matchesClass && matchesStatus;
  });

  if (isLoading) {
    return (
      <div style={{ textAlign: 'center', padding: 120 }}>
        <Spin size="large" />
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ textAlign: 'center', padding: 120, color: '#888' }}>
        Сервис недоступен. Проверьте, запущен ли бэкенд (порт 5000).
      </div>
    );
  }

  return (
    <div style={{ maxWidth: 1200, margin: '0 auto', padding: '32px' }}>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 24, width: '100%' }}>
        <div>
          <Title level={2} style={{ color: '#fff', margin: 0 }}>Автомобили</Title>
          <Text style={{ color: '#888' }}>{isAuthenticated ? 'Весь автопарк' : 'Доступные автомобили'} — выбирайте и бронируйте</Text>
        </div>

        <Row gutter={[16, 16]}>
          <Col xs={24} sm={12} md={8}>
            <Input
              prefix={<SearchOutlined style={{ color: '#666' }} />}
              placeholder="Поиск по марке, модели..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              allowClear
              variant="filled"
              size="large"
            />
          </Col>
          <Col xs={12} sm={6} md={4}>
            <Select
              placeholder="Класс"
              allowClear
              style={{ width: '100%' }}
              value={filterClass}
              onChange={setFilterClass}
              size="large"
              options={Object.entries(classLabels).map(([v, l]) => ({ value: v, label: l }))}
            />
          </Col>
          {isAuthenticated && (
            <Col xs={12} sm={6} md={4}>
              <Select
                placeholder="Статус"
                allowClear
                style={{ width: '100%' }}
                value={filterStatus}
                onChange={setFilterStatus}
                size="large"
                options={[
                  { value: 'Available', label: 'Доступен' },
                  { value: 'Rented', label: 'Арендован' },
                  { value: 'Maintenance', label: 'На обслуживании' },
                  { value: 'Broken', label: 'Сломан' },
                  { value: 'Returned', label: 'Возвращён' },
                ]}
              />
            </Col>
          )}
        </Row>

        <Row gutter={[20, 20]}>
          {filtered.map((car) => (
            <Col xs={24} sm={12} lg={8} key={car.id}>
              <Card
                hoverable
                style={{
                  background: '#1a1a1a',
                  border: '1px solid rgba(255,255,255,0.06)',
                }}
                onClick={() => navigate(`/cars/${car.id}`)}
                actions={[
                  <Button
                    type="primary"
                    onClick={(e) => {
                      e.stopPropagation();
                      navigate(`/rentals/new/${car.id}`);
                    }}
                    style={{ width: '90%', margin: '0 auto' }}
                    disabled={car.status !== 'Available'}
                  >
                    {car.status === 'Available'
                      ? `Арендовать — ${car.pricePerHour} Br/ч`
                      : statusLabels[car.status]}
                  </Button>,
                ]}
              >
                <div
                  style={{
                    height: 140,
                    background: 'linear-gradient(135deg, #2a1a0a 0%, #1a1a2e 100%)',
                    borderRadius: 8,
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    marginBottom: 12,
                  }}
                >
                  <CarOutlined style={{ fontSize: 64, color: '#f97316', opacity: 0.5 }} />
                </div>
                <Card.Meta
                  title={
                    <Text style={{ color: '#fff', fontSize: 16, fontWeight: 600 }}>
                      {car.brand} {car.model}
                    </Text>
                  }
                  description={
                    <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
                      <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
                        <Tag color={car.status === 'Available' ? 'green' : 'default'}>
                          {statusLabels[car.status]}
                        </Tag>
                        <Tag color="orange">{classLabels[car.class]}</Tag>
                        {car.transmission && (
                          <Tag>{car.transmission === 'Automatic' ? 'Автомат' : 'Механика'}</Tag>
                        )}
                      </div>
                      {car.mileage != null && (
                        <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                          <DashboardOutlined style={{ color: '#666' }} />
                          <Text style={{ color: '#888' }}>{car.mileage.toLocaleString()} км</Text>
                        </div>
                      )}
                      {car.licensePlate && (
                        <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                          <Text style={{ color: '#666' }}>{car.licensePlate}</Text>
                        </div>
                      )}
                      <div>
                        <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                          <DollarOutlined style={{ color: '#f97316' }} />
                          <Text style={{ color: '#f97316', fontSize: 22, fontWeight: 700 }}>
                            {car.pricePerHour}
                          </Text>
                          <Text style={{ color: '#888' }}>Br/час</Text>
                        </div>
                      </div>
                    </div>
                  }
                />
              </Card>
            </Col>
          ))}
        </Row>
      </div>
    </div>
  );
}
