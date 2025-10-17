import { Component, Input, Output, EventEmitter, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ObjectiveDto } from '../../../features/dashboard/services/objectives.service';

export interface ObjectiveTreeItem extends ObjectiveDto {
  objectiveCategoryName?: string | null;
  objectiveSubcategoryName?: string | null;
}

@Component({
  selector: 'app-objective-tree',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './objective-tree.component.html',
  styleUrls: ['./objective-tree.component.css']
})
export class ObjectiveTreeComponent {
  // store objectives as a signal so computed() can track changes
  private readonly _objectives = signal<ObjectiveTreeItem[]>([]);
  @Input()
  get objectives(): ObjectiveTreeItem[] {
    return this._objectives();
  }
  set objectives(v: ObjectiveTreeItem[]) {
    this._objectives.set(v || []);
    // when objective list changes, re-evaluate expansion
    this.updateExpandedFromSelection();
  }
  
  private _selectedIds: string[] = [];
  private _hasUserInteracted = false; // Flag para prevenir auto-expand después de user interaction
  
  @Input()
  get selectedIds(): string[] {
    return this._selectedIds;
  }
  set selectedIds(v: string[]) {
    this._selectedIds = v || [];
    
    // Solo auto-expandir si el usuario no ha interactuado aún
    if (!this._hasUserInteracted) {
      this.updateExpandedFromSelection();
    }
    
    // emit selection change for consumers
    this.selectionChange.emit(this._selectedIds.slice());
  }
  @Input() addingMode = false; // if true show remove buttons

  @Output() add = new EventEmitter<ObjectiveDto>();
  @Output() remove = new EventEmitter<ObjectiveDto>();
  @Output() selectionChange = new EventEmitter<string[]>();

  // internal expanded state per category/subcategory
  expandedCategories = signal<Record<string, boolean>>({});
  expandedSubcategories = signal<Record<string, boolean>>({});

  grouped = computed(() => {
    const map = new Map<string, Map<string, ObjectiveTreeItem[]>>();

    for (const o of this._objectives() || []) {
      const cat = o.objectiveCategoryName || o.objectiveCategoryId || 'Uncategorized';
      const sub = o.objectiveSubcategoryName || o.objectiveSubcategoryId || 'General';

      if (!map.has(cat)) map.set(cat, new Map());
      const subMap = map.get(cat)!;
      if (!subMap.has(sub)) subMap.set(sub, []);
      subMap.get(sub)!.push(o);
    }

    // sort objectives inside groups by level desc then name
    for (const [cat, subMap] of map) {
      for (const [sub, list] of subMap) {
        list.sort((a, b) => (b.level || 1) - (a.level || 1) || a.name.localeCompare(b.name));
      }
    }

    return map;
  });

  // Expose a computed array of pairs for easier iteration in template
  groupedPairs = computed(() => {
    const pairs: Array<[string, Map<string, ObjectiveTreeItem[]>]> = [];
    for (const [cat, sub] of this.grouped()) {
      pairs.push([cat, sub]);
    }
    return pairs;
  });

  // helper to compute total count for a subMap
  subMapTotal(subMap: Map<string, ObjectiveTreeItem[]>): number {
    let acc = 0;
    for (const arr of Array.from(subMap.values())) acc += arr.length;
    return acc;
  }

  private updateExpandedFromSelection() {
    const selected = new Set(this._selectedIds || []);
    if (selected.size === 0) {
      return;
    }

    // Expand categories/subcategories that contain selected ids
    for (const [cat, subMap] of this.grouped()) {
      let catHas = false;
      for (const [sub, list] of subMap) {
        const match = list.some(o => selected.has(o.id));
        if (match) {
          catHas = true;
          // expand sub
          const key = `${cat}::${sub}`;
          this.expandedSubcategories.update(s => ({ ...s, [key]: true }));
        }
      }
      if (catHas) {
        this.expandedCategories.update(s => ({ ...s, [cat]: true }));
      }
    }

    // If selection is small, expand all to help discovery
    if (selected.size <= 5) {
      for (const [cat, subMap] of this.grouped()) {
        this.expandedCategories.update(s => ({ ...s, [cat]: true }));
        for (const [sub] of subMap) {
          const key = `${cat}::${sub}`;
          this.expandedSubcategories.update(s => ({ ...s, [key]: true }));
        }
      }
    }
  }

  isSelected(id: string) {
    return this.selectedIds.includes(id);
  }

  toggleCategory(cat: string) {
    this._hasUserInteracted = true; // Mark que el usuario ha interactuado
    
    const currentState = this.expandedCategories()[cat];
    const newState = currentState !== true;
    
    this.expandedCategories.update(s => {
      const updated = { ...s, [cat]: newState };
      return updated;
    });
  }

  toggleSubcategory(cat: string, sub: string) {
    const key = `${cat}::${sub}`;
    this._hasUserInteracted = true; // Mark que el usuario ha interactuado
    
    const currentState = this.expandedSubcategories()[key];
    const newState = currentState !== true;
    
    this.expandedSubcategories.update(s => {
      const updated = { ...s, [key]: newState };
      return updated;
    });
  }

  onAdd(o: ObjectiveDto) {
    this.add.emit(o);
  }

  onRemove(o: ObjectiveDto) {
    this.remove.emit(o);
  }
}
