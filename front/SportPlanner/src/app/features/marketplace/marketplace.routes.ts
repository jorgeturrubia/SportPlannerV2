import { Route } from '@angular/router';

export const MARKETPLACE_ROUTES: Route[] = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'items',
  },
  {
    path: 'items',
    loadComponent: () =>
      import('./pages/marketplace-list-page/marketplace-list-page.component').then(
        (c) => c.MarketplaceListPageComponent
      ),
  },
  {
    path: 'items/:id',
    loadComponent: () =>
      import('./pages/marketplace-detail-page/marketplace-detail-page.component').then(
        (c) => c.MarketplaceDetailPageComponent
      ),
  },
  {
    path: 'itineraries',
    loadComponent: () =>
      import('./pages/itinerary-list-page/itinerary-list-page.component').then(
        (c) => c.ItineraryListPageComponent
      ),
  },
];