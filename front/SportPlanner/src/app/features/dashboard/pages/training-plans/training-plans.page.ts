import { Component, signal, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { DataTableComponent, TableColumn, TableAction } from '../../../../shared/components/data-table/data-table.component';
import { DynamicFormComponent, FormField } from '../../../../shared/components/dynamic-form/dynamic-form.component';
import { TrainingPlansService, TrainingPlanDto } from '../../services/training-plans.service';
import { NotificationService } from '../../../../shared/notifications/notification.service';

@Component({
  selector: 'app-training-plans-page',
  standalone: true,
  imports: [CommonModule, TranslateModule, DataTableComponent, DynamicFormComponent],
  templateUrl: './training-plans.page.html'
})
export class TrainingPlansPage implements OnInit {
  private plansService = inject(TrainingPlansService);
  private ns = inject(NotificationService);

  plans = signal<TrainingPlanDto[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  // Form state
  isFormOpen = signal(false);
  selectedPlan = signal<TrainingPlanDto | null>(null);
  formTitle = 'Add Training Plan';

  // Confirmation dialog state
  isConfirmDialogOpen = signal(false);

  columns: TableColumn[] = [
    { key: 'name', label: 'Nombre', sortable: true },
    {
      key: 'startDate',
      label: 'Fecha Inicio',
      sortable: true,
      type: 'date'
    },
    {
      key: 'endDate',
      label: 'Fecha Fin',
      sortable: true,
      type: 'date'
    },
    {
      key: 'durationDays',
      label: 'Duración (días)',
      sortable: true,
      type: 'number'
    },
    {
      key: 'schedule',
      label: 'Sesiones/Semana',
      sortable: false,
      formatter: (value: any) => value?.sessionsPerWeek || '-'
    },
    { key: 'isActive', label: 'Estado', sortable: true, type: 'badge' }
  ];

  actions: TableAction[] = [
    {
      icon: 'M15 12a3 3 0 11-6 0 3 3 0 016 0z M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z',
      label: 'Ver Detalles',
      color: 'blue',
      handler: (row) => this.viewPlan(row)
    },
    {
      icon: 'M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z',
      label: 'Editar',
      color: 'green',
      handler: (row) => this.openEditForm(row)
    }
  ];

  formConfig: FormField[] = [
    { key: 'name', label: 'Nombre del Plan', type: 'text', required: true },
    { key: 'startDate', label: 'Fecha Inicio', type: 'date', required: true },
    { key: 'endDate', label: 'Fecha Fin', type: 'date', required: true },
    {
      key: 'schedule.sessionsPerWeek',
      label: 'Sesiones por Semana',
      type: 'number',
      required: true
    },
    { key: 'isActive', label: 'Activo', type: 'checkbox', required: false }
  ];

  async ngOnInit(): Promise<void> {
    await this.loadPlans();
  }

  private async loadPlans(): Promise<void> {
    try {
      this.isLoading.set(true);
      this.error.set(null);

      const data = await this.plansService.getPlans();
      this.plans.set(data || []);
    } catch (err: any) {
      console.error('Failed to load training plans:', err);
      this.error.set(err.message || 'Failed to load plans');
      this.ns.error(err?.message ?? 'Error al cargar planes', 'Error');
      this.plans.set([]);
    } finally {
      this.isLoading.set(false);
    }
  }

  openAddForm(): void {
    this.selectedPlan.set(null);
    this.formTitle = 'Add Training Plan';
    this.isFormOpen.set(true);
  }

  openEditForm(plan: TrainingPlanDto): void {
    this.selectedPlan.set(plan);
    this.formTitle = `Edit ${plan.name}`;
    this.isFormOpen.set(true);
  }

  viewPlan(plan: TrainingPlanDto): void {
    // TODO: Navigate to plan details
    this.ns.info(`Ver detalles del plan: ${plan.name}`, 'Planes de Entrenamiento');
  }

  async handleFormSubmit(formData: any): Promise<void> {
    const selected = this.selectedPlan();

    try {
      if (selected) {
        // Update existing plan
        const updateDto = {
          id: selected.id,
          name: formData.name,
          startDate: formData.startDate,
          endDate: formData.endDate,
          schedule: {
            sessionsPerWeek: formData['schedule.sessionsPerWeek']
          },
          isActive: formData.isActive ?? selected.isActive
        };
        await this.plansService.updatePlan(selected.id, updateDto);
        this.ns.success('Plan actualizado', 'Planes de Entrenamiento');
      } else {
        // Create new plan
        const createDto = {
          name: formData.name,
          startDate: formData.startDate,
          endDate: formData.endDate,
          schedule: {
            sessionsPerWeek: formData['schedule.sessionsPerWeek']
          }
        };
        await this.plansService.createPlan(createDto);
        this.ns.success('Plan creado', 'Planes de Entrenamiento');
      }

      await this.loadPlans();
      this.closeForm();
    } catch (err: any) {
      console.error('Failed to save training plan:', err);
      this.error.set(err.message || 'Failed to save plan');
      this.ns.error(err?.message ?? 'No se pudo guardar el plan', 'Error');
    }
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedPlan.set(null);
  }
}
