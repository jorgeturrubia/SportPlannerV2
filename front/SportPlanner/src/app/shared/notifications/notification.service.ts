import { Injectable, signal, Signal } from '@angular/core';
import { Notification, NotificationLevel } from './notification.model';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private _notifications = signal<Notification[]>([]);
  get notifications(): Signal<Notification[]> { return this._notifications; }

  private timeouts = new Map<string, ReturnType<typeof setTimeout>>();

  private create(level: NotificationLevel, message: string, title?: string, autoClose = true, durationMs = 5000) {
    const id = (globalThis as any).crypto?.randomUUID?.() ?? Math.random().toString(36).slice(2);
    const n: Notification = {
      id,
      level,
      title,
      message,
      autoClose,
      durationMs,
      createdAt: Date.now()
    };
    this._notifications.update(arr => [n, ...arr]);

    if (autoClose) {
      const t = setTimeout(() => this.remove(id), durationMs);
      this.timeouts.set(id, t);
    }

    return id;
  }

  success(message: string, title?: string, autoClose = true, durationMs = 4000) {
    return this.create('success', message, title, autoClose, durationMs);
  }
  info(message: string, title?: string, autoClose = true, durationMs = 4000) {
    return this.create('info', message, title, autoClose, durationMs);
  }
  warning(message: string, title?: string, autoClose = true, durationMs = 6000) {
    return this.create('warning', message, title, autoClose, durationMs);
  }
  error(message: string, title?: string, autoClose = false, durationMs = 8000) {
    return this.create('error', message, title, autoClose, durationMs);
  }

  add(notification: Notification) {
    this._notifications.update(arr => [notification, ...arr]);
    if (notification.autoClose) {
      const t = setTimeout(() => this.remove(notification.id), notification.durationMs);
      this.timeouts.set(notification.id, t);
    }
  }

  remove(id: string) {
    const t = this.timeouts.get(id);
    if (t) {
      clearTimeout(t);
      this.timeouts.delete(id);
    }
    this._notifications.update(arr => arr.filter(n => n.id !== id));
  }

  clear() {
    for (const t of this.timeouts.values()) clearTimeout(t);
    this.timeouts.clear();
    this._notifications.set([]);
  }
}
