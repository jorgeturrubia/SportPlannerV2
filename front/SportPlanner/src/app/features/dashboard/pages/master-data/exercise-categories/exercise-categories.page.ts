import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { EntityPageLayoutComponent } from '../../../../../shared/components/entity-page-layout/entity-page-layout.component';
import { DataTableComponent, TableColumn, TableAction } from '../../../../../shared/components/data-table/data-table.component';
import { DynamicFormComponent, FormField } from '../../../../../shared/components/dynamic-form/dynamic-form.component';
import { ConfirmationDialogComponent } from '../../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { ExerciseCategoriesService, ExerciseCategoryDto } from '../../../services/exercise-categories.service';
import { Sport } from '../../../services/objectives.service';
import { NotificationService } from '../../../../../shared/notifications/notification.service';

@Component({
  selector: 'app-exercise-categories-page',
  standalone: true,
  imports: [CommonModule, TranslateModule, EntityPageLayoutComponent, DataTableComponent, DynamicFormComponent, ConfirmationDialogComponent],
  templateUrl: './exercise-categories.page.html'
})
export class ExerciseCategoriesPage implements OnInit {
  private categoriesService = inject(ExerciseCategoriesService);
  private ns = inject(NotificationService);

  categories = signal<ExerciseCategoryDto[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  // Form state
  isFormOpen = signal(false);
  selectedCategory = signal<ExerciseCategoryDto | null>(null);
  formTitle = 'Add Exercise Category';

  // Confirmation dialog state
  isConfirmDialogOpen = signal(false);

  columns: TableColumn[] = [
    { key: 'name', label: 'Nombre', sortable: true },
    { key: 'description', label: 'Descripción', sortable: false },
    {
      key: 'sport',
      label: 'Deporte',
      sortable: true,
      formatter: (value) => this.getSportName(value)
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
    {
      key: 'sport',
      label: 'Deporte',
      type: 'select',
      required: true,
      options: [
        { value: Sport.Football, label: 'Fútbol' },
        { value: Sport.Basketball, label: 'Baloncesto' },
        { value: Sport.Handball, label: 'Balonmano' }
      ]
    },
    { key: 'isActive', label: 'Activo', type: 'checkbox', required: false }
  ];

  async ngOnInit(): Promise<void> {
    await this.loadCategories();
  }

  private async loadCategories(): Promise<void> {
    try {
      this.isLoading.set(true);
      this.error.set(null);

      const data = await this.categoriesService.getCategories();
      this.categories.set(data || []);
    } catch (err: any) {
      console.error('Failed to load exercise categories:', err);
      this.error.set(err.message || 'Failed to load categories');
      this.ns.error(err?.message ?? 'Error al cargar categorías', 'Error');
      this.categories.set([]);
    } finally {
      this.isLoading.set(false);
    }
  }

  getSportName(sport: Sport): string {
    switch (sport) {
      case Sport.Football: return 'Fútbol';
      case Sport.Basketball: return 'Baloncesto';
      case Sport.Handball: return 'Balonmano';
      default: return 'Desconocido';
    }
  }

  openAddForm(): void {
    this.selectedCategory.set(null);
    this.formTitle = 'Add Exercise Category';
    this.isFormOpen.set(true);
  }

  openEditForm(category: ExerciseCategoryDto): void {
    this.selectedCategory.set(category);
    this.formTitle = `Edit ${category.name}`;
    this.isFormOpen.set(true);
  }

  async handleFormSubmit(formData: any): Promise<void> {
    const selected = this.selectedCategory();

    try {
      if (selected) {
        // Update existing category
        await this.categoriesService.updateCategory(selected.id, formData);
        this.ns.success('Categoría actualizada', 'Categorías de Ejercicios');
      } else {
        // Create new category
        await this.categoriesService.createCategory(formData);
        this.ns.success('Categoría creada', 'Categorías de Ejercicios');
      }

      await this.loadCategories();
      this.closeForm();
    } catch (err: any) {
      console.error('Failed to save exercise category:', err);
      this.error.set(err.message || 'Failed to save category');
      this.ns.error(err?.message ?? 'No se pudo guardar la categoría', 'Error');
    }
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedCategory.set(null);
  }

  openDeleteConfirm(category: ExerciseCategoryDto): void {
    this.selectedCategory.set(category);
    this.isConfirmDialogOpen.set(true);
  }

  async handleDeleteConfirm(): Promise<void> {
    const categoryToDelete = this.selectedCategory();
    if (!categoryToDelete) return;

    try {
      await this.categoriesService.deleteCategory(categoryToDelete.id);
      this.ns.success('Categoría eliminada', 'Categorías de Ejercicios');
      this.categories.update(list => list.filter(c => c.id !== categoryToDelete.id));
    } catch (err: any) {
      console.error('Failed to delete exercise category:', err);
      this.error.set(err.message || 'Failed to delete category');
      this.ns.error(err?.message ?? 'No se pudo eliminar la categoría', 'Error');
    } finally {
      this.closeConfirmDialog();
    }
  }

  closeConfirmDialog(): void {
    this.isConfirmDialogOpen.set(false);
    this.selectedCategory.set(null);
  }
}
