import { Component, Input, Output, EventEmitter, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

export interface TableColumn {
  key: string;
  label: string;
  sortable?: boolean;
  filterable?: boolean;
  type?: 'text' | 'number' | 'date' | 'boolean' | 'badge' | 'avatar';
  width?: string;
  align?: 'left' | 'center' | 'right';
  format?: (value: any) => string;
  cellClass?: (row: any) => string;
}

export interface TableAction {
  icon: string;
  label: string;
  color?: 'primary' | 'secondary' | 'success' | 'danger' | 'warning';
  onClick: (row: any) => void;
  show?: (row: any) => boolean;
}

export interface PaginationConfig {
  pageSize: number;
  pageSizeOptions: number[];
  showTotal?: boolean;
}

/**
 * Professional Reusable Data Table Component
 *
 * Features:
 * - Sortable columns
 * - Filterable data
 * - Pagination
 * - Sticky actions column
 * - Dark/Light theme
 * - Fully customizable
 *
 * @example
 * <app-professional-table
 *   [data]="myData"
 *   [columns]="columns"
 *   [actions]="actions"
 *   [pagination]="{ pageSize: 10, pageSizeOptions: [5, 10, 25, 50] }"
 *   (rowClick)="handleRowClick($event)">
 * </app-professional-table>
 */
@Component({
  selector: 'app-professional-table',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './professional-table.component.html',
  styleUrls: ['./professional-table.component.css']
})
export class ProfessionalTableComponent {
  // Inputs
  @Input() data: any[] = [];
  @Input() columns: TableColumn[] = [];
  @Input() actions: TableAction[] = [];
  @Input() pagination: PaginationConfig = { pageSize: 10, pageSizeOptions: [10, 25, 50] };
  @Input() showSearch = true;
  @Input() showFilters = true;
  @Input() stickyHeader = true;
  @Input() stickyActions = true;
  @Input() striped = true;
  @Input() hoverable = true;
  @Input() loading = false;
  @Input() emptyMessage = 'No hay datos disponibles';

  // Outputs
  @Output() rowClick = new EventEmitter<any>();
  @Output() selectionChange = new EventEmitter<any[]>();

  // State
  searchTerm = signal('');
  sortColumn = signal<string>('');
  sortDirection = signal<'asc' | 'desc'>('asc');
  currentPage = signal(1);
  pageSize = signal(10);
  selectedRows = signal<Set<any>>(new Set());
  columnFilters = signal<Map<string, string>>(new Map());

  // Computed
  filteredData = computed(() => {
    let filtered = [...this.data];

    // Global search
    const search = this.searchTerm().toLowerCase();
    if (search) {
      filtered = filtered.filter(row =>
        this.columns.some(col => {
          const value = this.getCellValue(row, col.key);
          return value?.toString().toLowerCase().includes(search);
        })
      );
    }

    // Column filters
    this.columnFilters().forEach((filterValue, columnKey) => {
      if (filterValue) {
        filtered = filtered.filter(row => {
          const value = this.getCellValue(row, columnKey);
          return value?.toString().toLowerCase().includes(filterValue.toLowerCase());
        });
      }
    });

    // Sorting
    const sortCol = this.sortColumn();
    if (sortCol) {
      const direction = this.sortDirection();
      filtered.sort((a, b) => {
        const aVal = this.getCellValue(a, sortCol);
        const bVal = this.getCellValue(b, sortCol);

        if (aVal < bVal) return direction === 'asc' ? -1 : 1;
        if (aVal > bVal) return direction === 'asc' ? 1 : -1;
        return 0;
      });
    }

    return filtered;
  });

  paginatedData = computed(() => {
    const start = (this.currentPage() - 1) * this.pageSize();
    const end = start + this.pageSize();
    return this.filteredData().slice(start, end);
  });

  totalPages = computed(() =>
    Math.ceil(this.filteredData().length / this.pageSize())
  );

  pageNumbers = computed(() => {
    const total = this.totalPages();
    const current = this.currentPage();
    const delta = 2;
    const range: number[] = [];

    for (let i = Math.max(2, current - delta); i <= Math.min(total - 1, current + delta); i++) {
      range.push(i);
    }

    if (current - delta > 2) {
      range.unshift(-1);
    }
    if (current + delta < total - 1) {
      range.push(-1);
    }

    range.unshift(1);
    if (total > 1) range.push(total);

    return range;
  });

  constructor() {
    this.pageSize.set(this.pagination.pageSize);
  }

  // Methods
  getCellValue(row: any, key: string): any {
    return key.split('.').reduce((obj, k) => obj?.[k], row);
  }

  formatCell(row: any, column: TableColumn): string {
    const value = this.getCellValue(row, column.key);

    if (column.format) {
      return column.format(value);
    }

    if (column.type === 'date' && value) {
      return new Date(value).toLocaleDateString();
    }

    if (column.type === 'boolean') {
      return value ? 'Sí' : 'No';
    }

    return value?.toString() || '-';
  }

  getCellClass(row: any, column: TableColumn): string {
    const baseClass = `text-${column.align || 'left'}`;
    return column.cellClass ? `${baseClass} ${column.cellClass(row)}` : baseClass;
  }

  toggleSort(column: TableColumn) {
    if (!column.sortable) return;

    if (this.sortColumn() === column.key) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(column.key);
      this.sortDirection.set('asc');
    }
  }

  setColumnFilter(columnKey: string, value: string) {
    const filters = new Map(this.columnFilters());
    if (value) {
      filters.set(columnKey, value);
    } else {
      filters.delete(columnKey);
    }
    this.columnFilters.set(filters);
    this.currentPage.set(1);
  }

  clearFilters() {
    this.searchTerm.set('');
    this.columnFilters.set(new Map());
    this.currentPage.set(1);
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
    }
  }

  changePageSize(size: number) {
    this.pageSize.set(size);
    this.currentPage.set(1);
  }

  toggleRowSelection(row: any) {
    const selected = new Set(this.selectedRows());
    if (selected.has(row)) {
      selected.delete(row);
    } else {
      selected.add(row);
    }
    this.selectedRows.set(selected);
    this.selectionChange.emit(Array.from(selected));
  }

  toggleAllRows() {
    const allSelected = this.paginatedData().every(row => this.selectedRows().has(row));
    const selected = new Set(this.selectedRows());

    if (allSelected) {
      this.paginatedData().forEach(row => selected.delete(row));
    } else {
      this.paginatedData().forEach(row => selected.add(row));
    }

    this.selectedRows.set(selected);
    this.selectionChange.emit(Array.from(selected));
  }

  isRowSelected(row: any): boolean {
    return this.selectedRows().has(row);
  }

  isAllSelected(): boolean {
    return this.paginatedData().length > 0 &&
           this.paginatedData().every(row => this.selectedRows().has(row));
  }

  onRowClick(row: any) {
    this.rowClick.emit(row);
  }

  shouldShowAction(action: TableAction, row: any): boolean {
    return action.show ? action.show(row) : true;
  }

  getActionClass(action: TableAction): string {
    const baseClass = 'action-btn';
    const colorClass = {
      'primary': 'action-btn-primary',
      'secondary': 'action-btn-secondary',
      'success': 'action-btn-success',
      'danger': 'action-btn-danger',
      'warning': 'action-btn-warning'
    }[action.color || 'secondary'];

    return `${baseClass} ${colorClass}`;
  }
}
