import { Component, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

export interface TableColumn {
  key: string;
  label: string;
  sortable?: boolean;
  type?: 'text' | 'badge' | 'date' | 'number';
  formatter?: (value: any) => string;
}

export interface TableAction {
  icon: string;
  label: string;
  color: 'blue' | 'green' | 'red' | 'yellow';
  handler: (row: any) => void;
}

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.css']
})
export class DataTableComponent {
  // Inputs
  columns = input.required<TableColumn[]>();
  data = input.required<any[]>();
  actions = input<TableAction[]>([]);
  isLoading = input<boolean>(false);
  emptyMessage = input<string>('No data available');
  showSearch = input<boolean>(true);

  // Outputs
  rowClick = output<any>();
  add = output<void>();

  // State
  searchTerm = signal('');
  sortColumn = signal<string | null>(null);
  sortDirection = signal<'asc' | 'desc'>('asc');

  // Computed
  filteredData = () => {
    let result = this.data();

    // Apply search
    const search = this.searchTerm().toLowerCase();
    if (search) {
      result = result.filter(row =>
        this.columns().some(col => {
          const value = this.getCellValue(row, col);
          return value?.toString().toLowerCase().includes(search);
        })
      );
    }

    // Apply sorting
    const sortCol = this.sortColumn();
    if (sortCol) {
      result = [...result].sort((a, b) => {
        const aVal = a[sortCol];
        const bVal = b[sortCol];
        const direction = this.sortDirection() === 'asc' ? 1 : -1;

        if (aVal < bVal) return -1 * direction;
        if (aVal > bVal) return 1 * direction;
        return 0;
      });
    }

    return result;
  };

  // Methods
  getCellValue(row: any, column: TableColumn): any {
    const value = row[column.key];

    if (column.formatter) {
      return column.formatter(value);
    }

    if (column.type === 'date' && value) {
      return new Date(value).toLocaleDateString();
    }

    return value;
  }

  toggleSort(column: TableColumn): void {
    if (!column.sortable) return;

    if (this.sortColumn() === column.key) {
      this.sortDirection.update(dir => dir === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(column.key);
      this.sortDirection.set('asc');
    }
  }

  onRowClick(row: any): void {
    this.rowClick.emit(row);
  }

  onAdd(): void {
    this.add.emit();
  }

  getActionColor(color: string): string {
    const colors: Record<string, string> = {
      blue: 'text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300',
      green: 'text-green-600 hover:text-green-800 dark:text-green-400 dark:hover:text-green-300',
      red: 'text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-300',
      yellow: 'text-yellow-600 hover:text-yellow-800 dark:text-yellow-400 dark:hover:text-yellow-300'
    };
    return colors[color] || colors['blue'];
  }
}
