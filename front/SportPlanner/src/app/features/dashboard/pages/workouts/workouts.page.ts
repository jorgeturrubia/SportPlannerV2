import { Component, signal, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { DataTableComponent, TableColumn, TableAction } from '../../../../shared/components/data-table/data-table.component';
import { DynamicFormComponent, FormField } from '../../../../shared/components/dynamic-form/dynamic-form.component';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { WorkoutsService, WorkoutDto } from '../../services/workouts.service';
import { ObjectivesService } from '../../services/objectives.service';
import { NotificationService } from '../../../../shared/notifications/notification.service';
import { ContentOwnership } from '../../services/objectives.service';

@Component({
  selector: 'app-workouts-page',
  standalone: true,
  imports: [CommonModule, TranslateModule, DataTableComponent, DynamicFormComponent, ConfirmationDialogComponent],
  templateUrl: './workouts.page.html'
})
export class WorkoutsPage implements OnInit {
  private workoutsService = inject(WorkoutsService);
  private objectivesService = inject(ObjectivesService);
  private ns = inject(NotificationService);

  workouts = signal<WorkoutDto[]>([]);
  objectives = signal<any[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);
  isFormOpen = signal(false);
  selectedWorkout = signal<WorkoutDto | null>(null);
  formTitle = 'Add Workout';
  isConfirmDialogOpen = signal(false);

  columns = computed<TableColumn[]>(() => [
    { key: 'fecha', label: 'Fecha', sortable: true, type: 'date' },
    {
      key: 'estimatedDurationMinutes',
      label: 'Duración (min)',
      sortable: true,
      type: 'number'
    },
    {
      key: 'ownership',
      label: 'Tipo',
      sortable: true,
      formatter: (value: ContentOwnership) => this.getOwnershipLabel(value)
    },
    { key: 'isActive', label: 'Estado', sortable: true, type: 'badge' }
  ]);

  actions: TableAction[] = [
    {
      icon: 'M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z',
      label: 'Editar',
      color: 'blue',
      handler: (row) => {
        if (row.ownership === ContentOwnership.User) this.openEditForm(row);
      }
    },
    {
      icon: 'M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z',
      label: 'Clonar',
      color: 'green',
      handler: (row) => {
        if (row.ownership !== ContentOwnership.User) this.handleClone(row);
      }
    },
    {
      icon: 'M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16',
      label: 'Eliminar',
      color: 'red',
      handler: (row) => {
        if (row.ownership === ContentOwnership.User) this.openDeleteConfirm(row);
      }
    }
  ];

  formConfig = computed<FormField[]>(() => [
    { key: 'fecha', label: 'Fecha', type: 'date', required: true },
    { key: 'estimatedDurationMinutes', label: 'Duración Estimada (min)', type: 'number', required: false },
    { key: 'notes', label: 'Notas', type: 'textarea', required: false, colspan: 2 }
  ]);

  async ngOnInit(): Promise<void> {
    await Promise.all([
      this.loadWorkouts(),
      this.loadObjectives()
    ]);
  }

  private async loadObjectives(): Promise<void> {
    try {
      const data = await this.objectivesService.getObjectives();
      this.objectives.set(data || []);
    } catch (err: any) {
      console.error('Failed to load objectives:', err);
      this.ns.error(err?.message ?? 'Error al cargar objetivos', 'Error');
    }
  }

  private async loadWorkouts(): Promise<void> {
    try {
      this.isLoading.set(true);
      this.error.set(null);
      const data = await this.workoutsService.getWorkouts();
      this.workouts.set(data || []);
    } catch (err: any) {
      console.error('Failed to load workouts:', err);
      this.error.set(err.message || 'Failed to load workouts');
      this.ns.error(err?.message ?? 'Error al cargar sesiones', 'Error');
      this.workouts.set([]);
    } finally {
      this.isLoading.set(false);
    }
  }

  getOwnershipLabel(ownership: ContentOwnership): string {
    switch (ownership) {
      case ContentOwnership.System: return 'Sistema';
      case ContentOwnership.User: return 'Usuario';
      case ContentOwnership.MarketplaceUser: return 'Marketplace';
      default: return 'Unknown';
    }
  }

  openAddForm(): void {
    this.selectedWorkout.set(null);
    this.formTitle = 'Add Workout';
    this.isFormOpen.set(true);
  }

  openEditForm(workout: WorkoutDto): void {
    this.selectedWorkout.set(workout);
    const fecha = new Date(workout.fecha).toLocaleDateString();
    this.formTitle = `Editar Sesión del ${fecha}`;
    this.isFormOpen.set(true);
  }

  async handleFormSubmit(formData: any): Promise<void> {
    const selected = this.selectedWorkout();
    try {
      const dto = {
        ...formData,
        exercises: selected?.exercises || []
      };

      if (selected) {
        await this.workoutsService.updateWorkout(selected.id, { ...dto, id: selected.id });
        this.ns.success('Sesión actualizada', 'Sesiones');
      } else {
        await this.workoutsService.createWorkout(dto);
        this.ns.success('Sesión creada', 'Sesiones');
      }
      await this.loadWorkouts();
      this.closeForm();
    } catch (err: any) {
      console.error('Failed to save workout:', err);
      this.ns.error(err?.message ?? 'No se pudo guardar la sesión', 'Error');
    }
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedWorkout.set(null);
  }

  openDeleteConfirm(workout: WorkoutDto): void {
    this.selectedWorkout.set(workout);
    this.isConfirmDialogOpen.set(true);
  }

  async handleDeleteConfirm(): Promise<void> {
    const workoutToDelete = this.selectedWorkout();
    if (!workoutToDelete) return;
    try {
      await this.workoutsService.deleteWorkout(workoutToDelete.id);
      this.ns.success('Sesión eliminada', 'Sesiones');
      this.workouts.update(list => list.filter(w => w.id !== workoutToDelete.id));
    } catch (err: any) {
      console.error('Failed to delete workout:', err);
      this.ns.error(err?.message ?? 'No se pudo eliminar la sesión', 'Error');
    } finally {
      this.closeConfirmDialog();
    }
  }

  closeConfirmDialog(): void {
    this.isConfirmDialogOpen.set(false);
    this.selectedWorkout.set(null);
  }

  async handleClone(workout: WorkoutDto): Promise<void> {
    try {
      await this.workoutsService.cloneWorkout(workout.id);
      await this.loadWorkouts();
      this.ns.success('Sesión clonada', 'Sesiones');
    } catch (err: any) {
      console.error('Failed to clone workout:', err);
      this.ns.error(err?.message ?? 'No se pudo clonar la sesión', 'Error');
    }
  }
}
