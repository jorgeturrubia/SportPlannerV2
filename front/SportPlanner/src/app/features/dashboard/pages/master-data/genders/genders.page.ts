import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataTableComponent, TableColumn, TableAction } from '../../../../../shared/components/data-table/data-table.component';
import { DynamicFormComponent, FormField } from '../../../../../shared/components/dynamic-form/dynamic-form.component';
import { ConfirmationDialogComponent } from '../../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { MasterDataService, GenderResponse } from '../../../services/master-data.service';

@Component({
  selector: 'app-genders-page',
  standalone: true,
  imports: [CommonModule, DataTableComponent, DynamicFormComponent, ConfirmationDialogComponent],
  templateUrl: './genders.page.html'
})
export class GendersPage implements OnInit {
  private masterDataService = inject(MasterDataService);

  genders = signal<GenderResponse[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  // Form state
  isFormOpen = signal(false);
  selectedGender = signal<GenderResponse | null>(null);
  formTitle = 'Add Gender';

  // Confirmation dialog state
  isConfirmDialogOpen = signal(false);

  columns: TableColumn[] = [
    { key: 'name', label: 'Name', sortable: true },
    { key: 'code', label: 'Code', sortable: true },
    { key: 'description', label: 'Description', sortable: false },
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
    { key: 'isActive', label: 'Active', type: 'checkbox', required: false }
  ];

  async ngOnInit(): Promise<void> {
    await this.loadGenders();
  }

  private async loadGenders(): Promise<void> {
    try {
      this.isLoading.set(true);
      this.error.set(null);

      const data = await this.masterDataService.getGenders();
      this.genders.set(data || []);
    } catch (err: any) {
      console.error('Failed to load genders:', err);
      this.error.set(err.message || 'Failed to load genders');
      this.genders.set([]);
    } finally {
      this.isLoading.set(false);
    }
  }

  openAddForm(): void {
    this.selectedGender.set(null);
    this.formTitle = 'Add Gender';
    this.isFormOpen.set(true);
  }

  openEditForm(gender: GenderResponse): void {
    this.selectedGender.set(gender);
    this.formTitle = `Edit ${gender.name}`;
    this.isFormOpen.set(true);
  }

  async handleFormSubmit(formData: any): Promise<void> {
    const selected = this.selectedGender();

    try {
      if (selected) {
        // Update existing gender
        const updated = await this.masterDataService.updateGender(selected.id, formData);
        if (updated) {
          this.genders.update(list =>
            list.map(g => g.id === selected.id ? updated : g)
          );
        }
      } else {
        // Create new gender
        const created = await this.masterDataService.createGender(formData);
        if (created) {
          this.genders.update(list => [...list, created]);
        }
      }

      this.closeForm();
    } catch (err: any) {
      console.error('Failed to save gender:', err);
      this.error.set(err.message || 'Failed to save gender');
    }
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedGender.set(null);
  }

  openDeleteConfirm(gender: GenderResponse): void {
    this.selectedGender.set(gender);
    this.isConfirmDialogOpen.set(true);
  }

  async handleDeleteConfirm(): Promise<void> {
    const genderToDelete = this.selectedGender();
    if (!genderToDelete) return;

    try {
      const success = await this.masterDataService.deleteGender(genderToDelete.id);

      if (success) {
        this.genders.update(list => list.filter(g => g.id !== genderToDelete.id));
      } else {
        this.error.set('Failed to delete gender');
      }
    } catch (err: any) {
      console.error('Failed to delete gender:', err);
      this.error.set(err.message || 'Failed to delete gender');
    } finally {
      this.closeConfirmDialog();
    }
  }

  closeConfirmDialog(): void {
    this.isConfirmDialogOpen.set(false);
    this.selectedGender.set(null);
  }
}
