import { useEffect, useRef, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Card,
  Typography,
  Spin,
  Button,
  Descriptions,
  Tag,
  message,
  Space,
  Row,
  Col,
  Modal,
} from 'antd';
import {
  ArrowLeftOutlined,
  CheckCircleOutlined,
  CloseOutlined,
  FileTextOutlined,
} from '@ant-design/icons';
import SignatureCanvas from 'react-signature-canvas';
import dayjs from 'dayjs';
import { contractApi } from '../../api/endpoints';
import apiClient from '../../api/client';
import type { ContractResponse, ContractStatus } from '../../types';

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

export default function ContractSignPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const sigPadRef = useRef<SignatureCanvas>(null);
  const [pdfUrl, setPdfUrl] = useState<string | null>(null);
  const pdfBlobUrlRef = useRef<string | null>(null);

  const { data: contract, isLoading } = useQuery({
    queryKey: ['contract', id],
    queryFn: () => contractApi.getById(id!),
    enabled: !!id,
  });

  const isSigned = contract?.status === 'Active' || contract?.status === 'Ended';

  useEffect(() => {
    if (!id) return;

    const url = isSigned
      ? `/Contract/get-contract-${id}/pdf?signed=true`
      : `/Contract/get-contract-${id}/pdf`;

    apiClient.get<Blob>(url, { responseType: 'blob' })
      .then(response => {
        const blob = new Blob([response.data], { type: 'application/pdf' });
        const objectUrl = URL.createObjectURL(blob);
        pdfBlobUrlRef.current = objectUrl;
        setPdfUrl(objectUrl);
      })
      .catch(() => {});

    return () => {
      if (pdfBlobUrlRef.current) {
        URL.revokeObjectURL(pdfBlobUrlRef.current);
        pdfBlobUrlRef.current = null;
      }
    };
  }, [id, isSigned]);

  const signMutation = useMutation({
    mutationFn: () => {
      const sigPad = sigPadRef.current;
      if (!sigPad || sigPad.isEmpty()) {
        message.error('Пожалуйста, поставьте подпись');
        throw new Error('No signature');
      }
      const signatureBase64 = sigPad.toDataURL('image/png');
      return contractApi.sign({ id: id!, signatureBase64 });
    },
    onSuccess: () => {
      message.success('Договор подписан');
      queryClient.invalidateQueries({ queryKey: ['contract', id] });
      queryClient.invalidateQueries({ queryKey: ['my-contracts'] });
    },
    onError: (err: Error) => {
      if (err.message !== 'No signature') {
        message.error('Ошибка при подписании договора');
      }
    },
  });

  const handleSign = () => {
    Modal.confirm({
      title: 'Подписать договор?',
      content: 'Вы подтверждаете, что ознакомились с условиями договора и согласны с ними.',
      onOk: () => signMutation.mutate(),
    });
  };

  const handleClear = () => {
    sigPadRef.current?.clear();
  };

  if (isLoading) {
    return (
      <div style={{ textAlign: 'center', padding: 120 }}>
        <Spin size="large" />
      </div>
    );
  }

  if (!contract) {
    return <div style={{ textAlign: 'center', padding: 120, color: '#888' }}>Договор не найден</div>;
  }

  return (
    <div style={{ maxWidth: 1000, margin: '0 auto', padding: '32px' }}>
      <Button
        type="text"
        icon={<ArrowLeftOutlined />}
        onClick={() => navigate('/my-contracts')}
        style={{ color: '#888', marginBottom: 16, padding: 0 }}
      >
        Назад к договорам
      </Button>

      <Title level={3} style={{ color: '#fff', marginBottom: 24 }}>
        <FileTextOutlined style={{ marginRight: 8, color: '#f97316' }} />
        Подписание договора
      </Title>

      <Row gutter={24}>
        <Col xs={24} lg={14}>
          <Card
            style={{
              background: '#1a1a1a',
              border: '1px solid rgba(255,255,255,0.06)',
              marginBottom: 24,
            }}
          >
            <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <Title level={5} style={{ color: '#fff', margin: 0 }}>
                  Договор #{contract.id.slice(0, 8)}
                </Title>
                <Tag style={{ backgroundColor: statusColors[contract.status as ContractStatus], color: '#fff', border: 'none' }}>
                  {statusLabels[contract.status as ContractStatus]}
                </Tag>
              </div>

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
                  {dayjs(contract.startDate).format('DD.MM.YYYY HH:mm')}
                </Descriptions.Item>
                <Descriptions.Item label="Окончание">
                  {dayjs(contract.endDate).format('DD.MM.YYYY HH:mm')}
                </Descriptions.Item>
                <Descriptions.Item label="Стоимость">
                  {contract.estimatedPrice.toFixed(2)} Br
                </Descriptions.Item>
                <Descriptions.Item label="Создан">
                  {dayjs(contract.createdAt).format('DD.MM.YYYY HH:mm')}
                </Descriptions.Item>
              </Descriptions>

              {pdfUrl ? (
                <iframe
                  src={pdfUrl}
                  style={{
                    width: '100%',
                    height: 500,
                    border: '1px solid rgba(255,255,255,0.1)',
                    borderRadius: 8,
                    background: '#fff',
                  }}
                  title="PDF договора"
                />
              ) : (
                <div style={{ textAlign: 'center', padding: 40, color: '#666' }}>
                  {contract.pdfPath ? 'Загрузка PDF...' : 'PDF не сгенерирован'}
                </div>
              )}
            </div>
          </Card>
        </Col>

        <Col xs={24} lg={10}>
          <Card
            style={{
              background: '#1a1a1a',
              border: '1px solid rgba(255,255,255,0.06)',
              marginBottom: 24,
            }}
          >
            <Title level={5} style={{ color: '#fff', marginBottom: 16 }}>
              Подпись
            </Title>

            <div
              style={{
                border: '2px dashed rgba(255,255,255,0.2)',
                borderRadius: 8,
                background: '#fff',
                marginBottom: 16,
                cursor: 'crosshair',
              }}
            >
              <SignatureCanvas
                ref={sigPadRef}
                penColor="#000"
                canvasProps={{
                  width: 400,
                  height: 200,
                  style: { width: '100%', height: 200, borderRadius: 6 },
                }}
                clearOnResize={false}
              />
            </div>

            <Space style={{ width: '100%', justifyContent: 'space-between' }}>
              <Button icon={<CloseOutlined />} onClick={handleClear} disabled={signMutation.isPending}>
                Очистить
              </Button>
              <Button
                type="primary"
                icon={<CheckCircleOutlined />}
                onClick={handleSign}
                loading={signMutation.isPending}
                size="large"
              >
                Подписать договор
              </Button>
            </Space>

            <Text style={{ color: '#666', display: 'block', marginTop: 12, fontSize: 13 }}>
              Нарисуйте подпись в поле выше с помощью мыши или сенсорного ввода
            </Text>
          </Card>
        </Col>
      </Row>
    </div>
  );
}
