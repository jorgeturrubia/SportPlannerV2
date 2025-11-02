import { Component, signal, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { DataTableComponent, TableColumn, TableAction } from '../../../../shared/components/data-table/data-table.component';
import { ExerciseFormComponent, ExerciseFormConfig, ExerciseFormData } from '../../components/exercise-form/exercise-form.component';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { ExercisesService, ExerciseDto } from '../../services/exercises.service';
import { ExerciseCategoriesService } from '../../services/exercise-categories.service';
import { ExerciseTypesService } from '../../services/exercise-types.service';
import { NotificationService } from '../../../../shared/notifications/notification.service';
import { ContentOwnership } from '../../services/objectives.service';
import { ExerciseAnimationPlayerComponent } from '../../../exercise-demo/components/exercise-animation-player/exercise-animation-player.component';

@Component({
  selector: 'app-exercises-page',
  standalone: true,
  imports: [CommonModule, TranslateModule, DataTableComponent, ExerciseFormComponent, ConfirmationDialogComponent, ExerciseAnimationPlayerComponent],
  templateUrl: './exercises.page.html'
})
export class ExercisesPage implements OnInit {
  private exercisesService = inject(ExercisesService);
  private categoriesService = inject(ExerciseCategoriesService);
  private typesService = inject(ExerciseTypesService);
  private ns = inject(NotificationService);

  exercises = signal<ExerciseDto[]>([]);
  categories = signal<any[]>([]);
  types = signal<any[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);
  isFormOpen = signal(false);
  selectedExercise = signal<ExerciseDto | null>(null);
  formTitle = 'Add Exercise';
  isConfirmDialogOpen = signal(false);
  
  // Animation viewer
  isAnimationViewerOpen = signal(false);
  selectedAnimation = signal<any>(null);

  columns = computed<TableColumn[]>(() => [
    { key: 'name', label: 'Nombre', sortable: true },
    { key: 'categoryName', label: 'Categoría', sortable: true },
    { key: 'typeName', label: 'Tipo', sortable: true },
    {
      key: 'ownership',
      label: 'Tipo',
      sortable: true,
      formatter: (value: string) => this.getOwnershipLabel(value)
    },
    { key: 'isActive', label: 'Estado', sortable: true, type: 'badge' }
  ]);

  actions: TableAction[] = [
    {
      icon: 'M14.752 11.168l-3.197-2.132A1 1 0 0010 9.87v4.263a1 1 0 001.555.832l3.197-2.132a1 1 0 000-1.664z M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
      label: 'Ver Animación',
      color: 'blue',
      handler: (row) => {
        if (row.animationJson) this.openAnimationViewer(row);
      }
    },
    {
      icon: 'M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z',
      label: 'Editar',
      color: 'blue',
      handler: (row) => {
        // Backend returns ownership as string, so compare with string
        if (row.ownership === 'User') {
          this.openEditForm(row);
        }
      }
    },
    {
      icon: 'M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z',
      label: 'Clonar',
      color: 'green',
      handler: (row) => {
        // Backend returns ownership as string, so compare with string
        if (row.ownership !== 'User') this.handleClone(row);
      }
    },
    {
      icon: 'M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16',
      label: 'Eliminar',
      color: 'red',
      handler: (row) => {
        // Backend returns ownership as string, so compare with string
        if (row.ownership === 'User') this.openDeleteConfirm(row);
      }
    }
  ];

  // Current form does not require categories/types; keep config as empty object for future extensibility
  formConfig = computed<ExerciseFormConfig>(() => ({} as ExerciseFormConfig));

  async ngOnInit(): Promise<void> {
    await Promise.all([
      this.loadExercises(),
      this.loadMasterData()
    ]);
  }

  private async loadMasterData(): Promise<void> {
    try {
      const [categories, types] = await Promise.all([
        this.categoriesService.getCategories(),
        this.typesService.getTypes()
      ]);
      this.categories.set(categories);
      this.types.set(types);
    } catch (err: any) {
      console.error('Failed to load master data:', err);
      this.ns.error(err?.message ?? 'Error al cargar datos maestros', 'Error');
    }
  }

  private async loadExercises(): Promise<void> {
    try {
      this.isLoading.set(true);
      this.error.set(null);
      const data = await this.exercisesService.getExercises();
      this.exercises.set(data || []);
    } catch (err: any) {
      console.error('Failed to load exercises:', err);
      this.error.set(err.message || 'Failed to load exercises');
      this.ns.error(err?.message ?? 'Error al cargar ejercicios', 'Error');
      this.exercises.set([]);
    } finally {
      this.isLoading.set(false);
    }
  }

  getOwnershipLabel(ownership: string): string {
    switch (ownership) {
      case 'System': return 'Sistema';
      case 'User': return 'Usuario';
      case 'MarketplaceUser': return 'Marketplace';
      default: return 'Unknown';
    }
  }

  openAddForm(): void {
    this.selectedExercise.set(null);
    this.formTitle = 'Nuevo Ejercicio';
    this.isFormOpen.set(true);
  }

  openEditForm(exercise: ExerciseDto): void {
    console.log('openEditForm called with exercise:', exercise);
    console.log('Exercise ownership:', exercise.ownership, 'ContentOwnership.User:', ContentOwnership.User);
    this.selectedExercise.set(exercise);
    this.formTitle = `Editar ${exercise.name}`;
    this.isFormOpen.set(true);
  }

  async handleFormSubmit(formData: ExerciseFormData): Promise<void> {
    const selected = this.selectedExercise();
    try {
      if (selected) {
        const payload = { ...formData, id: selected.id };
        await this.exercisesService.updateExercise(selected.id, payload as any);
        this.ns.success('Ejercicio actualizado', 'Ejercicios');
      } else {
        await this.exercisesService.createExercise(formData as any);
        this.ns.success('Ejercicio creado', 'Ejercicios');
      }
      await this.loadExercises();
      this.closeForm();
    } catch (err: any) {
      console.error('Failed to save exercise:', err);
      this.ns.error(err?.message ?? 'No se pudo guardar el ejercicio', 'Error');
    }
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedExercise.set(null);
  }

  openDeleteConfirm(exercise: ExerciseDto): void {
    this.selectedExercise.set(exercise);
    this.isConfirmDialogOpen.set(true);
  }

  async handleDeleteConfirm(): Promise<void> {
    const exerciseToDelete = this.selectedExercise();
    if (!exerciseToDelete) return;
    try {
      await this.exercisesService.deleteExercise(exerciseToDelete.id);
      this.ns.success('Ejercicio eliminado', 'Ejercicios');
      this.exercises.update(list => list.filter(e => e.id !== exerciseToDelete.id));
    } catch (err: any) {
      console.error('Failed to delete exercise:', err);
      this.ns.error(err?.message ?? 'No se pudo eliminar el ejercicio', 'Error');
    } finally {
      this.closeConfirmDialog();
    }
  }

  closeConfirmDialog(): void {
    this.isConfirmDialogOpen.set(false);
    this.selectedExercise.set(null);
  }

  async handleClone(exercise: ExerciseDto): Promise<void> {
    try {
      await this.exercisesService.cloneExercise(exercise.id);
      await this.loadExercises();
      this.ns.success('Ejercicio clonado', 'Ejercicios');
    } catch (err: any) {
      console.error('Failed to clone exercise:', err);
      this.ns.error(err?.message ?? 'No se pudo clonar el ejercicio', 'Error');
    }
  }

  openAnimationViewer(exercise: ExerciseDto): void {
    if (!exercise.animationJson) {
      this.ns.error('Este ejercicio no tiene animación disponible', 'Error');
      return;
    }

    try {
      const animation = JSON.parse(exercise.animationJson);
      this.selectedAnimation.set(animation);
      this.isAnimationViewerOpen.set(true);
    } catch (err: any) {
      console.error('Error parsing animation JSON:', err);
      this.ns.error('Error al cargar la animación', 'Error');
    }
  }

  closeAnimationViewer(): void {
    this.isAnimationViewerOpen.set(false);
    this.selectedAnimation.set(null);
  }
}
