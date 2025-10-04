export interface CardField<T = any> {
  key: keyof T | string;
  label: string;
  type: 'text' | 'number' | 'badge' | 'icon-value' | 'custom';
  icon?: string; // SVG path for icon-value type
  badgeConfig?: BadgeConfig;
  colorConfig?: ColorConfig;
  customRender?: (value: any, item: T) => string;
  visible?: boolean;
  suffix?: string; // e.g., '%', 'kg', 'years'
}

export interface BadgeConfig {
  colorMap: Record<string, string>; // value -> Tailwind classes
}

export interface ColorConfig {
  valueMap?: Record<any, string>; // value -> color classes
  rangeMap?: { min: number; max: number; color: string }[]; // for numeric ranges
}

export interface CardAction<T = any> {
  icon: string; // SVG path
  label: string;
  color: string; // Tailwind classes
  onClick: (item: T) => void;
  primary?: boolean; // If true, uses flex-1 and different styling
}

export interface CardConfig<T = any> {
  // Avatar/Header configuration
  avatarField?: keyof T | string;
  headerGradientField?: keyof T | string; // Field to determine gradient color
  headerBadgeField?: keyof T | string; // Field to show in header badge
  gradientMap?: Record<string, string>; // value -> Tailwind gradient classes

  // Main fields
  titleField: keyof T | string;
  subtitleFields?: CardField<T>[]; // Badges shown under title

  // Info fields (with icons)
  infoFields: CardField<T>[];

  // Actions
  actions?: CardAction<T>[];

  // Search configuration
  searchableFields?: (keyof T | string)[];
}
