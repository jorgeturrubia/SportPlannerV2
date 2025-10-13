import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlanObjective } from '../../plan-goals-manager.component';

@Component({
  selector: 'app-goal-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './goal-card.component.html',
  styleUrls: ['./goal-card.component.css']
})
export class GoalCardComponent {
  @Input() goal!: PlanObjective;
  @Input() isSelected = false;
  @Input() viewMode: 'grid' | 'list' = 'grid';
  @Input() viewOnly = false;
  @Output() add = new EventEmitter<PlanObjective>();
  @Output() remove = new EventEmitter<PlanObjective>();
  @Output() edit = new EventEmitter<PlanObjective>();

  onAdd(): void {
    if (!this.viewOnly) {
      this.add.emit(this.goal);
    }
  }

  onRemove(): void {
    if (!this.viewOnly) {
      this.remove.emit(this.goal);
    }
  }

  onEdit(): void {
    if (!this.viewOnly) {
      this.edit.emit(this.goal);
    }
  }

  getCategoryColor(categoryId: string | undefined): string {
    if (!categoryId) return 'bg-gray-100 text-gray-800';

    // Generate consistent colors based on category ID
    const colors = [
      'bg-blue-100 text-blue-800',
      'bg-green-100 text-green-800',
      'bg-yellow-100 text-yellow-800',
      'bg-red-100 text-red-800',
      'bg-purple-100 text-purple-800',
      'bg-pink-100 text-pink-800',
      'bg-indigo-100 text-indigo-800'
    ];

    const hash = categoryId.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0);
    return colors[hash % colors.length];
  }

  getPriorityLabel(priority: number | undefined): string {
    if (priority === undefined) return 'Normal';
    if (priority >= 8) return 'Crítica';
    if (priority >= 5) return 'Alta';
    if (priority >= 3) return 'Media';
    return 'Baja';
  }

  getPriorityColor(priority: number | undefined): string {
    if (priority === undefined) return 'bg-gray-100 text-gray-700';
    if (priority >= 8) return 'bg-red-100 text-red-700';
    if (priority >= 5) return 'bg-orange-100 text-orange-700';
    if (priority >= 3) return 'bg-yellow-100 text-yellow-700';
    return 'bg-green-100 text-green-700';
  }
}
