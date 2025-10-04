import { Component, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface ConfirmationConfig {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  confirmColor?: 'danger' | 'primary' | 'warning';
}

@Component({
  selector: 'app-confirmation-dialog',
  imports: [CommonModule],
  templateUrl: './confirmation-dialog.html',
  styleUrl: './confirmation-dialog.css'
})
export class ConfirmationDialog {
  // Inputs
  isOpen = input.required<boolean>();
  config = input.required<ConfirmationConfig>();

  // Outputs
  confirm = output<void>();
  cancel = output<void>();

  // Internal state
  isClosing = signal(false);

  onConfirm(): void {
    this.isClosing.set(true);
    setTimeout(() => {
      this.confirm.emit();
      this.isClosing.set(false);
    }, 150);
  }

  onCancel(): void {
    this.isClosing.set(true);
    setTimeout(() => {
      this.cancel.emit();
      this.isClosing.set(false);
    }, 150);
  }

  onBackdropClick(): void {
    this.onCancel();
  }

  getConfirmButtonClasses(): string {
    const color = this.config().confirmColor || 'danger';
    const baseClasses = 'px-4 py-2 rounded-lg font-medium transition-colors duration-150 focus:outline-none focus:ring-2 focus:ring-offset-2';

    switch (color) {
      case 'danger':
        return `${baseClasses} bg-red-600 text-white hover:bg-red-700 focus:ring-red-500`;
      case 'primary':
        return `${baseClasses} bg-blue-600 text-white hover:bg-blue-700 focus:ring-blue-500`;
      case 'warning':
        return `${baseClasses} bg-orange-600 text-white hover:bg-orange-700 focus:ring-orange-500`;
      default:
        return `${baseClasses} bg-red-600 text-white hover:bg-red-700 focus:ring-red-500`;
    }
  }
}
