import { Component, input, output, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

export interface FormField {
  key: string;
  label: string;
  type: 'text' | 'number' | 'textarea' | 'select' | 'checkbox' | 'date';
  required?: boolean;
  options?: { label: string; value: any }[];
  colspan?: number; // Number of columns to span (default: 1)
  disabled?: boolean; // Whether the field is disabled
  onChange?: (value: any) => void; // Callback when field value changes
}

export interface FormLayout {
  columns: number; // Total columns in the grid (e.g., 2, 3, 4)
  fields: FormField[]; // Array of fields in order
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
  layout = input<FormLayout | null>(null); // Optional layout config
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

    // Reset form when dialog opens with null data
    effect(() => {
      const isDialogOpen = this.isOpen();
      const currentData = this.initialData();

      if (isDialogOpen && !currentData) {
        // Force reset the form when opening with null data
        this.form.reset();
      }
    });
  }

  private buildForm(config: FormField[], initialData: any): void {
    this.form = this.fb.group({});
    if (!config || config.length === 0) return;

    for (const field of config) {
      const validators = field.required ? [Validators.required] : [];
      // Handle checkbox default value
      let value = initialData?.[field.key];
      if (value === undefined || value === null) {
        value = field.type === 'checkbox' ? false : '';
      }
      
      const control = this.fb.control({ value, disabled: field.disabled }, validators);
      this.form.addControl(field.key, control);
      
      // Add change listener if onChange is provided
      if (field.onChange) {
        this.form.get(field.key)?.valueChanges.subscribe(field.onChange);
      }
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

  getGridClass(): string {
    const l = this.layout();
    if (!l) {
      return 'grid-cols-2';
    }
    return `grid-cols-${l.columns}`;
  }

  getColSpanClass(colspan: number): string {
    const l = this.layout();
    const totalCols = l?.columns || 2;

    if (colspan === totalCols) {
      return 'col-span-full';
    }

    return `col-span-${colspan}`;
  }
}
