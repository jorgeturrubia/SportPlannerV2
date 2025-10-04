import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataTable } from '../components/data-table/data-table';
import { ConfirmationDialog, ConfirmationConfig } from '../components/confirmation-dialog/confirmation-dialog';
import { DynamicForm } from '../components/dynamic-form/dynamic-form';
import { TableConfig } from '../models/table-config';
import { FormConfig } from '../models/form-config';
import {
  AdvancedAthlete,
  ADVANCED_ATHLETES_100
} from './demo-data';

@Component({
  selector: 'app-sport-tables-design',
  imports: [CommonModule, DataTable, ConfirmationDialog, DynamicForm],
  templateUrl: './sport-tables-design.html',
  styleUrl: './sport-tables-design.css'
})
export class SportTablesDesign {
  // Data
  athletes = signal<AdvancedAthlete[]>(ADVANCED_ATHLETES_100);

  // Dialog states
  isDeleteDialogOpen = signal(false);
  isFormDialogOpen = signal(false);
  formTitle = signal('');
  selectedAthlete = signal<AdvancedAthlete | null>(null);

  // Table Configuration
  tableConfig = signal<TableConfig<AdvancedAthlete>>({
    columns: [
      { key: 'id', label: 'ID', type: 'number', width: 'w-16', align: 'center' },
      { key: 'name', label: 'Name', type: 'text', width: 'min-w-[180px]' },
      { key: 'email', label: 'Email', type: 'text', width: 'min-w-[220px]' },
      {
        key: 'sport',
        label: 'Sport',
        type: 'badge',
        width: 'min-w-[140px]',
        badgeConfig: {
          colorMap: {
            'Football': 'bg-gradient-to-r from-purple-500 to-indigo-600 text-white',
            'Basketball': 'bg-gradient-to-r from-orange-500 to-red-600 text-white',
            'Tennis': 'bg-gradient-to-r from-green-500 to-teal-600 text-white',
            'Swimming': 'bg-gradient-to-r from-blue-500 to-cyan-600 text-white',
            'Athletics': 'bg-gradient-to-r from-yellow-500 to-orange-600 text-white',
            'Cycling': 'bg-gradient-to-r from-lime-500 to-green-600 text-white',
            'Volleyball': 'bg-gradient-to-r from-pink-500 to-purple-600 text-white',
            'Boxing': 'bg-gradient-to-r from-red-500 to-pink-600 text-white',
            'default': 'bg-gray-500 text-white'
          }
        }
      },
      {
        key: 'level',
        label: 'Level',
        type: 'badge',
        width: 'min-w-[140px]',
        badgeConfig: {
          colorMap: {
            'Beginner': 'bg-green-500 text-white',
            'Intermediate': 'bg-blue-500 text-white',
            'Advanced': 'bg-orange-500 text-white',
            'Professional': 'bg-purple-600 text-white',
            'Elite': 'bg-gradient-to-r from-pink-500 to-rose-500 text-white'
          }
        }
      },
      { key: 'age', label: 'Age', type: 'number', width: 'w-16', align: 'center' },
      { key: 'weight', label: 'Weight (kg)', type: 'number', width: 'w-24', align: 'center' },
      { key: 'height', label: 'Height (cm)', type: 'number', width: 'w-24', align: 'center' },
      { key: 'country', label: 'Country', type: 'text', width: 'min-w-[120px]' },
      {
        key: 'status',
        label: 'Status',
        type: 'badge',
        width: 'min-w-[120px]',
        badgeConfig: {
          colorMap: {
            'Active': 'bg-green-500 text-white',
            'Inactive': 'bg-gray-500 text-white',
            'On Leave': 'bg-orange-500 text-white',
            'Injured': 'bg-red-500 text-white',
            'Recovery': 'bg-cyan-500 text-white'
          }
        }
      },
      { key: 'lastSession', label: 'Last Session', type: 'date', width: 'min-w-[130px]' },
      {
        key: 'totalSessions',
        label: 'Sessions',
        type: 'custom',
        width: 'w-32',
        align: 'center',
        customRender: (value) => `<span class="inline-flex items-center justify-center min-w-[40px] px-2 py-1 rounded-lg text-sm font-bold bg-gradient-to-r from-green-400 to-cyan-400 text-gray-900">${value}</span>`
      },
      {
        key: 'actions',
        label: 'Actions',
        type: 'custom',
        width: 'w-40',
        sticky: true,
        customRender: (_, row) => this.renderActions(row)
      }
    ],
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    searchable: true
  });

  // Form Configuration
  formConfig = signal<FormConfig>({
    fields: [
      { key: 'name', label: 'Name', type: 'text', required: true, placeholder: 'Enter athlete name' },
      { key: 'email', label: 'Email', type: 'email', required: true, placeholder: 'athlete@example.com' },
      {
        key: 'sport',
        label: 'Sport',
        type: 'select',
        required: true,
        options: [
          { value: 'Football', label: 'Football' },
          { value: 'Basketball', label: 'Basketball' },
          { value: 'Tennis', label: 'Tennis' },
          { value: 'Swimming', label: 'Swimming' },
          { value: 'Athletics', label: 'Athletics' },
          { value: 'Cycling', label: 'Cycling' },
          { value: 'Volleyball', label: 'Volleyball' },
          { value: 'Boxing', label: 'Boxing' }
        ]
      },
      {
        key: 'level',
        label: 'Level',
        type: 'select',
        required: true,
        options: [
          { value: 'Beginner', label: 'Beginner' },
          { value: 'Intermediate', label: 'Intermediate' },
          { value: 'Advanced', label: 'Advanced' },
          { value: 'Professional', label: 'Professional' },
          { value: 'Elite', label: 'Elite' }
        ]
      },
      { key: 'age', label: 'Age', type: 'number', required: true, min: 18, max: 65 },
      { key: 'weight', label: 'Weight (kg)', type: 'number', required: true, min: 40, max: 150 },
      { key: 'height', label: 'Height (cm)', type: 'number', required: true, min: 140, max: 220 },
      { key: 'country', label: 'Country', type: 'text', required: true, placeholder: 'Enter country' },
      {
        key: 'status',
        label: 'Status',
        type: 'select',
        required: true,
        options: [
          { value: 'Active', label: 'Active' },
          { value: 'Inactive', label: 'Inactive' },
          { value: 'On Leave', label: 'On Leave' },
          { value: 'Injured', label: 'Injured' },
          { value: 'Recovery', label: 'Recovery' }
        ]
      },
      { key: 'lastSession', label: 'Last Session', type: 'date', required: false }
    ],
    submitText: 'Save',
    cancelText: 'Cancel'
  });

  // Confirmation Config
  deleteConfirmConfig = signal<ConfirmationConfig>({
    title: 'Delete Athlete',
    message: 'Are you sure you want to delete this athlete? This action cannot be undone.',
    confirmText: 'Delete',
    cancelText: 'Cancel',
    confirmColor: 'danger'
  });

  // Methods
  renderActions(row: AdvancedAthlete): string {
    return `
      <div class="flex items-center gap-3">
        <button onclick="window.dispatchEvent(new CustomEvent('editAthlete', { detail: ${row.id} }))" class="text-indigo-600 dark:text-indigo-400 hover:text-indigo-900 dark:hover:text-indigo-300 hover:scale-110 transition-all duration-150" title="Edit">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
          </svg>
        </button>
        <button onclick="window.dispatchEvent(new CustomEvent('deleteAthlete', { detail: ${row.id} }))" class="text-red-600 dark:text-red-400 hover:text-red-900 dark:hover:text-red-300 hover:scale-110 transition-all duration-150" title="Delete">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
          </svg>
        </button>
      </div>
    `;
  }

  constructor() {
    // Listen for custom events from action buttons
    if (typeof window !== 'undefined') {
      window.addEventListener('editAthlete', ((e: CustomEvent) => {
        const athlete = this.athletes().find(a => a.id === e.detail);
        if (athlete) this.openEditForm(athlete);
      }) as EventListener);

      window.addEventListener('deleteAthlete', ((e: CustomEvent) => {
        const athlete = this.athletes().find(a => a.id === e.detail);
        if (athlete) this.openDeleteDialog(athlete);
      }) as EventListener);
    }
  }

  openCreateForm(): void {
    this.formTitle.set('Create New Athlete');
    this.selectedAthlete.set(null);
    this.isFormDialogOpen.set(true);
  }

  openEditForm(athlete: AdvancedAthlete): void {
    this.formTitle.set('Edit Athlete');
    this.selectedAthlete.set(athlete);
    this.isFormDialogOpen.set(true);
  }

  openDeleteDialog(athlete: AdvancedAthlete): void {
    this.selectedAthlete.set(athlete);
    this.isDeleteDialogOpen.set(true);
  }

  onFormSubmit(data: any): void {
    if (this.selectedAthlete()) {
      // Edit existing athlete
      const index = this.athletes().findIndex(a => a.id === this.selectedAthlete()!.id);
      if (index !== -1) {
        const updated = [...this.athletes()];
        updated[index] = { ...updated[index], ...data };
        this.athletes.set(updated);
      }
    } else {
      // Create new athlete
      const newAthlete: AdvancedAthlete = {
        id: Math.max(...this.athletes().map(a => a.id)) + 1,
        ...data,
        totalSessions: 0
      };
      this.athletes.set([...this.athletes(), newAthlete]);
    }
    this.isFormDialogOpen.set(false);
  }

  onFormCancel(): void {
    this.isFormDialogOpen.set(false);
  }

  onDeleteConfirm(): void {
    if (this.selectedAthlete()) {
      this.athletes.set(this.athletes().filter(a => a.id !== this.selectedAthlete()!.id));
    }
    this.isDeleteDialogOpen.set(false);
  }

  onDeleteCancel(): void {
    this.isDeleteDialogOpen.set(false);
  }

  onRowClick(athlete: AdvancedAthlete): void {
    console.log('Row clicked:', athlete);
  }
}
