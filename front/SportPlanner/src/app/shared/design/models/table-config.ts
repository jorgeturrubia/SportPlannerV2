export interface TableColumn<T = any> {
  key: keyof T | string;
  label: string;
  type?: 'text' | 'number' | 'date' | 'badge' | 'custom';
  width?: string; // e.g., 'w-16', 'min-w-[180px]', 'w-40'
  align?: 'left' | 'center' | 'right';
  sticky?: boolean;
  sortable?: boolean;
  badgeConfig?: BadgeConfig;
  customRender?: (value: any, row: T) => string;
}

export interface BadgeConfig {
  colorMap: Record<string, string>; // value -> Tailwind classes
}

export interface TableAction<T = any> {
  icon: string; // SVG path or icon name
  label: string;
  color: string; // Tailwind color classes
  onClick: (item: T) => void;
}

export interface TableConfig<T = any> {
  columns: TableColumn<T>[];
  actions?: TableAction<T>[];
  pageSize?: number;
  pageSizeOptions?: number[];
  searchable?: boolean;
  exportable?: boolean;
}
