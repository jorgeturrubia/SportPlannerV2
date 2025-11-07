import { Component, signal, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { DataTableComponent, TableColumn, TableAction } from '../../../../shared/components/data-table/data-table.component';
import { ObjectiveSelectorComponent } from '../../components/objective-selector/objective-selector';
import { TrainingPlanViewComponent } from '../../components/training-plan-view/training-plan-view.component';
import { DynamicFormComponent, FormField } from '../../../../shared/components/dynamic-form/dynamic-form.component';
import { TrainingPlansService, TrainingPlanDto } from '../../services/training-plans.service';
import { NotificationService } from '../../../../shared/notifications/notification.service';

@Component({
  selector: 'app-training-plans-page',
  standalone: true,
  imports: [CommonModule, TranslateModule, DataTableComponent, DynamicFormComponent, ObjectiveSelectorComponent, TrainingPlanViewComponent],
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

  // Add objectives modal state
  isAddObjectivesOpen = signal(false);
  planForAddObjectives = signal<TrainingPlanDto | null>(null);

  // View plan modal state
  isViewModalOpen = signal(false);
  planForView = signal<TrainingPlanDto | null>(null);

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
      formatter: (value: any) =>
        value?.totalSessions && value?.totalWeeks
          ? Math.round(value.totalSessions / value.totalWeeks).toString()
          : '-'
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
      handler: async (row) => await this.openEditForm(row)
    }
    ,
    {
      icon: 'M16 7l-8 8m0-8l8 8',
      label: 'Asignar Objetivos',
      color: 'yellow',
      handler: async (row) => await this.openManageObjectives(row),
      showLabel: true
    },
    {
      icon: 'M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3H4v2h16V7h-3z',
      label: 'Eliminar',
      color: 'red',
      handler: async (row) => await this.deletePlan(row),
      showLabel: true
    }
  ];

  formConfig: FormField[] = [
    { key: 'name', label: 'Nombre del Plan', type: 'text', required: true },
    { key: 'startDate', label: 'Fecha Inicio', type: 'date', required: true },
    { key: 'endDate', label: 'Fecha Fin', type: 'date', required: true },
    {
      key: 'sessionsPerWeek',
      label: 'Sesiones por Semana',
      type: 'number',
      required: true
    },
    {
      key: 'hoursPerSession',
      label: 'Horas por Sesión',
      type: 'number',
      required: true
    },
    { key: 'isActive', label: 'Activo', type: 'checkbox', required: false }
  ];

  async ngOnInit(): Promise<void> {
    await this.loadPlans();
  }

  async loadPlans(): Promise<void> {
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

  // Handler invoked when modal reports final objectives list
  async onObjectivesAdded(objectiveIds: string[]): Promise<void> {
    const plan = this.planForAddObjectives();
    if (!plan) {
      return;
    }

    try {
      this.isLoading.set(true);

      console.log('📋 onObjectivesAdded - Final objective IDs to update:', objectiveIds);

      // Convert objective IDs to AddObjectiveBatchItem with default priority/targetSessions
      const objectives = objectiveIds.map(objectiveId => ({
        objectiveId,
        priority: 3, // Default priority (1-5, 3 = medium)
        targetSessions: 5 // Default target sessions
      }));

      // Call backend API to UPDATE the plan with new objective list
      // This replaces the entire objectives collection, not adds to it
      await this.plansService.updatePlanObjectives(plan.id, objectives);

      this.ns.success(`Plan actualizado con ${objectiveIds.length} objetivo(s)`, 'Éxito');
      this.closeObjectivesModal();
      
      // Reload plans to show updated objectives
      await this.loadPlans();
    } catch (err: any) {
      console.error('Failed to update plan objectives:', err);
      this.ns.error(
        err?.error?.message || 'Error al actualizar objetivos',
        'Error'
      );
    } finally {
      this.isLoading.set(false);
    }
  }

  // Helper to provide plan id safely to template
  currentPlanId(): string | null {
    const p = this.planForAddObjectives();
    return p ? p.id : null;
  }

  closeObjectivesModal(): void {
    this.isAddObjectivesOpen.set(false);
    this.planForAddObjectives.set(null);
  }

  closeViewModal(): void {
    this.isViewModalOpen.set(false);
    this.planForView.set(null);
  }

  openAddForm(): void {
    this.selectedPlan.set(null);
    this.formTitle = 'Add Training Plan';
    this.isFormOpen.set(true);
  }

  async openEditForm(plan: TrainingPlanDto): Promise<void> {
    try {
      console.log('📝 openEditForm - Loading fresh data for plan ID:', plan.id);

      // Load fresh data from backend instead of using list data
      const freshPlan = await this.plansService.getPlan(plan.id);
      console.log('📝 openEditForm - Fresh plan data loaded:', freshPlan);

      console.log('📝 startDate from backend:', freshPlan.startDate, 'type:', typeof freshPlan.startDate);
      console.log('📝 endDate from backend:', freshPlan.endDate, 'type:', typeof freshPlan.endDate);

      // Convert dates from ISO strings to YYYY-MM-DD format for HTML date inputs
      const startDateFormatted = freshPlan.startDate ? new Date(freshPlan.startDate).toISOString().split('T')[0] : '';
      const endDateFormatted = freshPlan.endDate ? new Date(freshPlan.endDate).toISOString().split('T')[0] : '';

      console.log('📝 Converted startDate for form:', startDateFormatted);
      console.log('📝 Converted endDate for form:', endDateFormatted);

      // Map the plan data to form structure
      const formData = {
        ...freshPlan,
        startDate: startDateFormatted,
        endDate: endDateFormatted,
        sessionsPerWeek: freshPlan.schedule?.totalSessions ? Math.round(freshPlan.schedule.totalSessions / freshPlan.schedule.totalWeeks) : 0,
        hoursPerSession: freshPlan.schedule?.totalHours && freshPlan.schedule?.totalSessions
          ? Math.round(freshPlan.schedule.totalHours / freshPlan.schedule.totalSessions)
          : 0
      };

      console.log('📝 Final formData for editing:', formData);
      this.selectedPlan.set(formData as any);
      this.formTitle = `Edit ${freshPlan.name}`;
      this.isFormOpen.set(true);
    } catch (error) {
      console.error('❌ Error loading plan for editing:', error);
      // TODO: Show error notification to user
    }
  }

  async viewPlan(plan: TrainingPlanDto): Promise<void> {
    try {
      console.log('👁️ viewPlan - Loading plan ID:', plan.id);

      // Load fresh plan data with ALL objectives and category info
      const freshPlan = await this.plansService.getPlan(plan.id);
      console.log('👁️ viewPlan - Fresh plan loaded:', freshPlan);
      console.log('👁️ viewPlan - Objectives in plan:', freshPlan?.objectives?.length ?? 0);

      // Set the plan with complete data and open view modal
      this.planForView.set(freshPlan);
      this.isViewModalOpen.set(true);
    } catch (error) {
      console.error('❌ Error loading plan for viewing:', error);
      this.ns.error('Error al cargar el plan', 'Error');
    }
  }

  async openManageObjectives(plan: TrainingPlanDto): Promise<void> {
    try {
      console.log('📋 openManageObjectives - Loading plan ID:', plan.id);

      // Load fresh plan data with ALL objectives and category info
      const freshPlan = await this.plansService.getPlan(plan.id);
      console.log('📋 openManageObjectives - Fresh plan loaded:', freshPlan);
      console.log('📋 openManageObjectives - Objectives in plan:', freshPlan?.objectives?.length ?? 0);

      // Set the plan with complete data (including objectives) - editable mode
      this.planForAddObjectives.set(freshPlan);
      this.isAddObjectivesOpen.set(true);
    } catch (error) {
      console.error('❌ Error loading plan for managing objectives:', error);
      this.ns.error('Error al cargar el plan', 'Error');
    }
  }

  async deletePlan(plan: TrainingPlanDto): Promise<void> {
    try {
      const confirmed = confirm(`¿Estás seguro de que quieres eliminar el plan "${plan.name}"?`);
      if (!confirmed) {
        return;
      }

      this.isLoading.set(true);
      await this.plansService.deletePlan(plan.id);
      this.ns.success(`Plan "${plan.name}" eliminado correctamente`, 'Éxito');
      
      // Reload plans to update the list
      await this.loadPlans();
    } catch (err: any) {
      console.error('❌ Error deleting plan:', err);
      this.ns.error(
        err?.error?.message || 'Error al eliminar el plan',
        'Error'
      );
    } finally {
      this.isLoading.set(false);
    }
  }

  async handleFormSubmit(formData: any): Promise<void> {
    const selected = this.selectedPlan();

    try {
      console.log('🔍 Form data received:', formData);
      console.log('🔍 startDate type:', typeof formData.startDate, 'value:', formData.startDate);
      console.log('🔍 endDate type:', typeof formData.endDate, 'value:', formData.endDate);

      // Calculate training schedule based on form data
      const startDate = new Date(formData.startDate);
      const endDate = new Date(formData.endDate);

      console.log('🔍 Parsed startDate:', startDate, 'isValid:', !isNaN(startDate.getTime()));
      console.log('🔍 Parsed endDate:', endDate, 'isValid:', !isNaN(endDate.getTime()));

      const durationDays = Math.ceil((endDate.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24));
      const totalWeeks = Math.ceil(durationDays / 7);

      console.log('🔍 Calculated durationDays:', durationDays, 'totalWeeks:', totalWeeks);

      // Generate training days based on sessions per week
      const sessionsPerWeek = Number(formData.sessionsPerWeek);
      const hoursPerSession = Number(formData.hoursPerSession);

      // Map sessions to days (Mon=1, Tue=2, Wed=3, Thu=4, Fri=5, Sat=6, Sun=0)
      const trainingDays: number[] = [];
      const hoursPerDay: { [key: number]: number } = {};

      // Distribute sessions evenly across the week
      const dayMapping = [1, 3, 5, 2, 4, 6, 0]; // Mon, Wed, Fri, Tue, Thu, Sat, Sun
      for (let i = 0; i < Math.min(sessionsPerWeek, 7); i++) {
        const day = dayMapping[i];
        trainingDays.push(day);
        hoursPerDay[day] = hoursPerSession;
      }

      // Convert dates to ISO string format for backend
      const startDateISO = startDate.toISOString().split('T')[0]; // YYYY-MM-DD format
      const endDateISO = endDate.toISOString().split('T')[0];

      console.log('🔍 Converted to ISO format - startDate:', startDateISO, 'endDate:', endDateISO);

      if (selected) {
        // Update existing plan
        const updateDto = {
          id: selected.id,
          name: formData.name,
          startDate: startDateISO,
          endDate: endDateISO,
          schedule: {
            trainingDays,
            hoursPerDay,
            totalWeeks,
            totalSessions: sessionsPerWeek * totalWeeks,
            totalHours: sessionsPerWeek * hoursPerSession * totalWeeks
          },
          isActive: formData.isActive ?? selected.isActive
        };

        console.log('🔍 Update DTO:', updateDto);
        await this.plansService.updatePlan(selected.id, updateDto);
        this.ns.success('Plan actualizado', 'Planes de Entrenamiento');
      } else {
        // Create new plan
        const createDto = {
          name: formData.name,
          startDate: startDateISO,
          endDate: endDateISO,
          schedule: {
            trainingDays,
            hoursPerDay,
            totalWeeks,
            totalSessions: sessionsPerWeek * totalWeeks,
            totalHours: sessionsPerWeek * hoursPerSession * totalWeeks
          },
          isActive: formData.isActive ?? true,
          objectives: []
        };

        console.log('🔍 Create DTO:', createDto);
        await this.plansService.createPlan(createDto);
        this.ns.success('Plan creado', 'Planes de Entrenamiento');
      }

      await this.loadPlans();
      this.closeForm();
    } catch (err: any) {
      console.error('❌ Failed to save training plan:', err);
      this.error.set(err.message || 'Failed to save plan');
      this.ns.error(err?.message ?? 'No se pudo guardar el plan', 'Error');
    }
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedPlan.set(null);
  }
}

