import { useMemo, useRef, useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Table,
  Tag,
  Typography,
  Spin,
  Button,
  Space,
  Modal,
  message,
  Form,
  Input,
  Select,
  Divider,
} from 'antd';
import type { TextAreaRef } from 'antd/es/input/TextArea';
import {
  PlusOutlined,
  EditOutlined,
  PlayCircleOutlined,
  PauseCircleOutlined,
  FrownOutlined,
} from '@ant-design/icons';
import { templateApi } from '../../api/endpoints';
import type { ContractTemplate, DocumentType } from '../../types';
import TemplateVariablesPanel from './TemplateVariablesPanel';
import dayjs from 'dayjs';

const { Title, Text } = Typography;

export default function AdminTemplatesPage() {
  const queryClient = useQueryClient();
  const [createOpen, setCreateOpen] = useState(false);
  const [editContentOpen, setEditContentOpen] = useState(false);
  const [renameOpen, setRenameOpen] = useState(false);
  const [selectedTemplate, setSelectedTemplate] = useState<ContractTemplate | null>(null);
  const [searchText, setSearchText] = useState('');
  const [typeFilter, setTypeFilter] = useState<DocumentType | 'all'>('all');
  const [activeFilter, setActiveFilter] = useState<'all' | 'active' | 'inactive'>('all');

  const [createForm] = Form.useForm<{ name: string; documentType: DocumentType; content: string }>();
  const [editForm] = Form.useForm<{ content: string }>();
  const createTextareaRef = useRef<TextAreaRef | null>(null);
  const editTextareaRef = useRef<TextAreaRef | null>(null);
  const [createVariablesType, setCreateVariablesType] = useState<DocumentType | null>(null);

  const { data: templates, isLoading, isError, error } = useQuery({
    queryKey: ['templates'],
    queryFn: () => templateApi.getAll(),
  });

  const filteredTemplates = useMemo(() => {
    if (!templates) return [];
    return templates.filter(t => {
      if (typeFilter !== 'all' && t.documentType !== typeFilter) return false;
      if (activeFilter === 'active' && !t.isActive) return false;
      if (activeFilter === 'inactive' && t.isActive) return false;
      if (searchText) {
        const q = searchText.toLowerCase();
        if (!t.name.toLowerCase().includes(q)) return false;
      }
      return true;
    });
  }, [templates, searchText, typeFilter, activeFilter]);

  const apiError = isError
    ? (error as { response?: { status?: number } })?.response?.status === 403
      ? 'Нет доступа к шаблонам. Обратитесь к администратору для получения права ChangeContractTemplates.'
      : 'Ошибка загрузки шаблонов'
    : null;

  const createMutation = useMutation({
    mutationFn: (data: { name: string; content: string; documentType: DocumentType }) =>
      templateApi.create(data),
    onSuccess: () => {
      message.success('Шаблон создан');
      queryClient.invalidateQueries({ queryKey: ['templates'] });
      setCreateOpen(false);
    },
    onError: () => message.error('Ошибка при создании'),
  });

  const updateContentMutation = useMutation({
    mutationFn: (data: { id: string; content: string }) =>
      templateApi.updateContent(data),
    onSuccess: () => {
      message.success('Содержимое обновлено');
      queryClient.invalidateQueries({ queryKey: ['templates'] });
      setEditContentOpen(false);
    },
    onError: () => message.error('Ошибка при обновлении'),
  });

  const renameMutation = useMutation({
    mutationFn: (data: { id: string; name: string }) =>
      templateApi.rename(data),
    onSuccess: () => {
      message.success('Шаблон переименован');
      queryClient.invalidateQueries({ queryKey: ['templates'] });
      setRenameOpen(false);
    },
    onError: () => message.error('Ошибка при переименовании'),
  });

  const activateMutation = useMutation({
    mutationFn: (id: string) => templateApi.activate(id),
    onSuccess: () => {
      message.success('Шаблон активирован');
      queryClient.invalidateQueries({ queryKey: ['templates'] });
    },
    onError: () => message.error('Ошибка при активации'),
  });

  const deactivateMutation = useMutation({
    mutationFn: (id: string) => templateApi.deactivate(id),
    onSuccess: () => {
      message.success('Шаблон деактивирован');
      queryClient.invalidateQueries({ queryKey: ['templates'] });
    },
    onError: () => message.error('Ошибка при деактивации'),
  });

  const columns = [
    {
      title: <Text style={{ color: '#888' }}>Название</Text>,
      dataIndex: 'name',
      key: 'name',
      render: (v: string) => <Text style={{ color: '#fff' }}>{v}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Версия</Text>,
      dataIndex: 'version',
      key: 'version',
      width: 80,
      render: (v: number) => <Text style={{ color: '#ccc' }}>v{v}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Тип</Text>,
      dataIndex: 'documentType',
      key: 'type',
      width: 130,
      render: (v: DocumentType) => {
        const labels: Record<DocumentType, string> = {
          Contract: 'Аренда',
          ReturnAct: 'Возврат',
          Addition: 'Доп.',
        };
        return <Text style={{ color: '#ccc' }}>{labels[v] || v}</Text>;
      },
    },
    {
      title: <Text style={{ color: '#888' }}>Статус</Text>,
      dataIndex: 'isActive',
      key: 'active',
      width: 110,
      render: (v: boolean) => (
        <Tag style={{ backgroundColor: v ? '#22c55e' : '#666', color: '#fff', border: 'none' }}>
          {v ? 'Активен' : 'Неактивен'}
        </Tag>
      ),
    },
    {
      title: <Text style={{ color: '#888' }}>Создан</Text>,
      dataIndex: 'createdOn',
      key: 'createdOn',
      render: (v: string) => <Text style={{ color: '#ccc' }}>{dayjs(v).format('DD.MM.YYYY')}</Text>,
    },
    {
      title: <Text style={{ color: '#888' }}>Действия</Text>,
      key: 'actions',
      width: 280,
      render: (_: unknown, record: ContractTemplate) => (
        <Space>
          <Button
            type="link"
            style={{ color: '#f97316' }}
            icon={<EditOutlined />}
            onClick={() => {
              setSelectedTemplate(record);
              setEditContentOpen(true);
            }}
          >
            Контент
          </Button>
          <Button
            type="link"
            style={{ color: '#3b82f6' }}
            onClick={() => {
              setSelectedTemplate(record);
              setRenameOpen(true);
            }}
          >
            Переименовать
          </Button>
          {record.isActive ? (
            <Button
              type="link"
              icon={<PauseCircleOutlined />}
              style={{ color: '#f97316' }}
              onClick={() =>
                Modal.confirm({
                  title: 'Деактивировать шаблон?',
                  onOk: () => deactivateMutation.mutate(record.id),
                })
              }
            >
              Деакт.
            </Button>
          ) : (
            <Button
              type="link"
              icon={<PlayCircleOutlined />}
              style={{ color: '#22c55e' }}
              onClick={() =>
                Modal.confirm({
                  title: 'Активировать шаблон?',
                  onOk: () => activateMutation.mutate(record.id),
                })
              }
            >
              Акт.
            </Button>
          )}
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
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 24 }}>
        <Title level={2} style={{ color: '#fff', margin: 0 }}>Управление шаблонами</Title>
        {!apiError && (
          <Button type="primary" icon={<PlusOutlined />} onClick={() => setCreateOpen(true)}>
            Создать шаблон
          </Button>
        )}
      </div>

      {apiError ? (
        <div style={{
          textAlign: 'center', padding: 80, background: '#1a1a1a', borderRadius: 12,
          border: '1px solid rgba(255,255,255,0.06)',
        }}>
          <FrownOutlined style={{ fontSize: 48, color: '#ef4444', marginBottom: 16 }} />
          <div style={{ color: '#ccc', fontSize: 16 }}>{apiError}</div>
        </div>
      ) : (
        <>
          <Space wrap style={{ marginBottom: 16, width: '100%', justifyContent: 'space-between' }}>
            <Space wrap>
              <Input.Search
                placeholder="Поиск по названию..."
                value={searchText}
                onChange={e => setSearchText(e.target.value)}
                onSearch={setSearchText}
                allowClear
                style={{ width: 300 }}
              />
              <Select
                value={typeFilter}
                onChange={v => setTypeFilter(v)}
                style={{ width: 150 }}
                options={[
                  { value: 'all', label: 'Все типы' },
                  { value: 'Contract', label: 'Аренда' },
                  { value: 'ReturnAct', label: 'Возврат' },
                  { value: 'Addition', label: 'Дополнительный' },
                ]}
              />
              <Select
                value={activeFilter}
                onChange={v => setActiveFilter(v)}
                style={{ width: 140 }}
                options={[
                  { value: 'all', label: 'Все' },
                  { value: 'active', label: 'Активен' },
                  { value: 'inactive', label: 'Неактивен' },
                ]}
              />
              <Button onClick={() => { setSearchText(''); setTypeFilter('all'); setActiveFilter('all'); }}>
                Сбросить
              </Button>
            </Space>
            <Text style={{ color: '#888' }}>Найдено: {filteredTemplates.length}</Text>
          </Space>

          <div style={{ background: '#1a1a1a', borderRadius: 12, border: '1px solid rgba(255,255,255,0.06)', overflow: 'hidden' }}>
            <Table
              dataSource={filteredTemplates}
              columns={columns}
              rowKey="id"
              pagination={{ pageSize: 10 }}
              scroll={{ x: 900 }}
              style={{ background: 'transparent' }}
            />
          </div>
        </>
      )}

      <Modal
        title={<Text style={{ color: '#fff' }}>Создать шаблон</Text>}
        open={createOpen}
        onCancel={() => {
          setCreateOpen(false);
          setCreateVariablesType(null);
        }}
        footer={null}
        width={900}
      >
        <Form
          form={createForm}
          layout="vertical"
          onFinish={(values) => createMutation.mutate(values)}
          onValuesChange={(changed) => {
            if ('documentType' in changed) {
              setCreateVariablesType(changed.documentType as DocumentType);
            }
          }}
          style={{ marginTop: 16 }}
        >
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 24 }}>
            <div>
              <Form.Item
                name="name"
                label={<Text style={{ color: '#ccc' }}>Название</Text>}
                rules={[{ required: true, message: 'Введите название' }]}
              >
                <Input style={{ background: '#111', color: '#fff', borderColor: '#333' }} />
              </Form.Item>
              <Form.Item
                name="documentType"
                label={<Text style={{ color: '#ccc' }}>Тип документа</Text>}
                rules={[{ required: true, message: 'Выберите тип' }]}
              >
                <Select
                  style={{ background: '#111', color: '#fff' }}
                  options={[
                    { value: 'Contract', label: 'Аренда' },
                    { value: 'ReturnAct', label: 'Возврат' },
                    { value: 'Addition', label: 'Дополнительный' },
                  ]}
                />
              </Form.Item>
              <Form.Item
                name="content"
                label={<Text style={{ color: '#ccc' }}>Содержимое</Text>}
                rules={[{ required: true, message: 'Введите содержимое' }]}
              >
                <Input.TextArea
                  rows={14}
                  ref={(instance) => {
                    createTextareaRef.current = instance;
                  }}
                  style={{ background: '#111', color: '#fff', borderColor: '#333' }}
                />
              </Form.Item>
            </div>
            <div>
              <Text style={{ color: '#ccc', fontWeight: 600, display: 'block', marginBottom: 12 }}>
                Доступные переменные
              </Text>
              {createVariablesType ? (
                <TemplateVariablesPanel
                  documentType={createVariablesType}
                  textareaRef={createTextareaRef}
                  onInsert={(newValue) => {
                    createForm.setFieldValue('content', newValue);
                  }}
                />
              ) : (
                <Text style={{ color: '#888', fontSize: 13 }}>
                  Сначала выберите тип документа, чтобы увидеть доступные переменные.
                </Text>
              )}
            </div>
          </div>
          <Divider style={{ borderColor: 'rgba(255,255,255,0.06)' }} />
          <Form.Item>
            <Button type="primary" htmlType="submit" loading={createMutation.isPending}>
              Создать
            </Button>
          </Form.Item>
        </Form>
      </Modal>

      <Modal
        title={<Text style={{ color: '#fff' }}>Редактировать содержимое</Text>}
        open={editContentOpen}
        destroyOnClose
        onCancel={() => { setEditContentOpen(false); setSelectedTemplate(null); }}
        footer={null}
        width={900}
      >
        <Form
          form={editForm}
          layout="vertical"
          initialValues={{ content: selectedTemplate?.content }}
          onFinish={(values) => {
            if (selectedTemplate) {
              updateContentMutation.mutate({ id: selectedTemplate.id, content: values.content });
            }
          }}
          style={{ marginTop: 16 }}
        >
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 24 }}>
            <div>
              <Form.Item
                name="content"
                label={<Text style={{ color: '#ccc' }}>Содержимое</Text>}
                rules={[{ required: true, message: 'Введите содержимое' }]}
              >
                <Input.TextArea
                  rows={18}
                  ref={(instance) => {
                    editTextareaRef.current = instance;
                  }}
                  style={{ background: '#111', color: '#fff', borderColor: '#333' }}
                />
              </Form.Item>
            </div>
            <div>
              <Text style={{ color: '#ccc', fontWeight: 600, display: 'block', marginBottom: 12 }}>
                Доступные переменные
              </Text>
              {selectedTemplate ? (
                <TemplateVariablesPanel
                  documentType={selectedTemplate.documentType}
                  textareaRef={editTextareaRef}
                  onInsert={(newValue) => {
                    editForm.setFieldValue('content', newValue);
                  }}
                />
              ) : null}
            </div>
          </div>
          <Divider style={{ borderColor: 'rgba(255,255,255,0.06)' }} />
          <Form.Item>
            <Button type="primary" htmlType="submit" loading={updateContentMutation.isPending}>
              Сохранить
            </Button>
          </Form.Item>
        </Form>
      </Modal>

      <Modal
        title={<Text style={{ color: '#fff' }}>Переименовать шаблон</Text>}
        open={renameOpen}
        onCancel={() => { setRenameOpen(false); setSelectedTemplate(null); }}
        footer={null}
      >
        <Form
          layout="vertical"
          initialValues={{ name: selectedTemplate?.name }}
          onFinish={(values) => {
            if (selectedTemplate) {
              renameMutation.mutate({ id: selectedTemplate.id, name: values.name });
            }
          }}
          style={{ marginTop: 16 }}
        >
          <Form.Item
            name="name"
            label={<Text style={{ color: '#ccc' }}>Название</Text>}
            rules={[{ required: true, message: 'Введите название' }]}
          >
            <Input style={{ background: '#111', color: '#fff', borderColor: '#333' }} />
          </Form.Item>
          <Form.Item>
            <Button type="primary" htmlType="submit" loading={renameMutation.isPending}>
              Сохранить
            </Button>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
