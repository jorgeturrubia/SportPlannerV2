import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Itinerary,
  MarketplaceFilter,
  MarketplaceItem,
  MarketplaceItemType,
  PagedResult,
  Sport,
} from '../models/marketplace.models';

export interface MarketplaceSearchParams {
  sport: Sport;
  type?: MarketplaceItemType;
  filter?: MarketplaceFilter;
  searchTerm?: string;
  page?: number;
  pageSize?: number;
}

@Injectable({
  providedIn: 'root',
})
export class MarketplaceApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/planning';

  searchMarketplace(params: MarketplaceSearchParams): Observable<PagedResult<MarketplaceItem>> {
    let httpParams = new HttpParams().set('sport', params.sport);

    if (params.type) {
      httpParams = httpParams.set('type', params.type);
    }
    if (params.filter) {
      httpParams = httpParams.set('filter', params.filter);
    }
    if (params.searchTerm) {
      httpParams = httpParams.set('searchTerm', params.searchTerm);
    }
    if (params.page) {
      httpParams = httpParams.set('page', params.page.toString());
    }
    if (params.pageSize) {
      httpParams = httpParams.set('pageSize', params.pageSize.toString());
    }

    return this.http.get<PagedResult<MarketplaceItem>>(`${this.baseUrl}/marketplace`, { params: httpParams });
  }

  getMarketplaceItemById(id: string): Observable<MarketplaceItem> {
    return this.http.get<MarketplaceItem>(`${this.baseUrl}/marketplace/${id}`);
  }

  downloadFromMarketplace(id: string): Observable<{ clonedEntityId: string }> {
    return this.http.post<{ clonedEntityId: string }>(`${this.baseUrl}/marketplace/${id}/download`, {});
  }

  rateMarketplaceItem(id: string, stars: number, comment?: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/marketplace/${id}/rate`, { stars, comment });
  }

  getItineraries(): Observable<Itinerary[]> {
    return this.http.get<Itinerary[]>(`${this.baseUrl}/itineraries`);
  }
}