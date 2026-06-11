import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import {
  Tag,
  Typography,
  Spin,
  Button,
  Row,
  Col,
  Divider,
} from 'antd';
import {
  ArrowLeftOutlined,
  CarOutlined,
  DollarOutlined,
  BarcodeOutlined,
  DashboardOutlined,
  CalendarOutlined,
  ToolOutlined,
} from '@ant-design/icons';
import { carApi } from '../../api/endpoints';
import { useAuthStore } from '../../stores/authStore';

const { Title, Text } = Typography;

const statusColors: Record<string, string> = {
  Available: '#22c55e',
  Rented: '#3b82f6',
  Maintenance: '#f97316',
  Broken: '#ef4444',
  Returned: '#a855f7',
};

const statusLabels: Record<string, string> = {
  Available: 'Доступен',
  Rented: 'Арендован',
  Maintenance: 'На обслуживании',
  Broken: 'Сломан',
  Returned: 'Возвращён',
};

export default function CarDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);

  const { data: car, isLoading, error } = useQuery({
    queryKey: ['car', id, isAuthenticated ? 'detailed' : 'public'],
    queryFn: () => isAuthenticated ? carApi.getDetailed(id!) : carApi.getPublic(id!),
    enabled: !!id,
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
        Автомобиль не найден или сервис недоступен.
      </div>
    );
  }

  if (!car) {
    return (
      <div style={{ textAlign: 'center', padding: 120, color: '#888' }}>
        Автомобиль не найден
      </div>
    );
  }

  const driveTypeLabels: Record<string, string> = {
    Front: 'Передний',
    Rear: 'Задний',
    All: 'Полный',
  };

  const specs: { icon: React.ReactNode; label: string; value: string | null }[] = [
    car.mileage != null ? { icon: <DashboardOutlined />, label: 'Пробег', value: `${car.mileage.toLocaleString()} км` } : null,
    car.transmission ? { icon: <ToolOutlined />, label: 'Коробка', value: car.transmission === 'Automatic' ? 'Автомат' : 'Механика' } : null,
    car.driveType ? { icon: <CarOutlined />, label: 'Привод', value: driveTypeLabels[car.driveType] || car.driveType } : null,
    car.releaseDate ? { icon: <CalendarOutlined />, label: 'Год', value: new Date(car.releaseDate).getFullYear().toString() } : null,
  ].filter(Boolean) as { icon: React.ReactNode; label: string; value: string }[];

  return (
    <div style={{ maxWidth: 1200, margin: '0 auto', padding: '32px' }}>
      <Button
        type="text"
        icon={<ArrowLeftOutlined />}
        onClick={() => navigate('/cars')}
        style={{ color: '#888', marginBottom: 16, padding: 0 }}
      >
        Назад к каталогу
      </Button>

      <Row gutter={[32, 32]}>
        <Col xs={24} lg={14}>
          {car.photoUrl ? (
            <img
              src={car.photoUrl}
              alt={`${car.brand} ${car.model}`}
              style={{
                width: '100%',
                height: 360,
                objectFit: 'cover',
                borderRadius: 12,
                border: '1px solid rgba(255,255,255,0.06)',
              }}
            />
          ) : (
            <div
              style={{
                height: 360,
                background: 'linear-gradient(135deg, #2a1a0a 0%, #1a1a2e 100%)',
                borderRadius: 12,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                border: '1px solid rgba(255,255,255,0.06)',
              }}
            >
              <CarOutlined style={{ fontSize: 120, color: '#f97316', opacity: 0.4 }} />
            </div>
          )}
        </Col>

        <Col xs={24} lg={10}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 16, width: '100%' }}>
            <div>
              <div style={{ marginBottom: 8 }}>
                <Tag style={{ backgroundColor: statusColors[car.status], color: '#fff', border: 'none', fontSize: 13, padding: '2px 12px' }}>
                  {statusLabels[car.status]}
                </Tag>
              </div>
              <Title level={2} style={{ color: '#fff', margin: 0 }}>
                {car.brand} {car.model}
              </Title>
              {car.generation && (
                <Text style={{ color: '#888' }}>{car.generation}</Text>
              )}
            </div>

            <div
              style={{
                padding: 20,
                background: '#111',
                borderRadius: 12,
                border: '1px solid rgba(255,255,255,0.06)',
              }}
            >
              <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                <DollarOutlined style={{ color: '#f97316', fontSize: 24 }} />
                <Text style={{ color: '#f97316', fontSize: 36, fontWeight: 800 }}>
                  {car.pricePerHour}
                </Text>
                <Text style={{ color: '#888', fontSize: 16 }}>Br/час</Text>
              </div>
            </div>

            {isAuthenticated ? (
              <Button
                type="primary"
                size="large"
                block
                style={{ height: 48, fontSize: 16 }}
                disabled={car.status !== 'Available'}
                onClick={() => navigate(`/rentals/new/${car.id}`)}
              >
                {car.status === 'Available' ? 'Арендовать сейчас' : statusLabels[car.status]}
              </Button>
            ) : (
              <Button
                type="primary"
                size="large"
                block
                style={{ height: 48, fontSize: 16 }}
                onClick={() => navigate('/login')}
              >
                Войдите для аренды
              </Button>
            )}

            {car.vinCode && car.licensePlate && (
              <div style={{ color: '#666', fontSize: 13 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                  <BarcodeOutlined />
                  VIN: <Text code style={{ color: '#888' }}>{car.vinCode}</Text>
                </div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 6, marginTop: 4 }}>
                  <CarOutlined />
                  Госномер: <Text style={{ color: '#888' }}>{car.licensePlate}</Text>
                </div>
              </div>
            )}
          </div>
        </Col>
      </Row>

      {specs.length > 0 && (
        <>
          <Divider style={{ borderColor: 'rgba(255,255,255,0.06)', margin: '40px 0' }} />
          <Title level={3} style={{ color: '#fff', marginBottom: 20 }}>Характеристики</Title>
          <Row gutter={[16, 16]}>
            {specs.map((s) => (
              <Col xs={12} sm={8} lg={4} key={s.label}>
                <div
                  style={{
                    padding: '16px 12px',
                    background: '#1a1a1a',
                    borderRadius: 8,
                    textAlign: 'center',
                    border: '1px solid rgba(255,255,255,0.06)',
                  }}
                >
                  <div style={{ display: 'flex', flexDirection: 'column', gap: 4, alignItems: 'center' }}>
                    {s.icon}
                    <Text style={{ color: '#666', fontSize: 12 }}>{s.label}</Text>
                    <Text style={{ color: '#fff', fontSize: 14, fontWeight: 600 }}>{s.value}</Text>
                  </div>
                </div>
              </Col>
            ))}
          </Row>
        </>
      )}

      {car.color && (
        <>
          <Divider style={{ borderColor: 'rgba(255,255,255,0.06)', margin: '40px 0' }} />
          <Title level={3} style={{ color: '#fff', marginBottom: 20 }}>Дополнительно</Title>
          <Row gutter={[16, 16]}>
            <Col xs={12}>
              <div
                style={{
                  padding: 16,
                  background: '#1a1a1a',
                  borderRadius: 8,
                  border: '1px solid rgba(255,255,255,0.06)',
                }}
              >
                <Text style={{ color: '#888' }}>Цвет</Text>
                <br />
                <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                  <div
                    style={{
                      width: 16,
                      height: 16,
                      borderRadius: '50%',
                      background: car.color.toLowerCase(),
                      border: '1px solid rgba(255,255,255,0.2)',
                      display: 'inline-block',
                    }}
                  />
                  <Text style={{ color: '#fff' }}>{car.color}</Text>
                </div>
              </div>
            </Col>
            <Col xs={12}>
              <div
                style={{
                  padding: 16,
                  background: '#1a1a1a',
                  borderRadius: 8,
                  border: '1px solid rgba(255,255,255,0.06)',
                }}
              >
                <Text style={{ color: '#888' }}>Класс</Text>
                <br />
                <Text style={{ color: '#fff' }}>{car.class}</Text>
              </div>
            </Col>
          </Row>
        </>
      )}
    </div>
  );
}
