import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MarketplaceItem } from '../../models/marketplace.models';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-marketplace-item-card',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './marketplace-item-card.component.html',
})
export class MarketplaceItemCardComponent {
  item = input.required<MarketplaceItem>();
}