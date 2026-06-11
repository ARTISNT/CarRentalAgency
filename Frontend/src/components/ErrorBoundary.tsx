import { Component } from 'react';
import { Typography, Button } from 'antd';

const { Title, Text } = Typography;

interface Props {
  children: React.ReactNode;
}

interface State {
  hasError: boolean;
  error: Error | null;
}

export default class ErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error };
  }

  render() {
    if (this.state.hasError) {
      return (
        <div
          style={{
            minHeight: '100vh',
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'center',
            background: '#0a0a0a',
            padding: 32,
            gap: 16,
          }}
        >
          <Title level={3} style={{ color: '#fff', margin: 0 }}>Что-то пошло не так</Title>
          <Text style={{ color: '#888', textAlign: 'center' }}>
            Произошла ошибка при загрузке страницы.
          </Text>
          <Button
            type="primary"
            onClick={() => {
              this.setState({ hasError: false, error: null });
              window.location.href = '/';
            }}
          >
            Вернуться на главную
          </Button>
        </div>
      );
    }

    return this.props.children;
  }
}