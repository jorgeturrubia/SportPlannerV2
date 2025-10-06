import { Component, input, output, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

export interface FormField {
  key: string;
  label: string;
  type: 'text' | 'number' | 'textarea' | 'select' | 'checkbox';
  required?: boolean;
  options?: { label: string; value: any }[];
}

@Component({
  selector: 'app-dynamic-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './dynamic-form.component.html',
  styleUrls: ['./dynamic-form.component.css']
})
export class DynamicFormComponent {
  private fb = inject(FormBuilder);

  // Input signals
  isOpen = input<boolean>(false);
  config = input<FormField[]>([]);
  initialData = input<any>(null);
  title = input<string>('Form');

  // Output signals
  formSubmit = output<any>();
  cancel = output<void>();

  form: FormGroup;

  constructor() {
    this.form = this.fb.group({});

    // React to config or initialData changes
    effect(() => {
      const currentConfig = this.config();
      const currentData = this.initialData();
      this.buildForm(currentConfig, currentData);
    });
  }

  private buildForm(config: FormField[], initialData: any): void {
    this.form = this.fb.group({});
    if (!config || config.length === 0) return;

    for (const field of config) {
      const validators = field.required ? [Validators.required] : [];
      const value = initialData ? initialData[field.key] : '';
      this.form.addControl(field.key, this.fb.control(value, validators));
    }
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.formSubmit.emit(this.form.value);
    }
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
