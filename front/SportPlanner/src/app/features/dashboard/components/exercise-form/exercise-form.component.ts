import { Component, input, output, effect, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ObjectiveSelectorComponent } from '../objective-selector/objective-selector';
import { ObjectivesService, ObjectiveDto } from '../../services/objectives.service';

export interface ExerciseFormData {
  name: string;
  description: string;
  categoryId: string;
  typeId: string;
  instructions?: string;
  defaultSets?: number;
  defaultReps?: number;
  defaultDurationSeconds?: number;
  videoUrl?: string;
  imageUrl?: string;
  objectiveIds: string[];
}

export interface ExerciseFormConfig {
  categories: { id: string; name: string }[];
  types: { id: string; name: string }[];
}

@Component({
  selector: 'app-exercise-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ObjectiveSelectorComponent],
  templateUrl: './exercise-form.component.html',
  styleUrls: ['./exercise-form.component.css']
})
export class ExerciseFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private objectivesService = inject(ObjectivesService);

  // Input signals
  isOpen = input<boolean>(false);
  config = input<ExerciseFormConfig>({ categories: [], types: [] });
  initialData = input<any>(null);
  title = input<string>('Nuevo Ejercicio');

  // Output signals
  formSubmit = output<ExerciseFormData>();
  cancel = output<void>();

  // Form state
  form: FormGroup;
  selectedObjectiveIds = signal<string[]>([]);
  showObjectiveSelector = signal(false);
  allObjectives = signal<ObjectiveDto[]>([]);

  // Computed: map objective IDs to objects for the selector
  initialObjectivesForSelector = computed(() => 
    this.selectedObjectiveIds().map(id => ({ objectiveId: id }))
  );

  // Computed: get objective names
  selectedObjectivesWithNames = computed(() => {
    const ids = this.selectedObjectiveIds();
    const objectives = this.allObjectives();
    return ids.map(id => {
      const obj = objectives.find(o => o.id === id);
      return {
        id,
        name: obj?.name || id
      };
    });
  });

  async ngOnInit(): Promise<void> {
    try {
      const objectives = await this.objectivesService.getObjectives();
      this.allObjectives.set(objectives);
    } catch (err) {
      console.error('Failed to load objectives:', err);
    }
  }

  constructor() {
    this.form = this.fb.group({
      name: ['', [Validators.required]],
      description: ['', [Validators.required]],
      categoryId: ['', [Validators.required]],
      typeId: ['', [Validators.required]],
      instructions: [''],
      defaultSets: [''],
      defaultReps: [''],
      defaultDurationSeconds: [''],
      videoUrl: [''],
      imageUrl: ['']
    });

    // React to initialData changes
    effect(() => {
      const currentData = this.initialData();
      if (currentData) {
        this.form.patchValue(currentData);
        // Load existing objectives if editing
        if (currentData.objectiveIds && Array.isArray(currentData.objectiveIds)) {
          this.selectedObjectiveIds.set(currentData.objectiveIds);
        }
      } else {
        this.form.reset();
        this.selectedObjectiveIds.set([]);
      }
    });

    // Reset when dialog opens with null data
    effect(() => {
      const isDialogOpen = this.isOpen();
      const currentData = this.initialData();

      if (isDialogOpen && !currentData) {
        this.form.reset();
        this.selectedObjectiveIds.set([]);
        this.showObjectiveSelector.set(false);
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const formData: ExerciseFormData = {
      ...this.form.value,
      objectiveIds: this.selectedObjectiveIds()
    };

    this.formSubmit.emit(formData);
  }

  onCancel(): void {
    this.cancel.emit();
  }

  openObjectiveSelector(): void {
    this.showObjectiveSelector.set(true);
  }

  closeObjectiveSelector(): void {
    this.showObjectiveSelector.set(false);
  }

  onObjectivesChanged(objectiveIds: string[]): void {
    this.selectedObjectiveIds.set(objectiveIds);
    this.closeObjectiveSelector();
  }

  removeObjective(objectiveId: string): void {
    this.selectedObjectiveIds.update(ids => ids.filter(id => id !== objectiveId));
  }

  getFieldError(fieldName: string): string | null {
    const control = this.form.get(fieldName);
    if (!control || !control.touched || !control.invalid) {
      return null;
    }

    if (control.hasError('required')) {
      return 'Este campo es obligatorio';
    }

    return null;
  }
}
