import { useEffect, useMemo, useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table,
  Button,
  Tag,
  Typography,
  Space,
  Spin,
  Modal,
  Form,
  Input,
  InputNumber,
  Select,
  DatePicker,
  Switch,
  Divider,
  Tabs,
  message,
} from 'antd';
import {
  EditOutlined,
  DeleteOutlined,
  PlusOutlined,
  EyeOutlined,
} from '@ant-design/icons';
import dayjs, { type Dayjs } from 'dayjs';
import { carApi } from '../../api/endpoints';
import CarDetailsModal from './CarDetailsModal';
import type { AddCarRequest, Car, UpdateCarRequest, BodyStyle, CarClass, DriveType, EngineType, TransmissionType } from '../../types';

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

const nextActions: Record<string, { label: string; action: (id: string) => Promise<unknown>; color: string }[]> = {
  Available: [
    { label: 'В обслуживание', action: (id) => carApi.sendToMaintenance(id), color: '#f97316' },
    { label: 'Снять с линии', action: (id) => carApi.break_(id), color: '#ef4444' },
  ],
  Rented: [],
  Maintenance: [
    { label: 'Завершить', action: (id) => carApi.completeMaintenance(id), color: '#22c55e' },
  ],
  Broken: [
    { label: 'В ремонт', action: (id) => carApi.sendToRepair(id), color: '#f97316' },
  ],
  Returned: [
    { label: 'Вернуть в строй', action: (id) => carApi.processReturnWithStatus(id, 'Available'), color: '#22c55e' },
    { label: 'В обслуживание', action: (id) => carApi.processReturnWithStatus(id, 'Maintenance'), color: '#f97316' },
    { label: 'Сломан', action: (id) => carApi.processReturnWithStatus(id, 'Broken'), color: '#ef4444' },
  ],
};

const bodyStyleOptions: BodyStyle[] = ['Sedan', 'Hatchback', 'SUV', 'Crossover', 'StationWagon', 'Minivan', 'Van', 'Coupe', 'Convertible', 'Pickup', 'Limousine', 'Roadster'];
const transmissionOptions: TransmissionType[] = ['Manual', 'Automatic', 'Variator', 'Robotic'];
const driveTypeOptions: DriveType[] = ['Fwd', 'Rwd', 'Awd', 'FourByFour'];
const engineTypeOptions: EngineType[] = ['Gasoline', 'Diesel', 'HybridGasoline', 'HybridDiesel', 'Electric'];
const carClassOptions: CarClass[] = ['Economy', 'Standard', 'Business', 'Premium'];

const LICENSE_PLATE_REGEX = /^\d{4} [A-Z]{2}-[1-7]$/;
const VIN_REGEX = /^[A-HJ-NPR-Z0-9]{17}$/i;

interface CarFormValues {
  brand: string;
  model: string;
  generation?: string;
  variant?: string;
  isFacelift: boolean;
  releaseDate: Dayjs;
  licensePlate: string;
  vinCode: string;
  color: string;
  mileage: number;
  fuelCurrentLiters?: number;
  fuelCapacityLiters?: number;
  batteryCurrentKWh?: number;
  batteryCapacityKWh?: number;
  bodyStyle: BodyStyle;
  transmissionType: TransmissionType;
  driveType: DriveType;
  engineType: EngineType;
  engineVolume?: number;
  horsePower: number;
  powerReverse?: number;
  pricePerHour: number;
  carClass: CarClass;
  photoUrl: string;
}

function toRequest(v: CarFormValues): AddCarRequest {
  return {
    releaseDate: v.releaseDate.toISOString(),
    licensePlate: v.licensePlate.trim().toUpperCase(),
    vinCode: v.vinCode.trim().toUpperCase(),
    color: v.color,
    model: v.model,
    brand: v.brand,
    generation: v.generation || null,
    isFacelift: v.isFacelift,
    variant: v.variant || null,
    pricePerHour: v.pricePerHour,
    CarClass: v.carClass,
    photoUrl: v.photoUrl,
    fuelCurrentLiters: v.fuelCurrentLiters ?? null,
    fuelCapacityLiters: v.fuelCapacityLiters ?? null,
    batteryCurrentKWh: v.batteryCurrentKWh ?? null,
    batteryCapacityKWh: v.batteryCapacityKWh ?? null,
    mileage: v.mileage,
    bodyStyle: v.bodyStyle,
    transmissionType: v.transmissionType,
    driveType: v.driveType,
    engineType: v.engineType,
    engineVolume: v.engineVolume ?? null,
    horsePower: v.horsePower,
    powerReverse: v.powerReverse ?? null,
  };
}

function toUpdateRequest(v: CarFormValues): UpdateCarRequest {
  return {
    releaseDate: v.releaseDate.toISOString(),
    licensePlate: v.licensePlate.trim().toUpperCase(),
    vinCode: v.vinCode.trim().toUpperCase(),
    color: v.color,
    model: v.model,
    brand: v.brand,
    generation: v.generation || null,
    isFacelift: v.isFacelift,
    variant: v.variant || null,
    pricePerHour: v.pricePerHour,
    CarClass: v.carClass,
    photoUrl: v.photoUrl,
    fuelCurrentLiters: v.fuelCurrentLiters ?? null,
    fuelCapacityLiters: v.fuelCapacityLiters ?? null,
    batteryCurrentKWh: v.batteryCurrentKWh ?? null,
    batteryCapacityKWh: v.batteryCapacityKWh ?? null,
    mileage: v.mileage,
    bodyStyle: v.bodyStyle,
    transmissionType: v.transmissionType,
    driveType: v.driveType,
    engineType: v.engineType,
    engineVolume: v.engineVolume ?? 0,
    horsePower: v.horsePower,
    powerReverse: v.powerReverse ?? 0,
  };
}

export default function AdminCarsPage() {
  const queryClient = useQueryClient();
  const [selectedCar, setSelectedCar] = useState<Car | null>(null);
  const [statusModalOpen, setStatusModalOpen] = useState(false);
  const [formModalOpen, setFormModalOpen] = useState(false);
  const [editingCar, setEditingCar] = useState<Car | null>(null);
  const [detailsCar, setDetailsCar] = useState<Car | null>(null);
  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [form] = Form.useForm<CarFormValues>();

  const [searchText, setSearchText] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [classFilter, setClassFilter] = useState<string>('all');

  const { data: cars, isLoading } = useQuery({
    queryKey: ['cars'],
    queryFn: () => carApi.getAll(),
  });

  const filteredCars = useMemo(() => {
    if (!cars) return [];
    return cars.filter(car => {
      if (statusFilter !== 'all' && car.availabilityStatus !== statusFilter) return false;
      if (classFilter !== 'all' && car.class !== classFilter) return false;
      if (searchText) {
        const q = searchText.toLowerCase();
        const searchable = [car.brand, car.model, car.licensePlate, car.vinCode].filter(Boolean).join(' ').toLowerCase();
        if (!searchable.includes(q)) return false;
      }
      return true;
    });
  }, [cars, searchText, statusFilter, classFilter]);

  const deleteMutation = useMutation({
    mutationFn: (id: string) => carApi.delete(id),
    onSuccess: () => {
      message.success('Автомобиль удалён');
      queryClient.invalidateQueries({ queryKey: ['cars'] });
    },
    onError: () => message.error('Ошибка при удалении'),
  });

  const statusMutation = useMutation({
    mutationFn: ({ id, action }: { id: string; action: (id: string) => Promise<unknown> }) => action(id),
    onSuccess: () => {
      message.success('Статус изменён');
      queryClient.invalidateQueries({ queryKey: ['cars'] });
      setStatusModalOpen(false);
    },
    onError: () => message.error('Ошибка при изменении статуса'),
  });

  const addMutation = useMutation({
    mutationFn: (data: AddCarRequest) => carApi.add(data),
    onSuccess: () => {
      message.success('Автомобиль добавлен');
      queryClient.invalidateQueries({ queryKey: ['cars'] });
      setFormModalOpen(false);
      form.resetFields();
    },
    onError: () => message.error('Ошибка при добавлении автомобиля'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateCarRequest }) => carApi.update(id, data),
    onSuccess: () => {
      message.success('Автомобиль обновлён');
      queryClient.invalidateQueries({ queryKey: ['cars'] });
      setFormModalOpen(false);
      setEditingCar(null);
      form.resetFields();
    },
    onError: () => message.error('Ошибка при обновлении автомобиля'),
  });

  useEffect(() => {
    if (!formModalOpen) return;

    if (editingCar) {
      const c = editingCar;
      form.setFieldsValue({
        brand: c.brand,
        model: c.model,
        generation: c.generation ?? undefined,
        variant: c.variant ?? undefined,
        isFacelift: c.isFacelift ?? false,
        releaseDate: c.releaseDate ? dayjs(c.releaseDate) : undefined,
        licensePlate: c.licensePlate,
        vinCode: c.vinCode,
        color: c.color ?? '',
        mileage: c.mileage ?? 0,
        fuelCurrentLiters: c.fuelCurrentLiters ?? null,
        fuelCapacityLiters: c.fuelCapacityLiters ?? null,
        batteryCurrentKWh: c.batteryCurrentKWh ?? null,
        batteryCapacityKWh: c.batteryCapacityKWh ?? null,
        bodyStyle: 'Sedan' as BodyStyle,
        transmissionType: (c.transmission as TransmissionType | undefined) ?? 'Manual',
        driveType: (c.driveType as DriveType | undefined) ?? 'Fwd',
        engineType: 'Gasoline' as EngineType,
        engineVolume: c.engineVolume ?? null,
        horsePower: c.horsePower ?? 0,
        powerReverse: c.powerReverse ?? null,
        pricePerHour: c.pricePerHour,
        carClass: c.class as CarClass,
        photoUrl: c.photoUrl ?? '',
      } as CarFormValues);
    } else {
      form.resetFields();
      form.setFieldsValue({
        isFacelift: false,
        bodyStyle: 'Sedan' as BodyStyle,
        transmissionType: 'Manual' as TransmissionType,
        driveType: 'Fwd' as DriveType,
        engineType: 'Gasoline' as EngineType,
        carClass: 'Economy' as CarClass,
        mileage: 0,
        horsePower: 0,
        pricePerHour: 0,
      } as Partial<CarFormValues>);
    }
  }, [formModalOpen, editingCar, form]);

  const handleSubmitForm = async () => {
    try {
      const values = await form.validateFields();
      if (editingCar) {
        updateMutation.mutate({ id: editingCar.id, data: toUpdateRequest(values) });
      } else {
        addMutation.mutate(toRequest(values));
      }
    } catch {
      // form validation already shows errors
    }
  };

  const openCreate = () => {
    setEditingCar(null);
    setFormModalOpen(true);
  };

  const openEdit = async (car: Car) => {
    try {
      const detailed = await carApi.getDetailed(car.id);
      setEditingCar(detailed);
      setFormModalOpen(true);
    } catch {
      setEditingCar(car);
      setFormModalOpen(true);
    }
  };

  const openDetails = async (car: Car) => {
    try {
      const detailed = await carApi.getDetailed(car.id);
      setDetailsCar(detailed);
      setDetailsModalOpen(true);
    } catch {
      setDetailsCar(car);
      setDetailsModalOpen(true);
    }
  };

  const columns = [
    {
      title: <Text style={{ color: '#888' }}>Автомобиль</Text>,
      key: 'car',
      width: 180,
      fixed: 'left' as const,
      render: (_: unknown, record: Car) => (
        <Text style={{ color: '#fff' }}>{record.brand} {record.model}</Text>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Госномер</Text>,
      dataIndex: 'licensePlate',
      key: 'plate',
      width: 130,
      render: (v: string) => <Text style={{ color: '#ccc' }}>{v || '—'}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>VIN</Text>,
      dataIndex: 'vinCode',
      key: 'vin',
      width: 200,
      ellipsis: true,
      render: (v: string) => <Text style={{ color: '#ccc', fontFamily: 'monospace' }}>{v || '—'}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Класс</Text>,
      dataIndex: 'class',
      key: 'class',
      width: 100,
      render: (v: string) => <Text style={{ color: '#ccc' }}>{v}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Пробег</Text>,
      key: 'mileage',
      width: 100,
      render: (_: unknown, record: Car) => (
        <Text style={{ color: '#ccc' }}>{record.mileage?.toLocaleString() ?? '—'} км</Text>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Цена/ч</Text>,
      key: 'price',
      width: 90,
      render: (_: unknown, record: Car) => (
        <Text style={{ color: '#f97316', fontWeight: 600 }}>{record.pricePerHour} Br</Text>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Статус</Text>,
      dataIndex: 'status',
      key: 'status',
      width: 150,
      render: (status: string) => (
        <Tag style={{ backgroundColor: statusColors[status], color: '#fff', border: 'none' }}>
          {statusLabels[status] ?? status}
        </Tag>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Действия</Text>,
      key: 'actions',
      width: 320,
      fixed: 'right' as const,
      render: (_: unknown, record: Car) => (
        <Space>
          <Button
            type="link"
            icon={<EyeOutlined />}
            style={{ color: '#a855f7' }}
            onClick={() => openDetails(record)}
          >
            Детали
          </Button>
          <Button
            type="link"
            icon={<EditOutlined />}
            style={{ color: '#3b82f6' }}
            onClick={() => openEdit(record)}
          >
            Изменить
          </Button>
          <Button
            type="link"
            icon={<EditOutlined />}
            style={{ color: '#f97316' }}
            onClick={() => {
              setSelectedCar(record);
              setStatusModalOpen(true);
            }}
            disabled={(nextActions[record.status] ?? []).length === 0}
          >
            Статус
          </Button>
          <Button
            type="link"
            danger
            icon={<DeleteOutlined />}
            onClick={() =>
              Modal.confirm({
                title: 'Удалить автомобиль?',
                content: `Удалить ${record.brand} ${record.model}?`,
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
    <div style={{ maxWidth: 1400, margin: '0 auto', padding: '32px' }}>
      <Space style={{ justifyContent: 'space-between', width: '100%', marginBottom: 24 }}>
        <Title level={2} style={{ color: '#fff', margin: 0 }}>Управление автомобилями</Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>
          Добавить авто
        </Button>
      </Space>

      <Space wrap style={{ marginBottom: 16, width: '100%', justifyContent: 'space-between' }}>
        <Space wrap>
          <Input.Search
            placeholder="Поиск по марке, модели, госномеру, VIN..."
            value={searchText}
            onChange={e => setSearchText(e.target.value)}
            onSearch={setSearchText}
            allowClear
            style={{ width: 360 }}
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
          <Select
            value={classFilter}
            onChange={v => setClassFilter(v)}
            style={{ width: 150 }}
            options={[
              { value: 'all', label: 'Все классы' },
              ...carClassOptions.map(v => ({ value: v, label: v })),
            ]}
          />
          <Button onClick={() => { setSearchText(''); setStatusFilter('all'); setClassFilter('all'); }}>
            Сбросить
          </Button>
        </Space>
        <Text style={{ color: '#888' }}>Найдено: {filteredCars.length}</Text>
      </Space>

      <div
        style={{
          background: '#1a1a1a',
          borderRadius: 12,
          border: '1px solid rgba(255,255,255,0.06)',
        }}
      >
        <Table
          dataSource={filteredCars}
          columns={columns}
          rowKey="id"
          pagination={{ pageSize: 10, showSizeChanger: true, pageSizeOptions: ['10', '20', '50'], showTotal: (t) => `Всего: ${t}` }}
          scroll={{ x: 1200 }}
          style={{ background: 'transparent' }}
        />
      </div>

      <Modal
        title={<Text style={{ color: '#fff' }}>Смена статуса: {selectedCar?.brand} {selectedCar?.model}</Text>}
        open={statusModalOpen}
        onCancel={() => setStatusModalOpen(false)}
        footer={null}
      >
        <div style={{ display: 'flex', flexDirection: 'column', gap: 16, width: '100%' }}>
          <div>
            <Text style={{ color: '#888' }}>Текущий статус: </Text>
            <Tag style={{ backgroundColor: selectedCar ? statusColors[selectedCar.status] : undefined, color: '#fff', border: 'none' }}>
              {selectedCar ? (statusLabels[selectedCar.status] ?? selectedCar.status) : ''}
            </Tag>
          </div>
          <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap' }}>
            {selectedCar &&
              (nextActions[selectedCar.status] ?? []).map((action) => (
                <Button
                  key={action.label}
                  style={{ backgroundColor: action.color, borderColor: action.color, color: '#fff' }}
                  onClick={() =>
                    statusMutation.mutate({ id: selectedCar.id, action: action.action })
                  }
                >
                  {action.label}
                </Button>
              ))}
          </div>
        </div>
      </Modal>

      <Modal
        title={editingCar ? `Редактирование: ${editingCar.brand} ${editingCar.model}` : 'Добавить автомобиль'}
        open={formModalOpen}
        onCancel={() => {
          setFormModalOpen(false);
          setEditingCar(null);
        }}
        onOk={handleSubmitForm}
        confirmLoading={addMutation.isPending || updateMutation.isPending}
        okText={editingCar ? 'Сохранить' : 'Добавить'}
        cancelText="Отмена"
        width={760}
        destroyOnHidden
      >
        <Form<CarFormValues>
          form={form}
          layout="vertical"
          style={{ marginTop: 16 }}
        >
          <Tabs
            items={[
              {
                key: 'main',
                label: 'Основное',
                children: (
                  <>
                    <Space.Compact style={{ width: '100%' }}>
                      <Form.Item name="brand" label="Марка" rules={[{ required: true, message: 'Укажите марку' }]} style={{ width: '50%' }}>
                        <Input placeholder="Toyota" />
                      </Form.Item>
                      <Form.Item name="model" label="Модель" rules={[{ required: true, message: 'Укажите модель' }]} style={{ width: '50%', marginLeft: 8 }}>
                        <Input placeholder="Camry" />
                      </Form.Item>
                    </Space.Compact>
                    <Space.Compact style={{ width: '100%' }}>
                      <Form.Item name="generation" label="Поколение" style={{ width: '40%' }}>
                        <Input placeholder="XV70" />
                      </Form.Item>
                      <Form.Item name="variant" label="Вариант" style={{ width: '40%', marginLeft: 8 }}>
                        <Input placeholder="Comfort" />
                      </Form.Item>
                      <Form.Item name="isFacelift" label="Рестайлинг" valuePropName="checked" style={{ width: '20%', marginLeft: 8, marginTop: 30 }}>
                        <Switch />
                      </Form.Item>
                    </Space.Compact>
                    <Form.Item
                      name="releaseDate"
                      label="Дата выпуска"
                      rules={[{ required: true, message: 'Укажите дату выпуска' }]}
                    >
                      <DatePicker style={{ width: '100%' }} />
                    </Form.Item>
                    <Form.Item
                      name="licensePlate"
                      label="Гос. номер (формат: 1234 AB-1)"
                      rules={[
                        { required: true, message: 'Укажите гос. номер' },
                        {
                          pattern: LICENSE_PLATE_REGEX,
                          message: 'Формат: 4 цифры, пробел, 2 латинские буквы, дефис, цифра 1-7',
                        },
                      ]}
                    >
                      <Input placeholder="1234 AB-1" />
                    </Form.Item>
                    <Form.Item
                      name="vinCode"
                      label="VIN (17 символов, без I/O/Q)"
                      rules={[
                        { required: true, message: 'Укажите VIN' },
                        { len: 17, message: 'VIN должен содержать 17 символов' },
                        { pattern: VIN_REGEX, message: 'Недопустимые символы в VIN' },
                      ]}
                    >
                      <Input placeholder="1HGCM82633A004352" style={{ fontFamily: 'monospace' }} />
                    </Form.Item>
                    <Space.Compact style={{ width: '100%' }}>
                      <Form.Item name="color" label="Цвет" rules={[{ required: true, message: 'Укажите цвет' }]} style={{ width: '50%' }}>
                        <Input placeholder="Чёрный" />
                      </Form.Item>
                      <Form.Item
                        name="mileage"
                        label="Пробег (км)"
                        rules={[{ required: true, message: 'Укажите пробег' }]}
                        style={{ width: '50%', marginLeft: 8 }}
                      >
                        <InputNumber min={0} style={{ width: '100%' }} />
                      </Form.Item>
                    </Space.Compact>
                    <Space.Compact style={{ width: '100%' }}>
                      <Form.Item
                        name="pricePerHour"
                        label="Цена за час (Br)"
                        rules={[{ required: true, message: 'Укажите цену' }]}
                        style={{ width: '50%' }}
                      >
                        <InputNumber min={0} step={0.5} style={{ width: '100%' }} />
                      </Form.Item>
                      <Form.Item
                        name="carClass"
                        label="Класс"
                        rules={[{ required: true, message: 'Выберите класс' }]}
                        style={{ width: '50%', marginLeft: 8 }}
                      >
                        <Select options={carClassOptions.map((v) => ({ value: v, label: v }))} />
                      </Form.Item>
                    </Space.Compact>
                    <Form.Item
                      name="photoUrl"
                      label="URL фото"
                      rules={[{ required: true, message: 'Укажите URL фото' }]}
                    >
                      <Input placeholder="https://..." />
                    </Form.Item>
                  </>
                ),
              },
              {
                key: 'tech',
                label: 'Двигатель и трансмиссия',
                children: (
                  <>
                    <Space.Compact style={{ width: '100%' }}>
                      <Form.Item
                        name="engineType"
                        label="Тип двигателя"
                        rules={[{ required: true, message: 'Выберите тип двигателя' }]}
                        style={{ width: '50%' }}
                      >
                        <Select options={engineTypeOptions.map((v) => ({ value: v, label: v }))} />
                      </Form.Item>
                      <Form.Item
                        name="engineVolume"
                        label="Объём (л)"
                        style={{ width: '50%', marginLeft: 8 }}
                      >
                        <InputNumber min={0} step={0.1} style={{ width: '100%' }} placeholder="2.5" />
                      </Form.Item>
                    </Space.Compact>
                    <Space.Compact style={{ width: '100%' }}>
                      <Form.Item
                        name="horsePower"
                        label="Лошадиные силы"
                        rules={[{ required: true, message: 'Укажите л.с.' }]}
                        style={{ width: '50%' }}
                      >
                        <InputNumber min={0} style={{ width: '100%' }} />
                      </Form.Item>
                      <Form.Item
                        name="powerReverse"
                        label="Мощность заднего хода"
                        style={{ width: '50%', marginLeft: 8 }}
                      >
                        <InputNumber min={0} step={0.1} style={{ width: '100%' }} />
                      </Form.Item>
                    </Space.Compact>
                    <Space.Compact style={{ width: '100%' }}>
                      <Form.Item
                        name="transmissionType"
                        label="Трансмиссия"
                        rules={[{ required: true, message: 'Выберите трансмиссию' }]}
                        style={{ width: '50%' }}
                      >
                        <Select options={transmissionOptions.map((v) => ({ value: v, label: v }))} />
                      </Form.Item>
                      <Form.Item
                        name="driveType"
                        label="Привод"
                        rules={[{ required: true, message: 'Выберите привод' }]}
                        style={{ width: '50%', marginLeft: 8 }}
                      >
                        <Select options={driveTypeOptions.map((v) => ({ value: v, label: v }))} />
                      </Form.Item>
                    </Space.Compact>
                    <Form.Item
                      name="bodyStyle"
                      label="Тип кузова"
                      rules={[{ required: true, message: 'Выберите тип кузова' }]}
                    >
                      <Select options={bodyStyleOptions.map((v) => ({ value: v, label: v }))} />
                    </Form.Item>
                  </>
                ),
              },
              {
                key: 'fuel',
                label: 'Топливо / Батарея',
                children: (
                  <>
                    <Divider style={{ color: '#888', borderColor: '#333' }} plain>Топливо</Divider>
                    <Space.Compact style={{ width: '100%' }}>
                      <Form.Item name="fuelCurrentLiters" label="Текущее (л)" style={{ width: '50%' }}>
                        <InputNumber min={0} step={0.1} style={{ width: '100%' }} />
                      </Form.Item>
                      <Form.Item name="fuelCapacityLiters" label="Ёмкость бака (л)" style={{ width: '50%', marginLeft: 8 }}>
                        <InputNumber min={0} step={0.1} style={{ width: '100%' }} />
                      </Form.Item>
                    </Space.Compact>
                    <Divider style={{ color: '#888', borderColor: '#333' }} plain>Батарея</Divider>
                    <Space.Compact style={{ width: '100%' }}>
                      <Form.Item name="batteryCurrentKWh" label="Текущий (кВт·ч)" style={{ width: '50%' }}>
                        <InputNumber min={0} step={0.1} style={{ width: '100%' }} />
                      </Form.Item>
                      <Form.Item name="batteryCapacityKWh" label="Ёмкость (кВт·ч)" style={{ width: '50%', marginLeft: 8 }}>
                        <InputNumber min={0} step={0.1} style={{ width: '100%' }} />
                      </Form.Item>
                    </Space.Compact>
                  </>
                ),
              },
            ]}
          />
          </Form>
        </Modal>

      <CarDetailsModal
        car={detailsCar}
        open={detailsModalOpen}
        onClose={() => setDetailsModalOpen(false)}
      />
    </div>
  );
}
