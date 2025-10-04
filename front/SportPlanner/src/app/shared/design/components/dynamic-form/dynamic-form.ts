import { Component, input, output, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormConfig, FormField } from '../../models/form-config';

@Component({
  selector: 'app-dynamic-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './dynamic-form.html',
  styleUrl: './dynamic-form.css'
})
export class DynamicForm implements OnInit {
  // Inputs
  isOpen = input.required<boolean>();
  config = input.required<FormConfig>();
  title = input.required<string>();
  initialData = input<any>(null);

  // Outputs
  submit = output<any>();
  cancel = output<void>();

  // Internal state
  formGroup = signal<FormGroup | null>(null);
  isClosing = signal(false);

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.buildForm();
  }

  buildForm(): void {
    const group: any = {};

    this.config().fields.forEach(field => {
      const validators = [];

      if (field.required) {
        validators.push(Validators.required);
      }

      if (field.type === 'email') {
        validators.push(Validators.email);
      }

      if (field.min !== undefined) {
        validators.push(Validators.min(field.min));
      }

      if (field.max !== undefined) {
        validators.push(Validators.max(field.max));
      }

      const initialValue = this.initialData()?.[field.key] || '';
      group[field.key] = [initialValue, validators];
    });

    this.formGroup.set(this.fb.group(group));
  }

  onSubmit(): void {
    const form = this.formGroup();
    if (!form) return;

    if (form.valid) {
      this.isClosing.set(true);
      setTimeout(() => {
        this.submit.emit(form.value);
        this.isClosing.set(false);
        form.reset();
      }, 150);
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(form.controls).forEach(key => {
        form.get(key)?.markAsTouched();
      });
    }
  }

  onCancel(): void {
    this.isClosing.set(true);
    setTimeout(() => {
      this.cancel.emit();
      this.isClosing.set(false);
      this.formGroup()?.reset();
    }, 150);
  }

  onBackdropClick(): void {
    this.onCancel();
  }

  getFieldError(field: FormField): string {
    const form = this.formGroup();
    if (!form) return '';

    const control = form.get(field.key);
    if (!control || !control.touched || !control.errors) return '';

    if (control.errors['required']) {
      return `${field.label} is required`;
    }
    if (control.errors['email']) {
      return 'Invalid email format';
    }
    if (control.errors['min']) {
      return `Minimum value is ${field.min}`;
    }
    if (control.errors['max']) {
      return `Maximum value is ${field.max}`;
    }

    return 'Invalid value';
  }

  hasError(field: FormField): boolean {
    const form = this.formGroup();
    if (!form) return false;

    const control = form.get(field.key);
    return !!(control && control.touched && control.errors);
  }
}
