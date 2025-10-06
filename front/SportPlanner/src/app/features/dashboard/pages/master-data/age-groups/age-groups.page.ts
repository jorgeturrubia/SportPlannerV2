import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { EntityPageLayoutComponent } from '../../../../../shared/components/entity-page-layout/entity-page-layout.component';
import { DataTableComponent, TableColumn, TableAction } from '../../../../../shared/components/data-table/data-table.component';
import { DynamicFormComponent, FormField } from '../../../../../shared/components/dynamic-form/dynamic-form.component';
import { ConfirmationDialogComponent } from '../../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { MasterDataService, AgeGroupResponse } from '../../../services/master-data.service';

enum Sport {
  Football = 0,
  Basketball = 1,
  Handball = 2
}

@Component({
  selector: 'app-age-groups-page',
  standalone: true,
  imports: [CommonModule, TranslateModule, EntityPageLayoutComponent, DataTableComponent, DynamicFormComponent, ConfirmationDialogComponent],
  templateUrl: './age-groups.page.html'
})
export class AgeGroupsPage implements OnInit {
  private masterDataService = inject(MasterDataService);

  ageGroups = signal<AgeGroupResponse[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  // Form state
  isFormOpen = signal(false);
  selectedAgeGroup = signal<AgeGroupResponse | null>(null);
  formTitle = 'Add Age Group';

  // Confirmation dialog state
  isConfirmDialogOpen = signal(false);

  columns: TableColumn[] = [
    { key: 'name', label: 'Name', sortable: true },
    { key: 'code', label: 'Code', sortable: true },
    { key: 'minAge', label: 'Min Age', sortable: true, type: 'number' },
    { key: 'maxAge', label: 'Max Age', sortable: true, type: 'number' },
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
    { key: 'minAge', label: 'Min Age', type: 'number', required: true },
    { key: 'maxAge', label: 'Max Age', type: 'number', required: true },
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
    await this.loadAgeGroups();
  }

  private async loadAgeGroups(): Promise<void> {
    try {
      this.isLoading.set(true);
      this.error.set(null);

      const data = await this.masterDataService.getAgeGroups();
      this.ageGroups.set(data || []);
    } catch (err: any) {
      console.error('Failed to load age groups:', err);
      this.error.set(err.message || 'Failed to load age groups');
      this.ageGroups.set([]);
    } finally {
      this.isLoading.set(false);
    }
  }

  openAddForm(): void {
    this.selectedAgeGroup.set(null);
    this.formTitle = 'Add Age Group';
    this.isFormOpen.set(true);
  }

  openEditForm(ageGroup: AgeGroupResponse): void {
    this.selectedAgeGroup.set(ageGroup);
    this.formTitle = `Edit ${ageGroup.name}`;
    this.isFormOpen.set(true);
  }

  async handleFormSubmit(formData: any): Promise<void> {
    const selected = this.selectedAgeGroup();

    try {
      if (selected) {
        // Update existing age group
        const updated = await this.masterDataService.updateAgeGroup(selected.id, formData);
        if (updated) {
          this.ageGroups.update(list =>
            list.map(ag => ag.id === selected.id ? updated : ag)
          );
        }
      } else {
        // Create new age group
        const created = await this.masterDataService.createAgeGroup(formData);
        if (created) {
          this.ageGroups.update(list => [...list, created]);
        }
      }

      this.closeForm();
    } catch (err: any) {
      console.error('Failed to save age group:', err);
      this.error.set(err.message || 'Failed to save age group');
    }
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedAgeGroup.set(null);
  }

  openDeleteConfirm(ageGroup: AgeGroupResponse): void {
    this.selectedAgeGroup.set(ageGroup);
    this.isConfirmDialogOpen.set(true);
  }

  async handleDeleteConfirm(): Promise<void> {
    const ageGroupToDelete = this.selectedAgeGroup();
    if (!ageGroupToDelete) return;

    try {
      const success = await this.masterDataService.deleteAgeGroup(ageGroupToDelete.id);

      if (success) {
        this.ageGroups.update(list => list.filter(ag => ag.id !== ageGroupToDelete.id));
      } else {
        this.error.set('Failed to delete age group');
      }
    } catch (err: any) {
      console.error('Failed to delete age group:', err);
      this.error.set(err.message || 'Failed to delete age group');
    } finally {
      this.closeConfirmDialog();
    }
  }

  closeConfirmDialog(): void {
    this.isConfirmDialogOpen.set(false);
    this.selectedAgeGroup.set(null);
  }
}
