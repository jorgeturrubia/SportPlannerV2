import { Component, input, output, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { TableConfig, TableColumn, TableAction } from '../../models/table-config';

@Component({
  selector: 'app-data-table',
  imports: [CommonModule],
  templateUrl: './data-table.html',
  styleUrl: './data-table.css'
})
export class DataTable<T = any> {
  constructor(private sanitizer: DomSanitizer) {}
  // Inputs
  data = input.required<T[]>();
  config = input.required<TableConfig<T>>();
  title = input<string>('');
  subtitle = input<string>('');

  // Outputs
  rowClick = output<T>();
  actionClick = output<{ action: TableAction<T>, item: T }>();

  // Internal state
  pageSize = signal(10);
  pageIndex = signal(0);
  searchTerm = signal('');

  // Computed values
  filteredData = computed(() => {
    const search = this.searchTerm().toLowerCase();
    if (!search) return this.data();

    return this.data().filter(item =>
      Object.values(item as object).some(value =>
        String(value).toLowerCase().includes(search)
      )
    );
  });

  paginatedData = computed(() => {
    const start = this.pageIndex() * this.pageSize();
    const end = start + this.pageSize();
    return this.filteredData().slice(start, end);
  });

  totalPages = computed(() =>
    Math.ceil(this.filteredData().length / this.pageSize())
  );

  totalItems = computed(() => this.filteredData().length);

  pageSizeOptions = computed(() =>
    this.config()?.pageSizeOptions || [5, 10, 25, 50, 100]
  );

  // Methods
  onPageChange(pageIndex: number): void {
    this.pageIndex.set(pageIndex);
  }

  onPageSizeChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    this.pageSize.set(Number(select.value));
    this.pageIndex.set(0);
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm.set(input.value);
    this.pageIndex.set(0);
  }

  onRowClick(item: T): void {
    this.rowClick.emit(item);
  }

  onActionClick(action: TableAction<T>, item: T): void {
    this.actionClick.emit({ action, item });
  }

  getCellValue(item: T, column: TableColumn<T>): any {
    return (item as any)[column.key];
  }

  getBadgeClasses(value: any, column: TableColumn<T>): string {
    if (!column.badgeConfig) return '';
    return column.badgeConfig.colorMap[value] || '';
  }

  getSafeHtml(html: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(html);
  }
}
