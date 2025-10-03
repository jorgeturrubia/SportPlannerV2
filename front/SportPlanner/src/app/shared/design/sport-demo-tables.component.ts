import { Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Componente de demostración con múltiples diseños innovadores de visualización
 * Incluye: Grid, Board, Carousel 3D, Timeline, Hexagon, DataTable, Galaxy
 *
 * @example
 * <app-sport-demo-tables
 *   [items]="teams()"
 *   [viewMode]="'galaxy'"
 *   (onEdit)="handleEdit($event)"
 *   (onDelete)="handleDelete($event)"
 *   (onSelect)="handleSelect($event)">
 * </app-sport-demo-tables>
 */
@Component({
  selector: 'app-sport-demo-tables',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sport-demo-tables.component.html',
  styleUrls: ['./sport-demo-tables.component.css']
})
export class SportDemoTablesComponent {
  // Inputs
  @Input() items: any[] = [];
  @Input() viewMode: 'grid' | 'board' | 'carousel' | 'timeline' | 'hexagon' | 'datatable' | 'galaxy' = 'grid';
  @Input() maxCapacity = 15;
  @Input() showCreateButton = true;

  // Outputs
  @Output() onEdit = new EventEmitter<any>();
  @Output() onDelete = new EventEmitter<any>();
  @Output() onCreate = new EventEmitter<void>();
  @Output() onSelect = new EventEmitter<any>();
  @Output() viewModeChange = new EventEmitter<string>();

  // Estado interno
  Math = Math;
  expandedRows = signal<Set<string>>(new Set());
  sortColumn = signal<string>('name');
  sortDirection = signal<'asc' | 'desc'>('asc');
  searchTerm = signal<string>('');
  selectedPlanet = signal<any | null>(null);

  // Métodos para DataTable
  toggleRow(itemId: string) {
    const expanded = new Set(this.expandedRows());
    if (expanded.has(itemId)) {
      expanded.delete(itemId);
    } else {
      expanded.add(itemId);
    }
    this.expandedRows.set(expanded);
  }

  isRowExpanded(itemId: string): boolean {
    return this.expandedRows().has(itemId);
  }

  sortBy(column: string) {
    if (this.sortColumn() === column) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(column);
      this.sortDirection.set('asc');
    }
  }

  get filteredAndSortedItems() {
    let filtered = this.items;

    const search = this.searchTerm().toLowerCase();
    if (search) {
      filtered = filtered.filter(t =>
        t.name.toLowerCase().includes(search) ||
        t.description?.toLowerCase().includes(search)
      );
    }

    const col = this.sortColumn();
    const dir = this.sortDirection();

    return filtered.sort((a, b) => {
      let aVal = a[col];
      let bVal = b[col];

      if (col === 'members') {
        aVal = a.members.length;
        bVal = b.members.length;
      } else if (col === 'createdAt') {
        aVal = new Date(a.createdAt).getTime();
        bVal = new Date(b.createdAt).getTime();
      }

      if (aVal < bVal) return dir === 'asc' ? -1 : 1;
      if (aVal > bVal) return dir === 'asc' ? 1 : -1;
      return 0;
    });
  }

  getCapacityStatus(item: any): 'low' | 'medium' | 'high' | 'full' {
    const percentage = (item.members.length / this.maxCapacity) * 100;
    if (percentage >= 100) return 'full';
    if (percentage >= 75) return 'high';
    if (percentage >= 40) return 'medium';
    return 'low';
  }

  // Métodos para Galaxy
  selectPlanet(item: any) {
    this.selectedPlanet.set(item);
    this.onSelect.emit(item);
  }

  closePlanetDetail() {
    this.selectedPlanet.set(null);
  }

  getPlanetSize(item: any): number {
    const percentage = item.members.length / this.maxCapacity;
    return 60 + (percentage * 80);
  }

  getOrbitRadius(index: number, total: number): number {
    const baseRadius = 180;
    const orbitSpacing = 100;
    const orbitLevel = Math.floor(index / 3);
    return baseRadius + (orbitLevel * orbitSpacing);
  }

  getOrbitSpeed(index: number): number {
    return 20 + (index * 3);
  }

  // Métodos de eventos
  handleEdit(item: any) {
    this.onEdit.emit(item);
  }

  handleDelete(item: any) {
    this.onDelete.emit(item);
  }

  handleCreate() {
    this.onCreate.emit();
  }

  toggleViewMode(mode: 'grid' | 'board' | 'carousel' | 'timeline' | 'hexagon' | 'datatable' | 'galaxy') {
    this.viewMode = mode;
    this.viewModeChange.emit(mode);
  }

  avatarUrl(name: unknown) {
    const s = typeof name === 'string' ? name : String(name ?? '');
    return `https://ui-avatars.com/api/?name=${encodeURIComponent(s)}&background=ffffff&color=000`;
  }
}
