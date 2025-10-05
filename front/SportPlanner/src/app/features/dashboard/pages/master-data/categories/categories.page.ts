import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataTableComponent, TableColumn, TableAction } from '../../../../../shared/components/data-table/data-table.component';
import { DynamicFormComponent, FormField } from '../../../../../shared/components/dynamic-form/dynamic-form.component';
import { ConfirmationDialogComponent } from '../../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { MasterDataService, TeamCategoryResponse } from '../../../services/master-data.service';

enum Sport {
  Football = 0,
  Basketball = 1,
  Handball = 2
}

@Component({
  selector: 'app-categories-page',
  standalone: true,
  imports: [CommonModule, DataTableComponent, DynamicFormComponent, ConfirmationDialogComponent],
  templateUrl: './categories.page.html'
})
export class CategoriesPage implements OnInit {
  private masterDataService = inject(MasterDataService);

  categories = signal<TeamCategoryResponse[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  // Form state
  isFormOpen = signal(false);
  selectedCategory = signal<TeamCategoryResponse | null>(null);
  formTitle = 'Add Category';

  // Confirmation dialog state
  isConfirmDialogOpen = signal(false);

  columns: TableColumn[] = [
    { key: 'name', label: 'Name', sortable: true },
    { key: 'code', label: 'Code', sortable: true },
    { key: 'description', label: 'Description', sortable: false },
    {
      key: 'sport',
      label: 'Sport',
      sortable: true,
      formatter: (value) => Sport[value] || 'Unknown'
    },
    { key: 'sortOrder', label: 'Order', sortable: true, type: 'number' },
    { key: 'isActive', label: 'Status', type: 'badge', sortable: true }
  ];

  actions: TableAction[] = [
    {
      icon: 'M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z',
      label: 'Edit',
      color: 'blue',
      handler: (row) => this.openEditForm(row)
    },
    {
      icon: 'M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16',
      label: 'Delete',
      color: 'red',
      handler: (row) => this.openDeleteConfirm(row)
    }
  ];

  formConfig: FormField[] = [
    { key: 'name', label: 'Name', type: 'text', required: true },
    { key: 'code', label: 'Code', type: 'text', required: true },
    { key: 'description', label: 'Description', type: 'textarea', required: false },
    {
      key: 'sport',
      label: 'Sport',
      type: 'select',
      required: true,
      options: [
        { value: Sport.Football, label: 'Football' },
        { value: Sport.Basketball, label: 'Basketball' },
        { value: Sport.Handball, label: 'Handball' }
      ]
    },
    { key: 'sortOrder', label: 'Sort Order', type: 'number', required: true },
    { key: 'isActive', label: 'Active', type: 'checkbox', required: false }
  ];

  async ngOnInit(): Promise<void> {
    await this.loadCategories();
  }

  private async loadCategories(): Promise<void> {
    try {
      this.isLoading.set(true);
      this.error.set(null);

      const data = await this.masterDataService.getTeamCategories();
      this.categories.set(data || []);
    } catch (err: any) {
      console.error('Failed to load categories:', err);
      this.error.set(err.message || 'Failed to load categories');
      this.categories.set([]);
    } finally {
      this.isLoading.set(false);
    }
  }

  openAddForm(): void {
    this.selectedCategory.set(null);
    this.formTitle = 'Add Category';
    this.isFormOpen.set(true);
  }

  openEditForm(category: TeamCategoryResponse): void {
    this.selectedCategory.set(category);
    this.formTitle = `Edit ${category.name}`;
    this.isFormOpen.set(true);
  }

  async handleFormSubmit(formData: any): Promise<void> {
    const selected = this.selectedCategory();

    try {
      if (selected) {
        // Update existing category
        const updated = await this.masterDataService.updateTeamCategory(selected.id, formData);
        if (updated) {
          this.categories.update(list =>
            list.map(c => c.id === selected.id ? updated : c)
          );
        }
      } else {
        // Create new category
        const created = await this.masterDataService.createTeamCategory(formData);
        if (created) {
          this.categories.update(list => [...list, created]);
        }
      }

      this.closeForm();
    } catch (err: any) {
      console.error('Failed to save category:', err);
      this.error.set(err.message || 'Failed to save category');
    }
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedCategory.set(null);
  }

  openDeleteConfirm(category: TeamCategoryResponse): void {
    this.selectedCategory.set(category);
    this.isConfirmDialogOpen.set(true);
  }

  async handleDeleteConfirm(): Promise<void> {
    const categoryToDelete = this.selectedCategory();
    if (!categoryToDelete) return;

    try {
      const success = await this.masterDataService.deleteTeamCategory(categoryToDelete.id);

      if (success) {
        this.categories.update(list => list.filter(c => c.id !== categoryToDelete.id));
      } else {
        this.error.set('Failed to delete category');
      }
    } catch (err: any) {
      console.error('Failed to delete category:', err);
      this.error.set(err.message || 'Failed to delete category');
    } finally {
      this.closeConfirmDialog();
    }
  }

  closeConfirmDialog(): void {
    this.isConfirmDialogOpen.set(false);
    this.selectedCategory.set(null);
  }
}
