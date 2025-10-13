import { Component, Input, Output, EventEmitter, signal, inject, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ObjectivesService, ObjectiveDto } from '../../../features/dashboard/services/objectives.service';
import { ObjectiveCategoriesService } from '../../../features/dashboard/services/objective-categories.service';
import { ObjectiveSubcategoriesService } from '../../../features/dashboard/services/objective-subcategories.service';
import { TrainingPlansService } from '../../../features/dashboard/services/training-plans.service';
import { GoalCardComponent } from './components/goal-card/goal-card.component';
import { GoalFiltersComponent } from './components/goal-filters/goal-filters.component';
import { GoalModalComponent } from './components/goal-modal/goal-modal.component';

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
    GoalCardComponent,
    GoalFiltersComponent,
    GoalModalComponent
  ],
  templateUrl: './plan-goals-manager.component.html',
  styleUrls: ['./plan-goals-manager.component.css']
})
export class PlanGoalsManagerComponent implements OnInit {
  private objectivesService = inject(ObjectivesService);
  private categoriesService = inject(ObjectiveCategoriesService);
  private subcategoriesService = inject(ObjectiveSubcategoriesService);
  private plansService = inject(TrainingPlansService);

  @Input() planId: string | null = null;
  @Input() viewOnly = false;
  @Input() isOpen = false;
  @Output() close = new EventEmitter<void>();
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
  isMaximized = signal(false);

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

  async removeGoalFromPlan(goal: PlanObjective): Promise<void> {
    if (!this.planId || this.viewOnly) return;

    this.goalToDelete.set(goal);
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

  openAddModal(): void {
    this.selectedGoal.set(null);
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

  toggleMaximize(): void {
    this.isMaximized.update(v => !v);
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
}
