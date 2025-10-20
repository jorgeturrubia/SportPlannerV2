import { Component, Input, Output, EventEmitter, signal, inject, OnInit, computed, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ObjectiveSubcategoriesService } from '../../../../../features/dashboard/services/objective-subcategories.service';
import { ObjectiveLevel } from '../../../../../features/dashboard/services/objectives.service';

@Component({
  selector: 'app-create-objective-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-objective-form.component.html',
  styleUrls: ['./create-objective-form.component.css']
})
export class CreateObjectiveFormComponent implements OnInit, OnChanges {
  @Input() categories: any[] = [];
  @Input() isSubmitting = false;
  @Output() formSubmit = new EventEmitter<any>();
  @Output() cancel = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private subcategoriesService = inject(ObjectiveSubcategoriesService);

  form: FormGroup;
  subcategories = signal<any[]>([]);
  selectedCategoryId = signal<string | null>(null);

  objectiveLevels = [
    { value: String(ObjectiveLevel.Beginner), label: 'Principiante' },
    { value: String(ObjectiveLevel.Intermediate), label: 'Intermedio' },
    { value: String(ObjectiveLevel.Advanced), label: 'Avanzado' }
  ];

  constructor() {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      objectiveCategoryId: ['', Validators.required],
      objectiveSubcategoryId: [{value: '', disabled: true}], // Deshabilitado desde el inicio
      level: [String(ObjectiveLevel.Intermediate)]
    });

    console.log('🔍 CreateObjectiveFormComponent - Constructor:');
    console.log('  - Form created:', this.form.value);
    console.log('  - Subcategory control disabled:', this.form.get('objectiveSubcategoryId')?.disabled);
    console.log('  - Subcategory control status:', this.form.get('objectiveSubcategoryId')?.status);
  }

  ngOnChanges(changes: SimpleChanges): void {
    console.log('🔍 CreateObjectiveFormComponent - ngOnChanges:', changes);
    if (changes['categories']) {
      console.log('  - Categories changed:', changes['categories'].currentValue?.length || 0, 'items');
    }
    if (changes['isSubmitting']) {
      console.log('  - isSubmitting changed:', changes['isSubmitting'].currentValue);
    }
  }

  ngOnInit(): void {
    console.log('🔍 CreateObjectiveFormComponent - ngOnInit:');
    console.log('  - Before disable - Subcategory disabled:', this.form.get('objectiveSubcategoryId')?.disabled);

    // Asegurar que la subcategoría esté deshabilitada al inicializar
    this.form.get('objectiveSubcategoryId')?.disable({ emitEvent: false });

    console.log('  - After disable - Subcategory disabled:', this.form.get('objectiveSubcategoryId')?.disabled);
    console.log('  - selectedCategoryId signal:', this.selectedCategoryId());

    // Subscribe to category changes to load subcategories
    this.form.get('objectiveCategoryId')?.valueChanges.subscribe(categoryId => {
      console.log('🔍 CreateObjectiveFormComponent - Category valueChanges:', categoryId);
      this.onCategoryChange(categoryId);
    });

    console.log('🔍 CreateObjectiveFormComponent - ngOnInit completed');
  }

  async onCategoryChange(categoryId: string): Promise<void> {
    console.log('🔍 CreateObjectiveFormComponent - onCategoryChange called with:', categoryId);
    console.log('  - categoryId type:', typeof categoryId, 'length:', categoryId?.length);
    console.log('  - categoryId trimmed:', categoryId?.trim());
    console.log('  - selectedCategoryId before:', this.selectedCategoryId());

    this.selectedCategoryId.set(categoryId || null);

    console.log('  - selectedCategoryId after:', this.selectedCategoryId());

    const subcategoryControl = this.form.get('objectiveSubcategoryId');
    console.log('  - subcategory control before change:', {
      value: subcategoryControl?.value,
      disabled: subcategoryControl?.disabled,
      status: subcategoryControl?.status
    });

    // Reset subcategory when changing category
    subcategoryControl?.setValue('', { emitEvent: false });

    if (categoryId && categoryId.trim() !== '') {
      console.log('  ✅ Enabling subcategory control');
      // HABILITAR el control de subcategoría
      subcategoryControl?.enable({ emitEvent: false });

      try {
        console.log('  📡 Loading subcategories for category:', categoryId);
        const subs = await this.subcategoriesService.getSubcategories(categoryId);
        console.log('  📡 Loaded subcategories:', subs?.length || 0, 'items');
        // Deduplicate by id
        const uniqueSubs = this.deduplicateById(subs || []);
        this.subcategories.set(uniqueSubs);
        console.log('  ✅ Set subcategories:', uniqueSubs.length, 'unique items');
      } catch (err) {
        console.error('  ❌ Failed to load subcategories:', err);
        this.subcategories.set([]);
      }
    } else {
      console.log('  🚫 Disabling subcategory control');
      // DESHABILITAR el control de subcategoría
      subcategoryControl?.disable({ emitEvent: false });
      this.subcategories.set([]);
    }

    console.log('  - subcategory control after change:', {
      value: subcategoryControl?.value,
      disabled: subcategoryControl?.disabled,
      status: subcategoryControl?.status
    });
    console.log('  - subcategories signal:', this.subcategories().length, 'items');
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.formSubmit.emit(this.form.value);
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.form.controls).forEach(key => {
        this.form.get(key)?.markAsTouched();
      });
    }
  }

  onCancel(): void {
    this.cancel.emit();
  }

  hasError(fieldName: string, errorType: string): boolean {
    const field = this.form.get(fieldName);
    return !!(field && field.hasError(errorType) && field.touched);
  }

  private deduplicateById<T extends { id: string }>(items: T[]): T[] {
    const seen = new Set<string>();
    const result: T[] = [];
    
    for (const item of items) {
      if (!seen.has(item.id)) {
        seen.add(item.id);
        result.push(item);
      }
    }
    
    return result;
  }

  isFormValid(): boolean {
    return this.form.valid;
  }
}
