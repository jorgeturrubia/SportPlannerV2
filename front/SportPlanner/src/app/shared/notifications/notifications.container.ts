import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { NotificationService } from './notification.service';

@Component({
  selector: 'sp-notifications',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="fixed top-4 right-4 space-y-2 z-50 w-96 max-w-full">
      <ng-container *ngFor="let n of ns.notifications()">
        <div [ngClass]="cardClass(n.level)"
             class="flex items-start gap-3 p-3 rounded shadow-lg border">
          <div class="flex-1">
            <div class="flex justify-between items-start gap-2">
              <div class="font-semibold text-sm">{{ n.title || (n.level | titlecase) }}</div>
              <button (click)="close(n.id)" class="text-sm opacity-70 hover:opacity-100">✕</button>
            </div>
            <div class="text-sm mt-1 whitespace-pre-wrap">{{ n.message }}</div>
          </div>
        </div>
      </ng-container>
    </div>
  `
})
export class NotificationsContainerComponent {
  constructor(public ns: NotificationService) {}

  cardClass(level: string) {
    // Devuelvo clases como string para simplicidad (Angular aceptará un objeto o string)
    switch (level) {
      case 'success': return 'bg-green-50 border border-green-200 text-green-800';
      case 'info': return 'bg-blue-50 border border-blue-200 text-blue-800';
      case 'warning': return 'bg-yellow-50 border border-yellow-200 text-yellow-800';
      case 'error': return 'bg-red-50 border border-red-200 text-red-800';
      default: return 'bg-white border border-gray-200 text-gray-800';
    }
  }

  close(id: string) {
    this.ns.remove(id);
  }
}
