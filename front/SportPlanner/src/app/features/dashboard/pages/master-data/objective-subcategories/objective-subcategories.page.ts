import { Component, signal, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { EntityPageLayoutComponent } from '../../../../../shared/components/entity-page-layout/entity-page-layout.component';
import { DataTableComponent, TableColumn, TableAction } from '../../../../../shared/components/data-table/data-table.component';
import { DynamicFormComponent, FormField } from '../../../../../shared/components/dynamic-form/dynamic-form.component';
import { ConfirmationDialogComponent } from '../../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { ObjectiveSubcategoriesService, ObjectiveSubcategoryDto } from '../../../services/objective-subcategories.service';
import { ObjectiveCategoriesService, ObjectiveCategoryDto } from '../../../services/objective-categories.service';
import { NotificationService } from '../../../../../shared/notifications/notification.service';

@Component({
  selector: 'app-objective-subcategories-page',
  standalone: true,
  imports: [CommonModule, TranslateModule, EntityPageLayoutComponent, DataTableComponent, DynamicFormComponent, ConfirmationDialogComponent],
  templateUrl: './objective-subcategories.page.html'
})
export class ObjectiveSubcategoriesPage implements OnInit {
  private subcategoriesService = inject(ObjectiveSubcategoriesService);
  private categoriesService = inject(ObjectiveCategoriesService);
  private ns = inject(NotificationService);

  subcategories = signal<ObjectiveSubcategoryDto[]>([]);
  categories = signal<ObjectiveCategoryDto[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  // Form state
  isFormOpen = signal(false);
  selectedSubcategory = signal<ObjectiveSubcategoryDto | null>(null);
  formTitle = 'Add Objective Subcategory';

  // Confirmation dialog state
  isConfirmDialogOpen = signal(false);

  columns = computed<TableColumn[]>(() => [
    { key: 'name', label: 'Nombre', sortable: true },
    {
      key: 'objectiveCategoryId',
      label: 'Categoría',
      sortable: true,
      formatter: (value: string) => this.getCategoryName(value)
    }
  ]);

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

  formConfig = computed<FormField[]>(() => [
    { key: 'name', label: 'Nombre', type: 'text', required: true },
    {
      key: 'objectiveCategoryId',
      label: 'Categoría',
      type: 'select',
      required: true,
      options: this.categories().map(c => ({ value: c.id, label: c.name }))
    }
  ]);

  async ngOnInit(): Promise<void> {
    await Promise.all([
      this.loadSubcategories(),
      this.loadCategories()
    ]);
  }

  private async loadSubcategories(): Promise<void> {
    try {
      this.isLoading.set(true);
      this.error.set(null);

      const data = await this.subcategoriesService.getSubcategories();
      this.subcategories.set(data || []);
    } catch (err: any) {
      console.error('Failed to load objective subcategories:', err);
      this.error.set(err.message || 'Failed to load subcategories');
      this.ns.error(err?.message ?? 'Error al cargar subcategorías', 'Error');
      this.subcategories.set([]);
    } finally {
      this.isLoading.set(false);
    }
  }

  private async loadCategories(): Promise<void> {
    try {
      const data = await this.categoriesService.getCategories();
      this.categories.set(data || []);
    } catch (err: any) {
      console.error('Failed to load objective categories:', err);
      this.ns.error(err?.message ?? 'Error al cargar categorías', 'Error');
    }
  }

  getCategoryName(categoryId: string): string {
    const category = this.categories().find(c => c.id === categoryId);
    return category?.name || 'Desconocido';
  }

  openAddForm(): void {
    this.selectedSubcategory.set(null);
    this.formTitle = 'Add Objective Subcategory';
    this.isFormOpen.set(true);
  }

  openEditForm(subcategory: ObjectiveSubcategoryDto): void {
    this.selectedSubcategory.set(subcategory);
    this.formTitle = `Edit ${subcategory.name}`;
    this.isFormOpen.set(true);
  }

  async handleFormSubmit(formData: any): Promise<void> {
    const selected = this.selectedSubcategory();

    try {
      if (selected) {
        // Update existing subcategory
        await this.subcategoriesService.updateSubcategory(selected.id, formData);
        this.ns.success('Subcategoría actualizada', 'Subcategorías de Objetivos');
      } else {
        // Create new subcategory
        await this.subcategoriesService.createSubcategory(formData);
        this.ns.success('Subcategoría creada', 'Subcategorías de Objetivos');
      }

      await this.loadSubcategories();
      this.closeForm();
    } catch (err: any) {
      console.error('Failed to save objective subcategory:', err);
      this.error.set(err.message || 'Failed to save subcategory');
      this.ns.error(err?.message ?? 'No se pudo guardar la subcategoría', 'Error');
    }
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedSubcategory.set(null);
  }

  openDeleteConfirm(subcategory: ObjectiveSubcategoryDto): void {
    this.selectedSubcategory.set(subcategory);
    this.isConfirmDialogOpen.set(true);
  }

  async handleDeleteConfirm(): Promise<void> {
    const subcategoryToDelete = this.selectedSubcategory();
    if (!subcategoryToDelete) return;

    try {
      await this.subcategoriesService.deleteSubcategory(subcategoryToDelete.id);
      this.ns.success('Subcategoría eliminada', 'Subcategorías de Objetivos');
      this.subcategories.update(list => list.filter(s => s.id !== subcategoryToDelete.id));
    } catch (err: any) {
      console.error('Failed to delete objective subcategory:', err);
      this.error.set(err.message || 'Failed to delete subcategory');
      this.ns.error(err?.message ?? 'No se pudo eliminar la subcategoría', 'Error');
    } finally {
      this.closeConfirmDialog();
    }
  }

  closeConfirmDialog(): void {
    this.isConfirmDialogOpen.set(false);
    this.selectedSubcategory.set(null);
  }
}
