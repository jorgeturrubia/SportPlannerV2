import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MarketplaceStateService } from '../../services/marketplace-state.service';
import { ItineraryCardComponent } from '../../components/itinerary-card/itinerary-card.component';

@Component({
  selector: 'app-itinerary-list-page',
  standalone: true,
  imports: [CommonModule, ItineraryCardComponent],
  templateUrl: './itinerary-list-page.component.html',
})
export class ItineraryListPageComponent implements OnInit {
  private readonly stateService = inject(MarketplaceStateService);

  // Expose state signals to the template
  itineraries = this.stateService.itineraries;
  loading = this.stateService.loading;
  error = this.stateService.error;

  ngOnInit(): void {
    this.stateService.loadItineraries();
  }
}