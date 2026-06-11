import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import {
  Typography,
  Button,
  Row,
  Col,
  Card,
  Space,
  Tag,
  Spin,
} from 'antd';
import {
  CarOutlined,
  SafetyCertificateOutlined,
  ThunderboltOutlined,
  CustomerServiceOutlined,
  ArrowRightOutlined,
  DollarOutlined,
} from '@ant-design/icons';
import { carApi } from '../api/endpoints';
const { Title, Text, Paragraph } = Typography;

export default function LandingPage() {
  const navigate = useNavigate();

  const { data: cars, isLoading, error } = useQuery({
    queryKey: ['cars', 'available'],
    queryFn: () => carApi.getAvailable(),
  });

  const popular = cars?.slice(0, 6) || [];

  const advantages = [
    {
      icon: <SafetyCertificateOutlined style={{ fontSize: 32, color: '#f97316' }} />,
      title: 'Полная страховка',
      desc: 'Все автомобили застрахованы. Вы в полной безопасности.',
    },
    {
      icon: <ThunderboltOutlined style={{ fontSize: 32, color: '#f97316' }} />,
      title: 'Мгновенное бронирование',
      desc: 'Забронируйте авто за 2 минуты без визита в офис.',
    },
    {
      icon: <DollarOutlined style={{ fontSize: 32, color: '#f97316' }} />,
      title: 'Лучшие цены',
      desc: 'Прямые цены от собственника без наценок посредников.',
    },
    {
      icon: <CustomerServiceOutlined style={{ fontSize: 32, color: '#f97316' }} />,
      title: 'Поддержка 24/7',
      desc: 'Круглосуточная поддержка на дороге и по телефону.',
    },
  ];

  const steps = [
    { step: '1', title: 'Выберите авто', desc: 'Просмотрите каталог и выберите подходящий автомобиль' },
    { step: '2', title: 'Укажите даты', desc: 'Выберите даты аренды и получите расчёт стоимости' },
    { step: '3', title: 'Поехали!', desc: 'Подпишите договор и заберите автомобиль' },
  ];

  return (
    <div>
      {/* Hero */}
      <div
        style={{
          position: 'relative',
          overflow: 'hidden',
          padding: '100px 32px 80px',
          textAlign: 'center',
          background: 'linear-gradient(135deg, #0a0a0a 0%, #1a0a00 50%, #0a0a0a 100%)',
        }}
      >
        <div
          style={{
            position: 'absolute',
            top: '-50%',
            left: '-50%',
            width: '200%',
            height: '200%',
            background: 'radial-gradient(circle at 50% 50%, rgba(249,115,22,0.08) 0%, transparent 50%)',
            pointerEvents: 'none',
          }}
        />
        <div style={{ position: 'relative', zIndex: 1, display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 24 }}>
          <Tag
            color="orange"
            style={{ fontSize: 14, padding: '4px 16px', borderRadius: 20 }}
          >
            До {cars?.length || 0} автомобилей в наличии
          </Tag>
          <Title
            level={1}
            style={{
              fontSize: 56,
              fontWeight: 800,
              color: '#fff',
              margin: 0,
              letterSpacing: '-1.5px',
            }}
          >
            Аренда автомобилей
            <br />
            <span style={{ color: '#f97316' }}>в Минске</span>
          </Title>
          <Paragraph
            style={{
              fontSize: 18,
              color: '#888',
              maxWidth: 600,
              margin: '0 auto',
            }}
          >
            От эконом-класса до премиум. Без залога, со страховкой, с доставкой
            в любую точку города.
          </Paragraph>
          <div style={{ display: 'flex', gap: 16 }}>
            <Button
              type="primary"
              size="large"
              style={{ height: 48, paddingInline: 32, fontSize: 16 }}
              onClick={() => navigate('/cars')}
            >
              Выбрать автомобиль <ArrowRightOutlined />
            </Button>
            <Button
              size="large"
              style={{ height: 48, paddingInline: 32, fontSize: 16, borderColor: '#333', color: '#ccc' }}
              onClick={() => window.scrollTo({ top: window.innerHeight, behavior: 'smooth' })}
            >
              Как это работает
            </Button>
          </div>
        </div>
      </div>

      {/* Advantages */}
      <div style={{ padding: '80px 32px', maxWidth: 1200, margin: '0 auto' }}>
        <Row gutter={[24, 24]}>
          {advantages.map((a) => (
            <Col xs={24} sm={12} lg={6} key={a.title}>
              <Card
                style={{
                  height: '100%',
                  background: '#1a1a1a',
                  border: '1px solid rgba(255,255,255,0.06)',
                  textAlign: 'center',
                }}
              >
                <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 12 }}>
                  {a.icon}
                  <Title level={4} style={{ color: '#fff', margin: 0 }}>{a.title}</Title>
                  <Text style={{ color: '#888' }}>{a.desc}</Text>
                </div>
              </Card>
            </Col>
          ))}
        </Row>
      </div>

      {/* Popular cars */}
      <div style={{ padding: '0 32px 80px', maxWidth: 1200, margin: '0 auto' }}>
        <div style={{ display: 'flex', flexDirection: 'column', gap: 24, width: '100%' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <div>
              <Title level={2} style={{ color: '#fff', margin: 0 }}>Доступные автомобили</Title>
              <Text style={{ color: '#666' }}>{cars?.length || 0} авто прямо сейчас</Text>
            </div>
            <Button type="link" style={{ color: '#f97316' }} onClick={() => navigate('/cars')}>
              Все авто <ArrowRightOutlined />
            </Button>
          </div>

          {isLoading ? (
            <div style={{ textAlign: 'center', padding: 40 }}>
              <Spin size="large" />
            </div>
          ) : error ? (
            <div style={{ textAlign: 'center', padding: 40, color: '#888' }}>
              Сервис недоступен. Попробуйте позже.
            </div>
          ) : (
            <Row gutter={[20, 20]}>
              {popular.map((car) => (
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
                        type="link"
                        style={{ color: '#f97316' }}
                        onClick={(e) => {
                          e.stopPropagation();
                          navigate(`/rentals/new/${car.id}`);
                        }}
                      >
                        Арендовать от {car.pricePerHour} Br/ч
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
                      <CarOutlined style={{ fontSize: 64, color: '#f97316', opacity: 0.6 }} />
                    </div>
                    <Card.Meta
                      title={
                        <Text style={{ color: '#fff', fontSize: 16 }}>
                          {car.brand} {car.model}
                        </Text>
                      }
                      description={
                        <div style={{ display: 'flex', flexDirection: 'column', gap: 4 }}>
                          <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
                            <Tag color="orange">{car.class === 'Economy' ? 'Эконом' : car.class === 'Premium' ? 'Премиум' : car.class}</Tag>
                          </div>
                          <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                            <DollarOutlined style={{ color: '#f97316' }} />
                            <Text style={{ color: '#f97316', fontSize: 18, fontWeight: 700 }}>
                              {car.pricePerHour} Br
                            </Text>
                            <Text style={{ color: '#666' }}>/ час</Text>
                          </div>
                        </div>
                      }
                    />
                  </Card>
                </Col>
              ))}
            </Row>
          )}
        </div>
      </div>

      {/* How it works */}
      <div
        style={{
          padding: '80px 32px',
          background: '#111',
        }}
      >
        <div style={{ maxWidth: 900, margin: '0 auto', textAlign: 'center' }}>
          <Title level={2} style={{ color: '#fff', marginBottom: 8 }}>Как это работает</Title>
          <Text style={{ color: '#666', fontSize: 16, display: 'block', marginBottom: 48 }}>
            Всего 3 простых шага
          </Text>
          <Row gutter={[48, 32]}>
            {steps.map((s) => (
              <Col xs={24} sm={8} key={s.step}>
                <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 12 }}>
                  <div
                    style={{
                      width: 48,
                      height: 48,
                      borderRadius: '50%',
                      background: '#f97316',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      fontSize: 20,
                      fontWeight: 700,
                      color: '#fff',
                      margin: '0 auto',
                    }}
                  >
                    {s.step}
                  </div>
                  <Title level={4} style={{ color: '#fff', margin: 0 }}>{s.title}</Title>
                  <Text style={{ color: '#888' }}>{s.desc}</Text>
                </div>
              </Col>
            ))}
          </Row>
        </div>
      </div>

      {/* CTA */}
      <div style={{ padding: '80px 32px', textAlign: 'center' }}>
        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 24 }}>
          <Title level={2} style={{ color: '#fff', margin: 0 }}>
            Готовы отправиться в путь?
          </Title>
          <Text style={{ color: '#888', fontSize: 16 }}>
            Выберите автомобиль и забронируйте прямо сейчас
          </Text>
          <Button
            type="primary"
            size="large"
            style={{ height: 48, paddingInline: 40, fontSize: 16 }}
            onClick={() => navigate('/cars')}
          >
            Перейти к каталогу <ArrowRightOutlined />
          </Button>
        </div>
      </div>
    </div>
  );
}
