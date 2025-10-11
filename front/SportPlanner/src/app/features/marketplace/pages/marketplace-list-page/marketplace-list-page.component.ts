import { Component, effect, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MarketplaceStateService } from '../../services/marketplace-state.service';
import { MarketplaceItemCardComponent } from '../../components/marketplace-item-card/marketplace-item-card.component';
import { MarketplaceFilter, MarketplaceItemType, Sport } from '../../models/marketplace.models';
import { FormsModule } from '@angular/forms';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'app-marketplace-list-page',
  standalone: true,
  imports: [CommonModule, FormsModule, MarketplaceItemCardComponent],
  templateUrl: './marketplace-list-page.component.html',
})
export class MarketplaceListPageComponent implements OnInit {
  private readonly stateService = inject(MarketplaceStateService);
  private readonly searchSubject = new Subject<string>();

  // Expose state signals to the template
  items = this.stateService.items;
  loading = this.stateService.loading;
  error = this.stateService.error;
  pagination = this.stateService.pagination;

  // Local signals for filter controls
  searchTerm = signal<string>('');
  selectedSport = signal<Sport>(Sport.General);
  selectedType = signal<MarketplaceItemType | undefined>(undefined);
  selectedFilter = signal<MarketplaceFilter>(MarketplaceFilter.MostPopular);

  // For template access to enums
  Sports = Object.values(Sport);
  ItemTypes = Object.values(MarketplaceItemType);
  Filters = Object.values(MarketplaceFilter);

  constructor() {
    // Trigger search when any filter signal changes
    effect(() => {
      this.triggerSearch();
    });

    this.searchSubject.pipe(debounceTime(300)).subscribe(term => {
      this.searchTerm.set(term);
    });
  }

  ngOnInit(): void {
    // Initial search on component load
    this.triggerSearch();
  }

  onSearchTermChange(term: string): void {
    this.searchSubject.next(term);
  }

  triggerSearch(): void {
    this.stateService.searchMarketplace({
      sport: this.selectedSport(),
      type: this.selectedType(),
      filter: this.selectedFilter(),
      searchTerm: this.searchTerm(),
      page: 1, // Reset to page 1 on new search
    });
  }

  changePage(page: number): void {
    if (page < 1 || page > (this.pagination()?.totalPages ?? 1)) {
      return;
    }
    this.stateService.searchMarketplace({
      sport: this.selectedSport(),
      type: this.selectedType(),
      filter: this.selectedFilter(),
      searchTerm: this.searchTerm(),
      page: page,
    });
  }
}