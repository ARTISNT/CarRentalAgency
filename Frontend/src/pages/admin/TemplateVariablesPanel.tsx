import { useQuery } from '@tanstack/react-query';
import { Empty, Skeleton, Space, Tag, Tooltip, Typography } from 'antd';
import type { TextAreaRef } from 'antd/es/input/TextArea';
import { templateApi } from '../../api/endpoints';
import type { DocumentType } from '../../types';

const { Text } = Typography;

interface Props {
  documentType: DocumentType;
  textareaRef: React.RefObject<TextAreaRef | null>;
  onInsert: (newValue: string) => void;
}

export default function TemplateVariablesPanel({ documentType, textareaRef, onInsert }: Props) {
  const { data, isLoading, isError } = useQuery({
    queryKey: ['templateVariables', documentType],
    queryFn: () => templateApi.getVariables(documentType),
    staleTime: 5 * 60 * 1000,
  });

  const handleClick = (key: string) => {
    const placeholder = `{{${key}}}`;
    const ta = textareaRef.current?.resizableTextArea?.textArea ?? null;
    if (!ta) {
      onInsert(placeholder);
      return;
    }
    const start = ta.selectionStart ?? ta.value.length;
    const end = ta.selectionEnd ?? ta.value.length;
    const before = ta.value.slice(0, start);
    const after = ta.value.slice(end);
    onInsert(`${before}${placeholder}${after}`);
    requestAnimationFrame(() => {
      ta.focus();
      const cursor = start + placeholder.length;
      ta.setSelectionRange(cursor, cursor);
    });
  };

  if (isLoading) {
    return <Skeleton active paragraph={{ rows: 4 }} />;
  }

  if (isError || !data) {
    return <Empty description="Не удалось загрузить переменные" />;
  }

  const groups = groupBy(data, (v) => v.group);

  return (
    <Space direction="vertical" size={12} style={{ width: '100%' }}>
      <Text style={{ color: '#888', fontSize: 12 }}>
        Кликните по переменной, чтобы вставить её в позицию курсора.
      </Text>
      {Array.from(groups.entries()).map(([group, vars]) => (
        <div key={group}>
          <Text style={{ color: '#ccc', fontWeight: 600, display: 'block', marginBottom: 6 }}>
            {group}
          </Text>
          <Space wrap size={[6, 6]}>
            {vars.map((v) => (
              <Tooltip
                key={v.key}
                title={
                  <Space direction="vertical" size={2}>
                    <span>{v.description}</span>
                    <span style={{ opacity: 0.7 }}>Пример: {v.example}</span>
                  </Space>
                }
              >
                <Tag
                  color="blue"
                  style={{ cursor: 'pointer', userSelect: 'none' }}
                  onClick={() => handleClick(v.key)}
                >
                  {`{{${v.key}}}`}
                </Tag>
              </Tooltip>
            ))}
          </Space>
        </div>
      ))}
    </Space>
  );
}

function groupBy<T, K>(arr: T[], keyFn: (t: T) => K): Map<K, T[]> {
  const map = new Map<K, T[]>();
  for (const item of arr) {
    const k = keyFn(item);
    const list = map.get(k) ?? [];
    list.push(item);
    map.set(k, list);
  }
  return map;
}
