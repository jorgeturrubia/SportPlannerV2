import { Component, Input, Output, EventEmitter, signal, inject, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ObjectivesService, ObjectiveDto } from '../../../features/dashboard/services/objectives.service';
import { ObjectiveCategoriesService } from '../../../features/dashboard/services/objective-categories.service';
import { ObjectiveSubcategoriesService } from '../../../features/dashboard/services/objective-subcategories.service';

@Component({
  selector: 'app-objective-selector',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './objective-selector.component.html',
  styleUrls: ['./objective-selector.component.css']
})
export class ObjectiveSelectorComponent {
  private objectivesService = inject(ObjectivesService);
  private categoriesService = inject(ObjectiveCategoriesService);
  private subcategoriesService = inject(ObjectiveSubcategoriesService);

  @Input() initialSelected: string[] = [];
  @Input() maxSelections: number | null = null;
  @Output() selectionChange = new EventEmitter<string[]>();

  // UI state
  searchTerm = signal('');
  categories = signal<any[]>([]);
  subcategories = signal<any[]>([]);
  selectedCategory = signal<string | null>(null);
  selectedSubcategory = signal<string | null>(null);
  objectives = signal<ObjectiveDto[]>([]);
  selectedObjectives = signal<string[]>([]);
  isLoading = signal(false);
  categoryColors = new Map<string, string>();

  constructor() {
    // Load initial data
    this.loadMasterData();
    this.loadObjectives();

    // Load objectives when filters change
    effect(() => {
      this.selectedCategory();
      this.selectedSubcategory();
      this.searchTerm();
      this.loadObjectives();
    });

    // Emit changes
    effect(() => {
      this.selectionChange.emit(this.selectedObjectives());
    });
  }

  ngOnInit() {
    if (this.initialSelected && this.initialSelected.length > 0) {
      this.selectedObjectives.set(this.initialSelected);
    }
  }

  async loadMasterData(): Promise<void> {
    try {
      const cats = await this.categoriesService.getCategories() as any[];
      this.categories.set(cats || []);
      // Asignar colores a categorías
      this.assignCategoryColors(cats);
    } catch (err) {
      console.error('Failed to load categories', err);
    }
  }

  private assignCategoryColors(categories: any[]): void {
    const colors = ['bg-blue-100 dark:bg-blue-900', 'bg-green-100 dark:bg-green-900', 'bg-purple-100 dark:bg-purple-900', 'bg-amber-100 dark:bg-amber-900'];
    categories.forEach((cat, idx) => {
      this.categoryColors.set(cat.id, colors[idx % colors.length]);
    });
  }

  async loadSubcategoriesFor(categoryId?: string): Promise<void> {
    try {
      const subs = await this.subcategoriesService.getSubcategories(categoryId) as any[];
      this.subcategories.set(subs || []);
    } catch (err) {
      console.error('Failed to load subcategories', err);
    }
  }

  async loadObjectives(): Promise<void> {
    try {
      this.isLoading.set(true);
      const all = await this.objectivesService.getObjectives() as ObjectiveDto[];
      let filtered: ObjectiveDto[] = all || [];

      const cat = this.selectedCategory();
      const sub = this.selectedSubcategory();
      const q = this.searchTerm().toLowerCase();

      if (cat) {
        filtered = filtered.filter((o: ObjectiveDto) => o.objectiveCategoryId === cat);
        // Load subcategories for this category
        await this.loadSubcategoriesFor(cat);
      }

      if (sub) filtered = filtered.filter((o: ObjectiveDto) => o.objectiveSubcategoryId === sub);
      if (q) filtered = filtered.filter((o: ObjectiveDto) => (o.name + ' ' + o.description).toLowerCase().includes(q));

      this.objectives.set(filtered);
    } catch (err) {
      console.error('Failed to load objectives', err);
    } finally {
      this.isLoading.set(false);
    }
  }

  onCategoryChange(categoryId: string | null): void {
    this.selectedCategory.set(categoryId);
    this.selectedSubcategory.set(null);
  }

  onSubcategoryChange(subcategoryId: string | null): void {
    this.selectedSubcategory.set(subcategoryId);
  }

  toggleObjective(objectiveId: string): void {
    const selected = this.selectedObjectives();
    const isSelected = selected.includes(objectiveId);

    if (isSelected) {
      // Deselect
      this.selectedObjectives.set(selected.filter(id => id !== objectiveId));
    } else {
      // Check max selections
      if (this.maxSelections && selected.length >= this.maxSelections) {
        return;
      }
      // Select
      this.selectedObjectives.set([...selected, objectiveId]);
    }
  }

  removeObjective(objectiveId: string): void {
    this.selectedObjectives.set(this.selectedObjectives().filter(id => id !== objectiveId));
  }

  getObjectiveName(objectiveId: string): string {
    const objective = this.objectives().find(o => o.id === objectiveId);
    return objective?.name || 'Unknown';
  }

  getObjectiveCategory(objectiveId: string): string {
    const objective = this.objectives().find(o => o.id === objectiveId);
    return objective?.objectiveCategoryId || '';
  }

  getCategoryColor(categoryId: string): string {
    return this.categoryColors.get(categoryId) || 'bg-gray-100 dark:bg-gray-900';
  }

  getSelectedObjectivesWithCategory() {
    return this.selectedObjectives().map(id => ({
      id,
      name: this.getObjectiveName(id),
      category: this.getObjectiveCategory(id)
    }));
  }

  isObjectiveSelected(objectiveId: string): boolean {
    return this.selectedObjectives().includes(objectiveId);
  }
}
