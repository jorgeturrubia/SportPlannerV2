import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

export interface ItineraryItemDto {
  marketplaceItemId: string;
  name: string;
  type: number;
  order: number;
}

export interface ItineraryDto {
  id: string;
  name: string;
  description: string;
  sport: number;
  level: number;
  isActive: boolean;
  items: ItineraryItemDto[];
}

export interface CreateItineraryDto {
  name: string;
  description: string;
  sport: number;
  level: number;
  items: ItineraryItemToAdd[];
}

export interface ItineraryItemToAdd {
  marketplaceItemId: string;
  order: number;
}

@Injectable({
  providedIn: 'root'
})
export class ItinerariesService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/api/planning/itineraries`;

  async getItineraries(): Promise<ItineraryDto[]> {
    return await this.http.get<ItineraryDto[]>(this.apiUrl).toPromise() || [];
  }

  async getItinerary(id: string): Promise<ItineraryDto> {
    return await this.http.get<ItineraryDto>(`${this.apiUrl}/${id}`).toPromise() as ItineraryDto;
  }

  async createItinerary(dto: CreateItineraryDto): Promise<{ id: string }> {
    return await this.http.post<{ id: string }>(this.apiUrl, dto).toPromise() as { id: string };
  }
}
