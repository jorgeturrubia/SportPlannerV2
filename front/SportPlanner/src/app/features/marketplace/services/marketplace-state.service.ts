import { computed, inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { MarketplaceApiService, MarketplaceSearchParams } from './marketplace-api.service';
import { Itinerary, MarketplaceItem, PagedResult } from '../models/marketplace.models';

export interface MarketplaceState {
  itemsResult: PagedResult<MarketplaceItem> | null;
  itineraries: Itinerary[];
  loading: boolean;
  error: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class MarketplaceStateService {
  private readonly apiService = inject(MarketplaceApiService);

  // Private state signal
  private readonly state = signal<MarketplaceState>({
    itemsResult: null,
    itineraries: [],
    loading: false,
    error: null,
  });

  // Public selectors from state
  readonly items = computed(() => this.state().itemsResult?.items ?? []);
  readonly pagination = computed(() => {
    const { items, ...pagination } = this.state().itemsResult ?? {};
    return pagination;
  });
  readonly itineraries = computed(() => this.state().itineraries);
  readonly loading = computed(() => this.state().loading);
  readonly error = computed(() => this.state().error);

  async searchMarketplace(params: MarketplaceSearchParams): Promise<void> {
    this.state.update((s) => ({ ...s, loading: true, error: null }));
    try {
      const result = await firstValueFrom(this.apiService.searchMarketplace(params));
      this.state.update((s) => ({ ...s, itemsResult: result, loading: false }));
    } catch (e) {
      const message = e instanceof Error ? e.message : 'An unknown error occurred.';
      this.state.update((s) => ({ ...s, error: message, loading: false }));
    }
  }

  async loadItineraries(): Promise<void> {
    this.state.update((s) => ({ ...s, loading: true, error: null }));
    try {
      const result = await firstValueFrom(this.apiService.getItineraries());
      this.state.update((s) => ({ ...s, itineraries: result, loading: false }));
    } catch (e) {
      const message = e instanceof Error ? e.message : 'An unknown error occurred.';
      this.state.update((s) => ({ ...s, error: message, loading: false }));
    }
  }

  async downloadItem(id: string): Promise<{ success: boolean; error?: string }> {
    try {
      await firstValueFrom(this.apiService.downloadFromMarketplace(id));
      // Optionally, we could update the download count on the specific item in the state here
      // For now, we just return success and let the component handle the notification.
      return { success: true };
    } catch (e) {
      const message = e instanceof Error ? e.message : 'Failed to download item.';
      return { success: false, error: message };
    }
  }
}