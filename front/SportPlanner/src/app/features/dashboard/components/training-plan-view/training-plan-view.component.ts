import { Component, input, output, computed, signal, inject, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TrainingPlanDto, PlanObjectiveDto } from '../../services/training-plans.service';
import { ObjectivesService, ObjectiveDto } from '../../services/objectives.service';

interface CategoryGroup {
  categoryId: string;
  categoryName: string;
  isExpanded: boolean;
  objectives: EnrichedObjective[];
}

interface EnrichedObjective extends PlanObjectiveDto {
  description?: string;
  categoryName?: string;
  level?: number;
}

@Component({
  selector: 'app-training-plan-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './training-plan-view.component.html'
})
export class TrainingPlanViewComponent {
  private objectivesService = inject(ObjectivesService);

  // Inputs
  isOpen = input<boolean>(false);
  plan = input<TrainingPlanDto | null>(null);

  // Outputs
  close = output<void>();

  // State
  allObjectives = signal<ObjectiveDto[]>([]);
  isLoadingObjectives = signal(false);
  collapsedCategories = signal<Set<string>>(new Set());

  // Effect: Load all objectives when modal opens
  private loadObjectivesEffect = effect(() => {
    const isModalOpen = this.isOpen();
    const currentPlan = this.plan();

    if (isModalOpen && currentPlan && this.allObjectives().length === 0) {
      this.loadAllObjectives();
    }
  });

  // Computed: Group objectives by category with enriched data
  groupedObjectives = computed<CategoryGroup[]>(() => {
    const currentPlan = this.plan();
    const objectives = this.allObjectives();
    const collapsed = this.collapsedCategories();

    if (!currentPlan?.objectives || currentPlan.objectives.length === 0 || objectives.length === 0) {
      return [];
    }

    // Map plan objectives to full objective data
    const enrichedObjectives: EnrichedObjective[] = currentPlan.objectives
      .map(planObj => {
        const fullObj = objectives.find(o => o.id === planObj.objectiveId);
        return {
          ...planObj,
          description: fullObj?.description,
          categoryName: fullObj?.objectiveCategoryName || 'Sin Categoría',
          level: fullObj?.level
        };
      })
      .filter(obj => obj.categoryName); // Only include objectives with category info

    // Group by category
    const categoryMap = new Map<string, EnrichedObjective[]>();

    for (const objective of enrichedObjectives) {
      const categoryName = objective.categoryName || 'Sin Categoría';

      if (!categoryMap.has(categoryName)) {
        categoryMap.set(categoryName, []);
      }

      categoryMap.get(categoryName)!.push(objective);
    }

    // Convert to array and sort
    return Array.from(categoryMap.entries())
      .map(([categoryName, objectives]) => ({
        categoryId: categoryName,
        categoryName,
        isExpanded: !collapsed.has(categoryName),
        objectives: objectives.sort((a, b) => a.objectiveName.localeCompare(b.objectiveName))
      }))
      .sort((a, b) => a.categoryName.localeCompare(b.categoryName));
  });

  // Computed: Training schedule info
  scheduleInfo = computed(() => {
    const currentPlan = this.plan();
    if (!currentPlan?.schedule) {
      return null;
    }

    const schedule = currentPlan.schedule;
    const daysOfWeek = ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb'];
    const trainingDaysNames = schedule.trainingDays
      ?.map(day => daysOfWeek[day])
      .join(', ') || '-';

    return {
      totalWeeks: schedule.totalWeeks,
      totalSessions: schedule.totalSessions,
      totalHours: schedule.totalHours,
      sessionsPerWeek: schedule.totalSessions && schedule.totalWeeks
        ? Math.round(schedule.totalSessions / schedule.totalWeeks)
        : 0,
      trainingDays: trainingDaysNames
    };
  });

  // Computed: Duration in days
  durationDays = computed(() => {
    const currentPlan = this.plan();
    if (!currentPlan?.startDate || !currentPlan?.endDate) {
      return 0;
    }

    const start = new Date(currentPlan.startDate);
    const end = new Date(currentPlan.endDate);

    return Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24));
  });

  async loadAllObjectives(): Promise<void> {
    try {
      this.isLoadingObjectives.set(true);
      const objectives = await this.objectivesService.getObjectives();
      this.allObjectives.set(objectives || []);
    } catch (error) {
      console.error('❌ Error loading objectives:', error);
      this.allObjectives.set([]);
    } finally {
      this.isLoadingObjectives.set(false);
    }
  }

  toggleCategory(categoryId: string): void {
    const collapsed = new Set(this.collapsedCategories());

    if (collapsed.has(categoryId)) {
      collapsed.delete(categoryId);
    } else {
      collapsed.add(categoryId);
    }

    this.collapsedCategories.set(collapsed);
  }

  expandAll(): void {
    this.collapsedCategories.set(new Set());
  }

  collapseAll(): void {
    const allCategories = this.groupedObjectives().map(g => g.categoryId);
    this.collapsedCategories.set(new Set(allCategories));
  }

  handleClose(): void {
    this.close.emit();
  }

  handleBackdropClick(event: MouseEvent): void {
    // Only close if clicking the backdrop itself, not its children
    if (event.target === event.currentTarget) {
      this.handleClose();
    }
  }
}
