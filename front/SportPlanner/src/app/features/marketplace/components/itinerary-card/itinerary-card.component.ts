import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Itinerary } from '../../models/marketplace.models';

@Component({
  selector: 'app-itinerary-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './itinerary-card.component.html',
})
export class ItineraryCardComponent {
  itinerary = input.required<Itinerary>();
}