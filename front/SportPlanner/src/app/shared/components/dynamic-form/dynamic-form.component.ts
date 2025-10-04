import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

export interface FormField {
  key: string;
  label: string;
  type: 'text' | 'number' | 'textarea' | 'select';
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
export class DynamicFormComponent implements OnChanges {
  @Input() isOpen = false;
  @Input() config: FormField[] = [];
  @Input() initialData: any = null;
  @Input() title = 'Form';

  @Output() formSubmit = new EventEmitter<any>();
  @Output() cancel = new EventEmitter<void>();

  form: FormGroup;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({});
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['config'] || changes['initialData']) {
      this.buildForm();
    }
  }

  private buildForm(): void {
    this.form = this.fb.group({});
    if (!this.config) return;

    for (const field of this.config) {
      const validators = field.required ? [Validators.required] : [];
      const value = this.initialData ? this.initialData[field.key] : '';
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
