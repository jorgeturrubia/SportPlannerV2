import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-entity-page-layout',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './entity-page-layout.component.html',
  styleUrls: ['./entity-page-layout.component.css']
})
export class EntityPageLayoutComponent {
  // Header configuration
  title = input.required<string>();
  subtitle = input<string>();

  // Action button configuration
  addButtonText = input<string>();
  addButtonIcon = input<string>('M12 6v6m0 0v6m0-6h6m-6 0H6'); // Default: plus icon

  // States
  error = input<string | null>(null);
  isLoading = input<boolean>(false);

  // Events
  add = output<void>();

  onAdd(): void {
    this.add.emit();
  }
}
