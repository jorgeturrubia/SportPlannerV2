export type NotificationLevel = 'success' | 'info' | 'warning' | 'error';

export interface Notification {
  id: string;
  level: NotificationLevel;
  title?: string;
  message: string;
  autoClose: boolean;
  durationMs: number;
  createdAt: number;
}
