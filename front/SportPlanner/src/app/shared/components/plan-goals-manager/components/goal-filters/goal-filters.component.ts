import { Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FilterState } from '../../plan-goals-manager.component';

@Component({
  selector: 'app-goal-filters',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './goal-filters.component.html',
  styleUrls: ['./goal-filters.component.css']
})
export class GoalFiltersComponent {
  @Input() categories: any[] = [];
  @Input() subcategories: any[] = [];
  @Input() filters!: FilterState;
  @Input() totalResults = 0;
  @Output() filtersChange = new EventEmitter<Partial<FilterState>>();
  @Output() categoryChange = new EventEmitter<string | null>();

  isExpanded = signal(true);

  onSearchChange(value: string): void {
    this.filtersChange.emit({ search: value });
  }

  onCategoryChange(value: string): void {
    const categoryId = value || null;
    this.categoryChange.emit(categoryId);
    this.filtersChange.emit({ categoryId, subcategoryId: null });
  }

  onSubcategoryChange(value: string): void {
    this.filtersChange.emit({ subcategoryId: value || null });
  }

  onSortChange(value: string): void {
    this.filtersChange.emit({ sortBy: value as any });
  }

  onViewModeChange(mode: 'grid' | 'list'): void {
    this.filtersChange.emit({ viewMode: mode });
  }

  clearFilters(): void {
    this.filtersChange.emit({
      search: '',
      categoryId: null,
      subcategoryId: null,
      sortBy: 'name'
    });
    this.categoryChange.emit(null);
  }

  toggleExpanded(): void {
    this.isExpanded.update(v => !v);
  }

  hasActiveFilters(): boolean {
    return !!(
      this.filters.search ||
      this.filters.categoryId ||
      this.filters.subcategoryId
    );
  }
}
