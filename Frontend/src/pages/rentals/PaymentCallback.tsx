import { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { Spin, Result, Typography } from 'antd';
import { CheckCircleOutlined, CloseCircleOutlined } from '@ant-design/icons';
import { paymentApi } from '../../api/endpoints';

const { Text } = Typography;

export default function PaymentCallback() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [status, setStatus] = useState<'processing' | 'success' | 'error'>('processing');

  useEffect(() => {
    const token = searchParams.get('token');
    const paymentStatus = searchParams.get('status');

    if (!token || paymentStatus !== 'successful') {
      setStatus('error');
      return;
    }

    paymentApi.confirm(token)
      .then((res) => {
        setStatus('success');
        setTimeout(() => navigate(`/my-rentals/${res.rentalId}?paid=1`, { replace: true }), 2000);
      })
      .catch(() => {
        setStatus('error');
      });
  }, [searchParams, navigate]);

  if (status === 'processing') {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', background: '#0a0a0a' }}>
        <div style={{ textAlign: 'center' }}>
          <Spin size="large" />
          <div style={{ marginTop: 24 }}>
            <Text style={{ color: '#888', fontSize: 16 }}>Обработка платежа...</Text>
          </div>
        </div>
      </div>
    );
  }

  if (status === 'success') {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', background: '#0a0a0a' }}>
        <Result
          status="success"
          icon={<CheckCircleOutlined style={{ color: '#22c55e', fontSize: 72 }} />}
          title={<Text style={{ color: '#fff', fontSize: 24 }}>Оплата прошла успешно</Text>}
          subTitle={<Text style={{ color: '#888' }}>Перенаправляем вас в аренду...</Text>}
        />
      </div>
    );
  }

  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', background: '#0a0a0a' }}>
      <Result
        status="error"
        icon={<CloseCircleOutlined style={{ color: '#ef4444', fontSize: 72 }} />}
        title={<Text style={{ color: '#fff', fontSize: 24 }}>Ошибка оплаты</Text>}
        subTitle={<Text style={{ color: '#888' }}>Платёж не был завершён. Попробуйте снова.</Text>}
      />
    </div>
  );
}
