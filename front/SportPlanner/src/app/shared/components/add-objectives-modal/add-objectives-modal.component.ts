import { Component, Input, Output, EventEmitter, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ObjectivesService, ObjectiveDto } from '../../../features/dashboard/services/objectives.service';
import { ObjectiveCategoriesService } from '../../../features/dashboard/services/objective-categories.service';
import { ObjectiveSubcategoriesService } from '../../../features/dashboard/services/objective-subcategories.service';
import { TrainingPlansService } from '../../../features/dashboard/services/training-plans.service';

@Component({
  selector: 'app-add-objectives-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './add-objectives-modal.component.html',
  styleUrls: ['./add-objectives-modal.component.css']
})
export class AddObjectivesModalComponent {
  private objectivesService = inject(ObjectivesService);
  private categoriesService = inject(ObjectiveCategoriesService);
  private subcategoriesService = inject(ObjectiveSubcategoriesService);
  private plansService = inject(TrainingPlansService);

  @Input() isOpen = false;
  @Input() planId: string | null = null;
  @Input() viewOnly = false;
  @Output() close = new EventEmitter<void>();
  @Output() added = new EventEmitter<string[]>();

  // UI state
  maximize = signal(false);
  searchTerm = signal('');
  categories = signal<any[]>([]);
  subcategories = signal<any[]>([]);
  selectedCategory = signal<string | null>(null);
  selectedSubcategory = signal<string | null>(null);
  objectives = signal<ObjectiveDto[]>([]);
  selected = signal<Record<string, boolean>>({});
  isLoading = signal(false);
  planHeader: { id?: string; name?: string; startDate?: string; endDate?: string } | null = null;

  async loadMasterData(): Promise<void> {
    try {
      const cats = await this.categoriesService.getCategories() as any[];
      this.categories.set(cats || []);
    } catch (err) {
      console.error('Failed to load categories', err);
    }
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
      if (this.viewOnly && this.planId) {
        // Load plan objectives summary
        try {
          const plan = await this.plansService.getPlan(this.planId);
          // set header info
          this.planHeader = {
            id: plan.id,
            name: plan.name,
            startDate: plan.startDate,
            endDate: plan.endDate
          };
          // Map plan objectives to minimal display objects
          const mapped = (plan.objectives || []).map(o => ({ id: o.objectiveId, name: o.objectiveName, description: '' } as ObjectiveDto));
          this.objectives.set(mapped);
          return;
        } catch (err) {
          console.error('Failed to load plan for view mode', err);
        } finally {
          this.isLoading.set(false);
        }
      }

      const all = await this.objectivesService.getObjectives() as ObjectiveDto[];
      let filtered: ObjectiveDto[] = all || [];

      const cat = this.selectedCategory();
      const sub = this.selectedSubcategory();
      const q = this.searchTerm().toLowerCase();

  if (cat) filtered = filtered.filter((o: ObjectiveDto) => o.objectiveCategoryId === cat);
  if (sub) filtered = filtered.filter((o: ObjectiveDto) => o.objectiveSubcategoryId === sub);
  if (q) filtered = filtered.filter((o: ObjectiveDto) => (o.name + ' ' + o.description).toLowerCase().includes(q));

      this.objectives.set(filtered);
    } catch (err) {
      console.error('Failed to load objectives', err);
    } finally {
      this.isLoading.set(false);
    }
  }

  async ngOnInit(): Promise<void> {
    await this.loadMasterData();
    await this.loadObjectives();
  }

  async onCategoryChange(catId: string | null) {
    this.selectedCategory.set(catId);
    this.selectedSubcategory.set(null);
    await this.loadSubcategoriesFor(catId || undefined);
    await this.loadObjectives();
  }

  async onSubcategoryChange(subId: string | null) {
    this.selectedSubcategory.set(subId);
    await this.loadObjectives();
  }

  toggleSelect(id: string) {
    this.selected.update(s => ({ ...s, [id]: !s[id] }));
  }

  async addSelected(): Promise<void> {
    const planId = this.planId;
    if (!planId) return;
    const selectedIds = Object.entries(this.selected()).filter(([k, v]) => v).map(([k]) => k);
    for (const id of selectedIds) {
      try {
        await this.plansService.addObjectiveToPlan(planId, { objectiveId: id, priority: 0, targetSessions: 0 });
      } catch (err) {
        console.error('Failed to add objective to plan', err);
      }
    }
    this.added.emit(selectedIds);
    this.close.emit();
  }

  onClose(): void {
    this.close.emit();
  }

  toggleMaximize(): void {
    this.maximize.update(s => !s);
  }
}
