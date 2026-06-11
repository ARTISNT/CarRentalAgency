import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Card,
  Typography,
  DatePicker,
  Input,
  Button,
  Tag,
  message,
  Spin,
  Divider,
  Statistic,
  Row,
  Col,
} from 'antd';
import {
  ArrowLeftOutlined,
  DollarOutlined,
  GiftOutlined,
  CalendarOutlined,
  CarOutlined,
} from '@ant-design/icons';
import dayjs from 'dayjs';
import { carApi, rentalApi } from '../../api/endpoints';
import { useAuthStore } from '../../stores/authStore';

const { Title, Text } = Typography;
const { RangePicker } = DatePicker;

const statusLabels: Record<string, string> = {
  Available: 'Доступен',
  Rented: 'Арендован',
  Maintenance: 'На обслуживании',
  Broken: 'Сломан',
  Returned: 'Возвращён',
};

export default function CreateRentalPage() {
  const { carId } = useParams<{ carId: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const user = useAuthStore((s) => s.user);

  const [dates, setDates] = useState<[dayjs.Dayjs | null, dayjs.Dayjs | null]>([null, null]);
  const [promoCode, setPromoCode] = useState('');

  const { data: car, isLoading } = useQuery({
    queryKey: ['car', carId],
    queryFn: () => carApi.getPublic(carId!),
    enabled: !!carId,
  });

  const createMutation = useMutation({
    mutationFn: () =>
      rentalApi.create({
        userId: user!.id,
        carId: carId!,
        startDate: dates[0]!.toISOString(),
        endDate: dates[1]!.toISOString(),
        promoCode: promoCode || null,
      }),
    onSuccess: (data: { rentalId: string }) => {
      message.success('Аренда создана!');
      queryClient.invalidateQueries({ queryKey: ['rentals'] });
      navigate(`/my-rentals/${data.rentalId}`);
    },
    onError: () => {
      message.error('Ошибка при создании аренды');
    },
  });

  if (isLoading) {
    return (
      <div style={{ textAlign: 'center', padding: 120 }}>
        <Spin size="large" />
      </div>
    );
  }

  if (!car) {
    return <div style={{ textAlign: 'center', padding: 120, color: '#888' }}>Автомобиль не найден</div>;
  }

  const hours = dates[0] && dates[1] ? dates[1].diff(dates[0], 'hour') : 0;
  const estimatedCost = hours * car.pricePerHour;

  return (
    <div style={{ maxWidth: 1200, margin: '0 auto', padding: '32px' }}>
      <Button
        type="text"
        icon={<ArrowLeftOutlined />}
        onClick={() => navigate(`/cars/${carId}`)}
        style={{ color: '#888', marginBottom: 16, padding: 0 }}
      >
        Назад к автомобилю
      </Button>

      <Row gutter={24}>
        <Col xs={24} lg={16}>
          <Card
            style={{
              background: '#1a1a1a',
              border: '1px solid rgba(255,255,255,0.06)',
            }}
          >
            <div style={{ display: 'flex', flexDirection: 'column', gap: 24, width: '100%' }}>
              <Title level={3} style={{ color: '#fff', margin: 0 }}>Новая аренда</Title>

              <div
                style={{
                  padding: 20,
                  background: '#111',
                  borderRadius: 12,
                  display: 'flex',
                  alignItems: 'center',
                  gap: 16,
                  border: '1px solid rgba(255,255,255,0.06)',
                }}
              >
                <CarOutlined style={{ fontSize: 48, color: '#f97316', opacity: 0.6 }} />
                <div>
                  <Text style={{ color: '#fff', fontSize: 18, fontWeight: 600 }}>
                    {car.brand} {car.model}
                  </Text>
                  <br />
                  {car.licensePlate && <Text style={{ color: '#888' }}>{car.licensePlate}</Text>}
                  <Tag color={car.status === 'Available' ? 'green' : 'red'} style={{ marginLeft: 8 }}>
                    {statusLabels[car.status]}
                  </Tag>
                </div>
                <div style={{ marginLeft: 'auto', textAlign: 'right' }}>
                  <Text style={{ color: '#f97316', fontSize: 24, fontWeight: 700 }}>
                    {car.pricePerHour}
                  </Text>
                  <Text style={{ color: '#888' }}> Br/ч</Text>
                </div>
              </div>

              <Divider style={{ borderColor: 'rgba(255,255,255,0.06)' }} />

              <div>
                <Text style={{ color: '#ccc', fontWeight: 600 }}>Даты аренды</Text>
                <div style={{ marginTop: 8 }}>
                  <RangePicker
                    size="large"
                    style={{ width: '100%' }}
                    showTime
                    format="DD.MM.YYYY HH:mm"
                    disabledDate={(d) => d.isBefore(dayjs(), 'day')}
                    onChange={(v) => setDates(v || [null, null])}
                    bordered={false}
                    variant="filled"
                  />
                </div>
              </div>

              <div>
                <Text style={{ color: '#ccc', fontWeight: 600 }}>Промокод</Text>
                <div style={{ marginTop: 8 }}>
                  <Input
                    prefix={<GiftOutlined style={{ color: '#666' }} />}
                    placeholder="Введите промокод (если есть)"
                    value={promoCode}
                    onChange={(e) => setPromoCode(e.target.value)}
                    size="large"
                    variant="filled"
                  />
                </div>
              </div>

              <Button
                type="primary"
                size="large"
                block
                style={{ height: 48, fontSize: 16 }}
                disabled={!dates[0] || !dates[1] || car.status !== 'Available'}
                loading={createMutation.isPending}
                icon={<CalendarOutlined />}
                onClick={() => createMutation.mutate()}
              >
                {car.status !== 'Available' ? 'Автомобиль недоступен' : 'Забронировать'}
              </Button>
            </div>
          </Card>
        </Col>

        <Col xs={24} lg={8}>
          <Card
            style={{
              background: '#1a1a1a',
              border: '1px solid rgba(255,255,255,0.06)',
            }}
          >
            <Title level={4} style={{ color: '#fff', marginBottom: 16 }}>Расчёт стоимости</Title>
            {dates[0] && dates[1] ? (
              <div style={{ display: 'flex', flexDirection: 'column', gap: 16, width: '100%' }}>
                <Statistic
                  title={<Text style={{ color: '#888' }}>Длительность</Text>}
                  value={hours}
                  suffix="ч"
                  valueStyle={{ color: '#fff' }}
                />
                <Statistic
                  title={<Text style={{ color: '#888' }}>Цена за час</Text>}
                  value={car.pricePerHour}
                  prefix={<DollarOutlined />}
                  suffix="Br"
                  valueStyle={{ color: '#fff' }}
                />
                <Divider style={{ borderColor: 'rgba(255,255,255,0.06)' }} />
                <Statistic
                  title={<Text style={{ color: '#888' }}>Оценочная стоимость</Text>}
                  value={estimatedCost}
                  prefix={<DollarOutlined />}
                  suffix="Br"
                  valueStyle={{ color: '#f97316', fontSize: 28, fontWeight: 700 }}
                />
              </div>
            ) : (
              <Text style={{ color: '#666' }}>Выберите даты аренды для расчёта</Text>
            )}
          </Card>
        </Col>
      </Row>
    </div>
  );
}
