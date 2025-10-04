export interface FormField {
  key: string;
  label: string;
  type: 'text' | 'number' | 'email' | 'date' | 'select' | 'textarea' | 'checkbox';
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  min?: number;
  max?: number;
  options?: { value: any; label: string }[]; // For select fields
  rows?: number; // For textarea
  validators?: ValidatorConfig[];
}

export interface ValidatorConfig {
  type: 'required' | 'email' | 'min' | 'max' | 'minLength' | 'maxLength' | 'pattern';
  value?: any;
  message?: string;
}

export interface FormConfig {
  fields: FormField[];
  submitText?: string;
  cancelText?: string;
}
