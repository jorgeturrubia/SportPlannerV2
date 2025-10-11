import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MarketplaceItem } from '../../models/marketplace.models';
import { MarketplaceApiService } from '../../services/marketplace-api.service';
import { MarketplaceStateService } from '../../services/marketplace-state.service';
import { NotificationService } from '../../../../shared/notifications/notification.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-marketplace-detail-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './marketplace-detail-page.component.html',
})
export class MarketplaceDetailPageComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly apiService = inject(MarketplaceApiService);
  private readonly stateService = inject(MarketplaceStateService);
  private readonly notificationService = inject(NotificationService);

  private readonly itemState = signal<{
    item: MarketplaceItem | null;
    loading: boolean;
    error: string | null;
  }>({ item: null, loading: true, error: null });

  // Public selectors
  item = computed(() => this.itemState().item);
  loading = computed(() => this.itemState().loading);
  error = computed(() => this.itemState().error);
  isDownloading = signal(false);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadItem(id);
    } else {
      this.itemState.set({ item: null, loading: false, error: 'No item ID provided.' });
    }
  }

  async loadItem(id: string): Promise<void> {
    this.itemState.set({ item: null, loading: true, error: null });
    try {
      const item = await firstValueFrom(this.apiService.getMarketplaceItemById(id));
      this.itemState.set({ item, loading: false, error: null });
    } catch (e) {
      const message = e instanceof Error ? e.message : 'Failed to load item details.';
      this.itemState.set({ item: null, loading: false, error: message });
    }
  }

  async downloadItem(): Promise<void> {
    const currentItem = this.item();
    if (!currentItem) return;

    this.isDownloading.set(true);
    const result = await this.stateService.downloadItem(currentItem.id);

    if (result.success) {
      this.notificationService.success(`'${currentItem.name}' has been added to your library.`, 'Download Complete');
    } else {
      this.notificationService.error(result.error ?? 'An unknown error occurred.', 'Download Failed');
    }
    this.isDownloading.set(false);
  }
}