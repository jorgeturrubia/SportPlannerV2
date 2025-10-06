import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

export interface ObjectiveSubcategoryDto {
  id: string;
  objectiveCategoryId: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class ObjectiveSubcategoriesService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/api/planning/objective-subcategories`;

  async getSubcategories(categoryId?: string): Promise<ObjectiveSubcategoryDto[]> {
    const params: Record<string, string> = {};
    if (categoryId) {
      params['categoryId'] = categoryId;
    }
    return await this.http.get<ObjectiveSubcategoryDto[]>(this.apiUrl, { params }).toPromise() || [];
  }
}
