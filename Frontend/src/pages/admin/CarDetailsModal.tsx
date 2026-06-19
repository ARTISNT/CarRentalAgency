import { Descriptions, Modal, Tag } from 'antd';
import dayjs from 'dayjs';
import type { Car } from '../../types';

interface Props {
  car: Car | null;
  open: boolean;
  onClose: () => void;
}

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
  Reserved: 'Забронирован',
};

export default function CarDetailsModal({ car, open, onClose }: Props) {
  if (!car) {
    return (
      <Modal title="Детали автомобиля" open={open} onCancel={onClose} footer={null} width={760}>
        <div style={{ padding: 24, textAlign: 'center', color: '#888' }}>
          Нет данных
        </div>
      </Modal>
    );
  }

  const statusKey = car.availabilityStatus;
  const statusLabel = statusLabels[statusKey] ?? car.availabilityStatus;
  const statusColor = statusColors[statusKey] ?? '#888';

  return (
    <Modal
      title={
        <span>
          {car.brand} {car.model}
          {car.generation ? ` (${car.generation})` : ''}
          {car.variant ? ` ${car.variant}` : ''}
        </span>
      }
      open={open}
      onCancel={onClose}
      footer={null}
      width={760}
    >
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 24, marginBottom: 16 }}>
        {car.photoUrl ? (
          <img
            src={car.photoUrl}
            alt={`${car.brand} ${car.model}`}
            style={{ width: '100%', borderRadius: 8, maxHeight: 280, objectFit: 'cover' }}
          />
        ) : (
          <div
            style={{
              width: '100%',
              height: 220,
              borderRadius: 8,
              background: '#111',
              color: '#666',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
            }}
          >
            Нет фото
          </div>
        )}
        <div>
          <Tag color={statusColor} style={{ marginBottom: 12, fontSize: 14 }}>
            {statusLabel}
          </Tag>
          <div style={{ fontSize: 22, fontWeight: 600, color: '#fff', marginBottom: 4 }}>
            {car.brand} {car.model}
          </div>
          <div style={{ fontSize: 14, color: '#888' }}>
            Класс: {car.class}
            {car.isFacelift ? ' · Рестайлинг' : ''}
          </div>
          <div style={{ fontSize: 14, color: '#888', marginTop: 8 }}>
            Гос. номер: <b style={{ color: '#fff' }}>{car.licensePlate}</b>
          </div>
          <div style={{ fontSize: 14, color: '#888' }}>
            VIN: <b style={{ color: '#fff', fontFamily: 'monospace' }}>{car.vinCode}</b>
          </div>
        </div>
      </div>

      <Descriptions
        title="Характеристики"
        bordered
        size="small"
        column={2}
        style={{ marginTop: 8 }}
        items={[
          { key: 'price', label: 'Цена за час', children: `${car.pricePerHour} Br` },
          {
            key: 'release',
            label: 'Год выпуска',
            children: car.releaseDate ? dayjs(car.releaseDate).format('DD.MM.YYYY') : '—',
          },
          { key: 'color', label: 'Цвет', children: car.color ?? '—' },
          {
            key: 'mileage',
            label: 'Пробег',
            children: car.mileage != null ? `${car.mileage.toLocaleString('ru-RU')} км` : '—',
          },
          { key: 'transmission', label: 'Трансмиссия', children: car.transmission ?? '—' },
          { key: 'driveType', label: 'Привод', children: car.driveType ?? '—' },
          {
            key: 'horsePower',
            label: 'Мощность (л.с.)',
            children: car.horsePower != null ? `${car.horsePower}` : '—',
          },
          {
            key: 'engineVolume',
            label: 'Объём двигателя (л)',
            children: car.engineVolume != null ? `${car.engineVolume}` : '—',
          },
          {
            key: 'powerReverse',
            label: 'Мощность (кВт)',
            children: car.powerReverse != null ? `${car.powerReverse}` : '—',
          },
          {
            key: 'fuelCap',
            label: 'Объём бака (л)',
            children: car.fuelCapacityLiters != null ? `${car.fuelCapacityLiters}` : '—',
          },
          {
            key: 'fuelCur',
            label: 'Текущее топливо (л)',
            children: car.fuelCurrentLiters != null ? `${car.fuelCurrentLiters}` : '—',
          },
          {
            key: 'batteryCap',
            label: 'Ёмкость батареи (кВт·ч)',
            children: car.batteryCapacityKWh != null ? `${car.batteryCapacityKWh}` : '—',
          },
          {
            key: 'batteryCur',
            label: 'Текущий заряд (кВт·ч)',
            children: car.batteryCurrentKWh != null ? `${car.batteryCurrentKWh}` : '—',
          },
        ]}
      />
    </Modal>
  );
}
