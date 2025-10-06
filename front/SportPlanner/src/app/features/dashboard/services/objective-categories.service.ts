import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Sport } from './objectives.service';

export interface ObjectiveCategoryDto {
  id: string;
  name: string;
  sport: Sport;
}

@Injectable({
  providedIn: 'root'
})
export class ObjectiveCategoriesService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/api/planning/objective-categories`;

  async getCategories(sport?: Sport): Promise<ObjectiveCategoryDto[]> {
    const params: Record<string, string> = {};
    if (sport !== undefined) {
      params['sport'] = sport.toString();
    }
    return await this.http.get<ObjectiveCategoryDto[]>(this.apiUrl, { params }).toPromise() || [];
  }
}
