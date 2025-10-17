import { Component, Input, Output, EventEmitter, signal, inject, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ObjectivesService, ObjectiveDto, CreateObjectiveDto, Sport } from '../../../features/dashboard/services/objectives.service';
import { ObjectiveCategoriesService } from '../../../features/dashboard/services/objective-categories.service';
import { ObjectiveSubcategoriesService } from '../../../features/dashboard/services/objective-subcategories.service';
import { TrainingPlansService } from '../../../features/dashboard/services/training-plans.service';
import { SubscriptionContextService } from '../../../core/subscription/services/subscription-context.service';
import { NotificationService } from '../../notifications/notification.service';
import { DynamicFormComponent, FormField } from '../dynamic-form/dynamic-form.component';
import { GoalCardComponent } from './components/goal-card/goal-card.component';
import { GoalFiltersComponent } from './components/goal-filters/goal-filters.component';
import { GoalModalComponent } from './components/goal-modal/goal-modal.component';
import { ObjectiveTreeComponent } from '../objective-tree/objective-tree.component';

export interface PlanObjective extends ObjectiveDto {
  priority?: number;
  targetSessions?: number;
  addedDate?: string;
}

export interface FilterState {
  search: string;
  categoryId: string | null;
  subcategoryId: string | null;
  sortBy: 'name' | 'dateAdded' | 'priority';
  viewMode: 'grid' | 'list';
}

@Component({
  selector: 'app-plan-goals-manager',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DynamicFormComponent,
    GoalCardComponent,
    GoalFiltersComponent,
    GoalModalComponent
    ,ObjectiveTreeComponent
  ],
  templateUrl: './plan-goals-manager.component.html',
  styleUrls: ['./plan-goals-manager.component.css']
})
export class PlanGoalsManagerComponent implements OnInit {
  private objectivesService = inject(ObjectivesService);
  private categoriesService = inject(ObjectiveCategoriesService);
  private subcategoriesService = inject(ObjectiveSubcategoriesService);
  private plansService = inject(TrainingPlansService);
  private subscriptionContext = inject(SubscriptionContextService);
  private ns = inject(NotificationService);

  @Input() planId: string | null = null;
  @Input() viewOnly = false;
  @Input() isOpen = false;
  @Output() close = new EventEmitter<void>();
  @Output() objectiveCreated = new EventEmitter<ObjectiveDto>();
  @Output() changed = new EventEmitter<void>();

  // State
  allObjectives = signal<ObjectiveDto[]>([]);
  planObjectives = signal<PlanObjective[]>([]);
  categories = signal<any[]>([]);
  subcategories = signal<any[]>([]);
  isLoading = signal(false);
  isModalOpen = signal(false);
  selectedGoal = signal<PlanObjective | null>(null);
  goalToDelete = signal<PlanObjective | null>(null);
  
  // Create Form State
  showCreateForm = signal(false);
  isCreatingObjective = signal(false);
  createFormFields = signal<FormField[]>([]);

  // Filters
  filterState = signal<FilterState>({
    search: '',
    categoryId: null,
    subcategoryId: null,
    sortBy: 'name',
    viewMode: 'grid'
  });

  // Computed
  filteredGoals = computed(() => {
    let goals = this.viewOnly
      ? this.planObjectives()
      : this.allObjectives();

    const filters = this.filterState();

    // Search filter
    if (filters.search) {
      const search = filters.search.toLowerCase();
      goals = goals.filter(g =>
        g.name.toLowerCase().includes(search) ||
        (g.description || '').toLowerCase().includes(search)
      );
    }

    // Category filter
    if (filters.categoryId) {
      goals = goals.filter(g => g.objectiveCategoryId === filters.categoryId);
    }

    // Subcategory filter
    if (filters.subcategoryId) {
      goals = goals.filter(g => g.objectiveSubcategoryId === filters.subcategoryId);
    }

    // Sort
    return this.sortGoals(goals, filters.sortBy);
  });

  // Expose simple getters for template binding
  availableObjectives = this.allObjectives;
  isEditing = computed(() => this.isEditingValue);

  selectedGoalIds = computed(() => {
    return new Set(this.planObjectives().map(g => g.id));
  });

  stats = computed(() => {
    const total = this.planObjectives().length;
    const categories = new Set(this.planObjectives().map(g => g.objectiveCategoryId)).size;
    return { total, categories };
  });

  async ngOnInit(): Promise<void> {
    await this.loadData();
  }

  async loadData(): Promise<void> {
    this.isLoading.set(true);
    try {
      const [objectives, categories] = await Promise.all([
        this.objectivesService.getObjectives(),
        this.categoriesService.getCategories()
      ]);

      this.allObjectives.set(objectives || []);
      this.categories.set(categories || []);

      if (this.planId) {
        await this.loadPlanObjectives();
      }
    } catch (err) {
      console.error('Failed to load data:', err);
    } finally {
      this.isLoading.set(false);
    }
  }

  async loadPlanObjectives(): Promise<void> {
    if (!this.planId) return;

    try {
      const plan = await this.plansService.getPlan(this.planId);
      const planGoals: PlanObjective[] = (plan.objectives || []).map(o => {
        const full = this.allObjectives().find(obj => obj.id === o.objectiveId);
        return {
          ...(full || { id: o.objectiveId, name: o.objectiveName, description: '' } as ObjectiveDto),
          priority: o.priority,
          targetSessions: o.targetSessions
        };
      });
      this.planObjectives.set(planGoals);
    } catch (err) {
      console.error('Failed to load plan objectives:', err);
    }
  }

  async onCategoryChange(categoryId: string | null): Promise<void> {
    this.filterState.update(f => ({ ...f, categoryId, subcategoryId: null }));

    if (categoryId) {
      const subs = await this.subcategoriesService.getSubcategories(categoryId);
      this.subcategories.set(subs || []);
    } else {
      this.subcategories.set([]);
    }
  }

  onFiltersChange(filters: Partial<FilterState>): void {
    this.filterState.update(f => ({ ...f, ...filters }));
  }

  sortGoals(goals: any[], sortBy: string): any[] {
    const sorted = [...goals];

    switch (sortBy) {
      case 'name':
        return sorted.sort((a, b) => a.name.localeCompare(b.name));
      case 'dateAdded':
        return sorted.sort((a, b) =>
          (b.addedDate || '').localeCompare(a.addedDate || '')
        );
      case 'priority':
        return sorted.sort((a, b) =>
          (b.priority || 0) - (a.priority || 0)
        );
      default:
        return sorted;
    }
  }

  async addGoalToPlan(goal: ObjectiveDto): Promise<void> {
    if (!this.planId || this.viewOnly) return;

    // If we're in editing mode, only add to the in-memory planObjectives list
    if (!this.viewOnly && this.planId) {
      const editing = this.isEditingValue;
      if (editing) {
        // Avoid duplicates
        const exists = this.planObjectives().some(p => p.id === goal.id);
        if (!exists) {
          const newEntry: PlanObjective = {
            ...goal,
            priority: 0,
            targetSessions: 0,
            addedDate: new Date().toISOString()
          };
          this.planObjectives.update(list => [...list, newEntry]);
          // ensure child components see the updated reference asap
          queueMicrotask(() => this.changed.emit());
        }
        return;
      }

      // Fallback: persist immediately if not editing (legacy behavior)
      try {
        await this.plansService.addObjectiveToPlan(this.planId, {
          objectiveId: goal.id,
          priority: 0,
          targetSessions: 0
        });

        await this.loadPlanObjectives();
        this.changed.emit();
      } catch (err) {
        console.error('Failed to add goal to plan:', err);
      }
    }
  }

  async removeGoalFromPlan(obj: ObjectiveDto): Promise<void> {
    if (!this.planId || this.viewOnly) return;

    // find matching plan objective by id
    const planGoal = this.planObjectives().find(p => p.id === obj.id);
    if (!planGoal) {
      // nothing to remove
      return;
    }

    // If we're editing (working in-memory), remove immediately from the list
    if (this.isEditingValue) {
      this.planObjectives.update(list => list.filter(p => p.id !== obj.id));
      this.changed.emit();
      return;
    }

    // Otherwise, ask for confirmation and persist removal
    this.goalToDelete.set(planGoal);
  }

  // Helpers exposed to template
  get viewMode(): string {
    return this.filterState().viewMode;
  }

  get availableObjectivesValue(): ObjectiveDto[] {
    return this.filteredGoals();
  }

  get isEditingValue(): boolean {
    return !this.viewOnly && !!this.planId;
  }

  get selectedIdsArray(): string[] {
    return this.planObjectives().map(p => p.id);
  }

  get goalsList(): PlanObjective[] {
    return this.filteredGoals();
  }

  async confirmDelete(): Promise<void> {
    const goal = this.goalToDelete();
    if (!goal || !this.planId) return;

    try {
      await this.plansService.removeObjectiveFromPlan(this.planId, goal.id);
      await this.loadPlanObjectives();
      this.changed.emit();
      this.goalToDelete.set(null);
    } catch (err) {
      console.error('Failed to remove goal from plan:', err);
    }
  }

  cancelDelete(): void {
    this.goalToDelete.set(null);
  }

  openCreateNewObjectiveModal(): void {
    // Preparar los campos del formulario
    this.createFormFields.set([
      {
        key: 'name',
        label: 'Nombre',
        type: 'text',
        required: true
      },
      {
        key: 'description',
        label: 'Descripción',
        type: 'textarea',
        required: false
      },
      {
        key: 'objectiveCategoryId',
        label: 'Categoría',
        type: 'select',
        required: true,
        options: this.categories().map(c => ({ value: c.id, label: c.name }))
      },
      {
        key: 'objectiveSubcategoryId',
        label: 'Subcategoría',
        type: 'select',
        required: false,
        options: this.subcategories().map(s => ({ value: s.id, label: s.name }))
      }
      ,
      {
        key: 'level',
        label: 'Nivel',
        type: 'number',
        required: false
      }
    ]);
    
    this.showCreateForm.set(true);
  }

  openAddExistingGoalModal(goal?: ObjectiveDto): void {
    // Si se pasa un goal específico, lo pre-seleccionamos
    if (goal) {
      this.selectedGoal.set(goal as PlanObjective);
    } else {
      this.selectedGoal.set(null);
    }
    this.isModalOpen.set(true);
  }

  openEditModal(goal: PlanObjective): void {
    this.selectedGoal.set(goal);
    this.isModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.selectedGoal.set(null);
  }

  async handleModalSave(goal: ObjectiveDto): Promise<void> {
    await this.addGoalToPlan(goal);
    this.closeModal();
  }

  onClose(): void {
    this.close.emit();
  }

  isGoalSelected(goalId: string): boolean {
    return this.selectedGoalIds().has(goalId);
  }

  trackByGoalId(index: number, goal: PlanObjective): string {
    return goal.id;
  }

  // Create Objective Form Methods
  async handleCreateFormSubmit(formData: any): Promise<void> {
    this.isCreatingObjective.set(true);

    try {
      const subscription = this.subscriptionContext.subscription();
      if (!subscription) {
        this.ns.error('No se pudo obtener la suscripción', 'Error');
        return;
      }

      const sport = this.parseSportToEnum(subscription.sport);

      const createPayload: CreateObjectiveDto = {
        sport: sport,
        name: formData.name,
        description: formData.description,
        objectiveCategoryId: formData.objectiveCategoryId,
        objectiveSubcategoryId: formData.objectiveSubcategoryId || undefined,
        level: formData.level ?? 1,
        techniques: []
      };

      const newObjectiveId = await this.objectivesService.createObjective(createPayload);
      
      // Recargar la lista de objetivos
      await this.loadData();
      
      // Buscar el objetivo recién creado en la lista
      const newObjective = this.allObjectives().find(obj => obj.id === newObjectiveId);
      if (newObjective) {
        this.objectiveCreated.emit(newObjective);
      }
      
      // Cerrar el formulario
      this.closeCreateForm();
      
      this.ns.success('Objetivo creado correctamente', 'Objetivos');
    } catch (err: any) {
      console.error('Failed to create objective:', err);
      this.ns.error(err?.message ?? 'No se pudo crear el objetivo', 'Error');
    } finally {
      this.isCreatingObjective.set(false);
    }
  }

  closeCreateForm(): void {
    this.showCreateForm.set(false);
    this.createFormFields.set([]);
  }

  private parseSportToEnum(sportString: string): Sport {
    const sportMap: Record<string, Sport> = {
      'Basketball': Sport.Basketball,
      'Football': Sport.Football,
      'Handball': Sport.Handball
    };
    return sportMap[sportString] || Sport.Basketball;
  }
}
