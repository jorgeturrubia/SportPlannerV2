import { Component, Input, Output, EventEmitter, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ObjectiveDto } from '../../../../../features/dashboard/services/objectives.service';

@Component({
  selector: 'app-goal-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './goal-modal.component.html',
  styleUrls: ['./goal-modal.component.css']
})
export class GoalModalComponent {
  @Input() isOpen = false;
  @Input() goal: ObjectiveDto | null = null;
  @Input() availableGoals: ObjectiveDto[] = [];
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<ObjectiveDto>();

  selectedGoalId = signal<string | null>(null);
  priority = signal<number>(5);
  targetSessions = signal<number>(0);

  isEditMode = computed(() => !!this.goal);

  selectedGoal = computed(() => {
    const id = this.selectedGoalId();
    return this.availableGoals.find(g => g.id === id);
  });

  ngOnChanges(): void {
    if (this.isOpen) {
      if (this.goal) {
        // Edit mode
        this.selectedGoalId.set(this.goal.id);
        this.priority.set((this.goal as any).priority || 5);
        this.targetSessions.set((this.goal as any).targetSessions || 0);
      } else {
        // Add mode - reset
        this.selectedGoalId.set(null);
        this.priority.set(5);
        this.targetSessions.set(0);
      }
    }
  }

  onGoalSelect(goalId: string): void {
    this.selectedGoalId.set(goalId);
  }

  onPriorityChange(value: number): void {
    this.priority.set(value);
  }

  onTargetSessionsChange(value: number): void {
    this.targetSessions.set(value);
  }

  onSave(): void {
    const goal = this.selectedGoal();
    if (!goal) return;

    const goalWithMeta = {
      ...goal,
      priority: this.priority(),
      targetSessions: this.targetSessions()
    };

    this.save.emit(goalWithMeta);
  }

  onClose(): void {
    this.close.emit();
  }

  canSave(): boolean {
    return !!this.selectedGoal();
  }

  getPriorityLabel(priority: number): string {
    if (priority >= 8) return 'Crítica';
    if (priority >= 5) return 'Alta';
    if (priority >= 3) return 'Media';
    return 'Baja';
  }

  getPriorityColor(priority: number): string {
    if (priority >= 8) return 'text-red-600';
    if (priority >= 5) return 'text-orange-600';
    if (priority >= 3) return 'text-yellow-600';
    return 'text-green-600';
  }
}
