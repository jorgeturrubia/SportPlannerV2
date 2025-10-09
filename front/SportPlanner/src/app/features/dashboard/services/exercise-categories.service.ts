import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Sport } from './objectives.service';

export interface ExerciseCategoryDto {
  id: string;
  sport: Sport;
  name: string;
  description: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateExerciseCategoryDto {
  sport: Sport;
  name: string;
  description: string;
}

export interface UpdateExerciseCategoryDto {
  sport: Sport;
  name: string;
  description: string;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ExerciseCategoriesService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/api/planning/exercise-categories`;

  async getCategories(sport?: Sport): Promise<ExerciseCategoryDto[]> {
    const params: Record<string, string> = {};
    if (sport !== undefined) {
      params['sport'] = sport.toString();
    }
    return await this.http.get<ExerciseCategoryDto[]>(this.apiUrl, { params }).toPromise() || [];
  }

  async createCategory(dto: CreateExerciseCategoryDto): Promise<ExerciseCategoryDto> {
    return await this.http.post<ExerciseCategoryDto>(this.apiUrl, dto).toPromise() as ExerciseCategoryDto;
  }

  async updateCategory(id: string, dto: UpdateExerciseCategoryDto): Promise<ExerciseCategoryDto> {
    return await this.http.put<ExerciseCategoryDto>(`${this.apiUrl}/${id}`, dto).toPromise() as ExerciseCategoryDto;
  }

  async deleteCategory(id: string): Promise<void> {
    await this.http.delete<void>(`${this.apiUrl}/${id}`).toPromise();
  }
}
