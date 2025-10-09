import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { EntityPageLayoutComponent } from '../../../../../shared/components/entity-page-layout/entity-page-layout.component';
import { DataTableComponent, TableColumn, TableAction } from '../../../../../shared/components/data-table/data-table.component';
import { DynamicFormComponent, FormField } from '../../../../../shared/components/dynamic-form/dynamic-form.component';
import { ConfirmationDialogComponent } from '../../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { ExerciseTypesService, ExerciseTypeDto } from '../../../services/exercise-types.service';
import { NotificationService } from '../../../../../shared/notifications/notification.service';

@Component({
  selector: 'app-exercise-types-page',
  standalone: true,
  imports: [CommonModule, TranslateModule, EntityPageLayoutComponent, DataTableComponent, DynamicFormComponent, ConfirmationDialogComponent],
  templateUrl: './exercise-types.page.html'
})
export class ExerciseTypesPage implements OnInit {
  private typesService = inject(ExerciseTypesService);
  private ns = inject(NotificationService);

  types = signal<ExerciseTypeDto[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  // Form state
  isFormOpen = signal(false);
  selectedType = signal<ExerciseTypeDto | null>(null);
  formTitle = 'Add Exercise Type';

  // Confirmation dialog state
  isConfirmDialogOpen = signal(false);

  columns: TableColumn[] = [
    { key: 'name', label: 'Nombre', sortable: true },
    { key: 'description', label: 'Descripción', sortable: false },
    {
      key: 'requiresSets',
      label: 'Requiere Series',
      sortable: true,
      type: 'badge'
    },
    {
      key: 'requiresReps',
      label: 'Requiere Repeticiones',
      sortable: true,
      type: 'badge'
    },
    {
      key: 'requiresDuration',
      label: 'Requiere Duración',
      sortable: true,
      type: 'badge'
    },
    { key: 'isActive', label: 'Estado', sortable: true, type: 'badge' }
  ];

  actions: TableAction[] = [
    {
      icon: 'M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z',
      label: 'Editar',
      color: 'blue',
      handler: (row) => this.openEditForm(row)
    },
    {
      icon: 'M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16',
      label: 'Eliminar',
      color: 'red',
      handler: (row) => this.openDeleteConfirm(row)
    }
  ];

  formConfig: FormField[] = [
    { key: 'name', label: 'Nombre', type: 'text', required: true },
    { key: 'description', label: 'Descripción', type: 'textarea', required: true },
    { key: 'requiresSets', label: 'Requiere Series', type: 'checkbox', required: false },
    { key: 'requiresReps', label: 'Requiere Repeticiones', type: 'checkbox', required: false },
    { key: 'requiresDuration', label: 'Requiere Duración', type: 'checkbox', required: false },
    { key: 'isActive', label: 'Activo', type: 'checkbox', required: false }
  ];

  async ngOnInit(): Promise<void> {
    await this.loadTypes();
  }

  private async loadTypes(): Promise<void> {
    try {
      this.isLoading.set(true);
      this.error.set(null);

      const data = await this.typesService.getTypes();
      this.types.set(data || []);
    } catch (err: any) {
      console.error('Failed to load exercise types:', err);
      this.error.set(err.message || 'Failed to load types');
      this.ns.error(err?.message ?? 'Error al cargar tipos', 'Error');
      this.types.set([]);
    } finally {
      this.isLoading.set(false);
    }
  }

  openAddForm(): void {
    this.selectedType.set(null);
    this.formTitle = 'Add Exercise Type';
    this.isFormOpen.set(true);
  }

  openEditForm(type: ExerciseTypeDto): void {
    this.selectedType.set(type);
    this.formTitle = `Edit ${type.name}`;
    this.isFormOpen.set(true);
  }

  async handleFormSubmit(formData: any): Promise<void> {
    const selected = this.selectedType();

    try {
      if (selected) {
        // Update existing type
        await this.typesService.updateType(selected.id, formData);
        this.ns.success('Tipo actualizado', 'Tipos de Ejercicios');
      } else {
        // Create new type
        await this.typesService.createType(formData);
        this.ns.success('Tipo creado', 'Tipos de Ejercicios');
      }

      await this.loadTypes();
      this.closeForm();
    } catch (err: any) {
      console.error('Failed to save exercise type:', err);
      this.error.set(err.message || 'Failed to save type');
      this.ns.error(err?.message ?? 'No se pudo guardar el tipo', 'Error');
    }
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedType.set(null);
  }

  openDeleteConfirm(type: ExerciseTypeDto): void {
    this.selectedType.set(type);
    this.isConfirmDialogOpen.set(true);
  }

  async handleDeleteConfirm(): Promise<void> {
    const typeToDelete = this.selectedType();
    if (!typeToDelete) return;

    try {
      await this.typesService.deleteType(typeToDelete.id);
      this.ns.success('Tipo eliminado', 'Tipos de Ejercicios');
      this.types.update(list => list.filter(t => t.id !== typeToDelete.id));
    } catch (err: any) {
      console.error('Failed to delete exercise type:', err);
      this.error.set(err.message || 'Failed to delete type');
      this.ns.error(err?.message ?? 'No se pudo eliminar el tipo', 'Error');
    } finally {
      this.closeConfirmDialog();
    }
  }

  closeConfirmDialog(): void {
    this.isConfirmDialogOpen.set(false);
    this.selectedType.set(null);
  }
}
