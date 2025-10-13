import { Component, signal, inject, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ObjectivesService, ObjectiveDto } from '../../../features/dashboard/services/objectives.service';

@Component({
  selector: 'app-objective-suggester',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './objective-suggester.component.html',
  styleUrls: ['./objective-suggester.component.css']
})
export class ObjectiveSuggesterComponent {
  private objectivesService = inject(ObjectivesService);

  // Inputs
  @Input() viewOnly: boolean = false;
  @Input() initialSelected: string[] = [];

  // Emits selected ids when changed
  @Output() selectionChange = new EventEmitter<string[]>();

  selectedIds = signal<string[]>([]);

  // Local state
  all = signal<ObjectiveDto[]>([]);
  suggestions = signal<ObjectiveDto[]>([]);
  query = signal('');
  isLoading = signal(false);

  constructor() {
    // nothing
  }

  async ngOnInit(): Promise<void> {
    await this.loadAll();
    // initialize selection
    if (this.initialSelected && this.initialSelected.length) {
      this.selectedIds.set(this.initialSelected.slice());
      this.selectionChange.emit(this.selectedIds());
    }
  }

  private async loadAll(): Promise<void> {
    this.isLoading.set(true);
    try {
      const data = await this.objectivesService.getObjectives();
      this.all.set(data || []);
      this.suggestions.set(data || []);
    } finally {
      this.isLoading.set(false);
    }
  }

  onQueryChange(value: string) {
    this.query.set(value || '');
    this.filterSuggestions();
  }

  private filterSuggestions() {
    const q = (this.query() || '').trim().toLowerCase();
    if (!q) {
      this.suggestions.set(this.all().slice(0, 50));
      return;
    }

    // split tokens and require each to appear in name or description or techniques
    const tokens = q.split(/\s+/).filter(t => t.length > 0);

    const results = this.all().filter(obj => {
      const hay = (obj.name + ' ' + (obj.description || '') + ' ' + (obj.techniques || []).map(t=>t.description).join(' ')).toLowerCase();
      return tokens.every(token => hay.indexOf(token) !== -1);
    });

    // limit results
    this.suggestions.set(results.slice(0, 100));
  }

  isSelected(id: string): boolean {
    return this.selectedIds().includes(id);
  }

  toggleSelect(obj: ObjectiveDto) {
    if (this.viewOnly) return;
    const ids = this.selectedIds().slice();
    const idx = ids.indexOf(obj.id);
    if (idx === -1) ids.push(obj.id);
    else ids.splice(idx, 1);
    this.selectedIds.set(ids);
    this.selectionChange.emit(this.selectedIds());
  }

  // helper for parent to get selection
  getSelected(): string[] {
    return this.selectedIds();
  }
}
