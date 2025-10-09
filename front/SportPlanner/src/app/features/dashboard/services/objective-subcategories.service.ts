import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

export interface ObjectiveSubcategoryDto {
  id: string;
  objectiveCategoryId: string;
  name: string;
}

export interface CreateObjectiveSubcategoryDto {
  name: string;
  objectiveCategoryId: string;
}

export interface UpdateObjectiveSubcategoryDto {
  name: string;
  objectiveCategoryId: string;
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

  async createSubcategory(dto: CreateObjectiveSubcategoryDto): Promise<ObjectiveSubcategoryDto> {
    return await this.http.post<ObjectiveSubcategoryDto>(this.apiUrl, dto).toPromise() as ObjectiveSubcategoryDto;
  }

  async updateSubcategory(id: string, dto: UpdateObjectiveSubcategoryDto): Promise<ObjectiveSubcategoryDto> {
    return await this.http.put<ObjectiveSubcategoryDto>(`${this.apiUrl}/${id}`, dto).toPromise() as ObjectiveSubcategoryDto;
  }

  async deleteSubcategory(id: string): Promise<void> {
    await this.http.delete<void>(`${this.apiUrl}/${id}`).toPromise();
  }
}
