import { Component, Input, Output, EventEmitter, signal, Signal, computed, inject, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ObjectivesService, ObjectiveDto } from '../../services/objectives.service';
import { TrainingPlansService } from '../../services/training-plans.service';

interface TreeNode {
  name: string;
  children?: TreeNode[];
  objective?: ObjectiveDto;
}

interface CategoryChip {
  id: string;
  name: string;
  objectiveCount: number;
}

interface SubcategoryChip {
  id: string;
  name: string;
  objectiveCount: number;
}

type FilterLevel = 'categories' | 'subcategories';

@Component({
  selector: 'app-objective-selector',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './objective-selector.html',
})
export class ObjectiveSelectorComponent implements OnInit {
  private objectivesService = inject(ObjectivesService);
  private trainingPlansService = inject(TrainingPlansService);

  @Input() set isOpen(value: boolean | Signal<boolean>) {
    const val = typeof value === 'boolean' ? value : value();
    this.isOpenSignal.set(val);
  }
  private isOpenSignal = signal(false);

  @Input() planId: string | null = null;

  @Input() set initialObjectives(value: any[]) {
    console.log('🔄 initialObjectives setter called with:', value?.length ?? 0, 'items');
    this.initialObjectivesSignal.set(value || []);
  }
  private initialObjectivesSignal = signal<any[]>([]);

  @Input() set viewOnly(value: boolean | Signal<boolean>) {
    const val = typeof value === 'boolean' ? value : value();
    this.viewOnlySignal.set(val);
  }
  private viewOnlySignal = signal(false);

  @Output() close = new EventEmitter<void>();
  @Output() objectivesChanged = new EventEmitter<string[]>();

  isLoading = signal(false);
  isSaving = signal(false);
  
  allObjectives = signal<ObjectiveDto[]>([]);
  selectedObjectives = signal<ObjectiveDto[]>([]);
  originalSelectedObjectives = signal<ObjectiveDto[]>([]); // Track original selection

  // Filtrado dinámico
  filterLevel = signal<FilterLevel>('categories');
  selectedCategoryId = signal<string | null>(null);
  selectedSubcategoryId = signal<string | null>(null);

  // Expose signals for template
  isOpenTemplate = this.isOpenSignal;
  viewOnlyTemplate = this.viewOnlySignal;

  // Effect: when initialObjectives changes AND allObjectives loaded, map them
  private initObjectivesEffect = effect(() => {
    const initialObjs = this.initialObjectivesSignal();
    const allObjs = this.allObjectives();

    console.log('🔄 Effect triggered: initialObjectivesSignal.length =', initialObjs.length, 'allObjectives.length =', allObjs.length);

    if (initialObjs.length > 0 && allObjs.length > 0) {
      console.log('✅ Effect: Processing initialObjectives with loaded allObjectives');
      
      const selected = initialObjs
        .map((po: any) => {
          console.log('🔍 Looking for objective ID:', po.objectiveId);
          const found = allObjs.find(o => o.id === po.objectiveId);
          console.log('   Found:', found?.name ?? 'NOT FOUND', 'ID:', po.objectiveId);
          return found;
        })
        .filter((o): o is ObjectiveDto => o !== undefined);
      
      console.log('✅ Selected objectives after effect:', selected.length);
      this.selectedObjectives.set(selected);
      this.originalSelectedObjectives.set(selected);
    }
  });

  // Obtener todas las categorías únicas
  allCategories = computed(() => {
    const categories = new Map<string, CategoryChip>();
    const selected = new Set(this.selectedObjectives().map(o => o.id));

    for (const obj of this.allObjectives()) {
      if (!selected.has(obj.id)) {
        const catId = obj.objectiveCategoryId;
        const catName = obj.objectiveCategoryName || catId;
        
        if (!categories.has(catId)) {
          categories.set(catId, { id: catId, name: catName, objectiveCount: 0 });
        }
        categories.get(catId)!.objectiveCount++;
      }
    }

    return Array.from(categories.values()).sort((a, b) => a.name.localeCompare(b.name));
  });

  // Obtener subcategorías de la categoría seleccionada
  subcategoriesForSelectedCategory = computed(() => {
    const categoryId = this.selectedCategoryId();
    if (!categoryId) return [];

    const subcategories = new Map<string, SubcategoryChip>();
    const selected = new Set(this.selectedObjectives().map(o => o.id));

    for (const obj of this.allObjectives()) {
      if (!selected.has(obj.id) && obj.objectiveCategoryId === categoryId) {
        const subcatId = obj.objectiveSubcategoryId || 'sin-subcategoria';
        const subcatName = obj.objectiveSubcategoryName || 'Sin Subcategoría';
        
        if (!subcategories.has(subcatId)) {
          subcategories.set(subcatId, { id: subcatId, name: subcatName, objectiveCount: 0 });
        }
        subcategories.get(subcatId)!.objectiveCount++;
      }
    }

    return Array.from(subcategories.values()).sort((a, b) => a.name.localeCompare(b.name));
  });

  // Objetivos disponibles filtrados
  availableObjectives = computed(() => {
    const selected = new Set(this.selectedObjectives().map(o => o.id));
    const categoryId = this.selectedCategoryId();
    const subcategoryId = this.selectedSubcategoryId();

    let filtered = this.allObjectives().filter(obj => !selected.has(obj.id));

    if (categoryId) {
      filtered = filtered.filter(obj => obj.objectiveCategoryId === categoryId);
    }

    if (subcategoryId && subcategoryId !== 'sin-subcategoria') {
      filtered = filtered.filter(obj => obj.objectiveSubcategoryId === subcategoryId);
    } else if (subcategoryId === 'sin-subcategoria') {
      filtered = filtered.filter(obj => !obj.objectiveSubcategoryId);
    }

    return filtered;
  });

  selectedObjectivesTree = computed(() => {
    const tree: TreeNode[] = [];
    const categoryMap = new Map<string, TreeNode>();

    for (const objective of this.selectedObjectives()) {
      const categoryId = objective.objectiveCategoryId;
      const categoryName = objective.objectiveCategoryName || categoryId;

      if (!categoryMap.has(categoryId)) {
        const categoryNode: TreeNode = { name: categoryName, children: [] };
        categoryMap.set(categoryId, categoryNode);
        tree.push(categoryNode);
      }

      const categoryNode = categoryMap.get(categoryId)!;
      
      const subcategoryId = objective.objectiveSubcategoryId || 'Sin Subcategoría';
      const subcategoryName = !objective.objectiveSubcategoryId 
        ? 'Sin Subcategoría' 
        : (objective.objectiveSubcategoryName || subcategoryId);

      let subcategoryNode = categoryNode.children!.find(n => n.name === subcategoryName);
      if (!subcategoryNode) {
        subcategoryNode = { name: subcategoryName, children: [] };
        categoryNode.children!.push(subcategoryNode);
      }

      subcategoryNode.children!.push({ name: objective.name, objective: objective });
    }

    return tree;
  });

  async ngOnInit(): Promise<void> {
    await this.loadData();
  }

  private async loadData(): Promise<void> {
    this.isLoading.set(true);
    try {
      // Load objectives first
      const objectives = await this.objectivesService.getObjectives();
      this.allObjectives.set(objectives || []);
      console.log('✅ Objectives loaded:', objectives?.length ?? 0);

      // NOTE: Processing of initialObjectives is now handled by the effect
      // when both initialObjectivesSignal and allObjectives are ready
      
    } catch (err) {
      console.error('Failed to load objectives:', err);
    } finally {
      this.isLoading.set(false);
    }
  }

  closeModal(): void {
    this.close.emit();
  }

  selectCategory(categoryId: string): void {
    this.selectedCategoryId.set(categoryId);
    this.selectedSubcategoryId.set(null);
    this.filterLevel.set('subcategories');
  }

  selectSubcategory(subcategoryId: string): void {
    this.selectedSubcategoryId.set(subcategoryId);
  }

  backToCategories(): void {
    this.selectedCategoryId.set(null);
    this.selectedSubcategoryId.set(null);
    this.filterLevel.set('categories');
  }

  backToSubcategories(): void {
    this.selectedSubcategoryId.set(null);
  }

  selectObjective(objective: ObjectiveDto): void {
    if (this.viewOnlySignal()) return;

    const isSelected = this.selectedObjectives().some(o => o.id === objective.id);
    if (isSelected) {
      this.selectedObjectives.update(list => list.filter(o => o.id !== objective.id));
    } else {
      this.selectedObjectives.update(list => [...list, objective]);
    }
  }

  isSelected(objective: ObjectiveDto): boolean {
    return this.selectedObjectives().some(o => o.id === objective.id);
  }

  getSelectedCategoryName(): string {
    const categoryId = this.selectedCategoryId();
    if (!categoryId) return '';
    const category = this.allCategories().find(c => c.id === categoryId);
    return category?.name || '';
  }

  getSelectedSubcategoryName(): string {
    const subcategoryId = this.selectedSubcategoryId();
    if (!subcategoryId) return '';
    const subcategory = this.subcategoriesForSelectedCategory().find(s => s.id === subcategoryId);
    return subcategory?.name || '';
  }

  async saveObjectives(): Promise<void> {
    if (this.viewOnlySignal()) {
      this.closeModal();
      return;
    }

    this.isSaving.set(true);
    try {
      // Get ALL selected objective IDs (complete final list)
      const allSelectedIds = this.selectedObjectives().map(o => o.id);

      console.log('📊 Final objective list for plan:');
      console.log('   Total selected:', allSelectedIds.length);
      console.log('   Objective IDs:', allSelectedIds);

      // Emit the COMPLETE list of objectives to keep in the plan
      this.objectivesChanged.emit(allSelectedIds);
      this.closeModal();
    } finally {
      this.isSaving.set(false);
    }
  }
}
