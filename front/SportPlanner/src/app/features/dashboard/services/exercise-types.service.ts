import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

export interface ExerciseTypeDto {
  id: string;
  name: string;
  description: string;
  requiresSets: boolean;
  requiresReps: boolean;
  requiresDuration: boolean;
  isActive: boolean;
  createdAt: string;
}

export interface CreateExerciseTypeDto {
  name: string;
  description: string;
  requiresSets: boolean;
  requiresReps: boolean;
  requiresDuration: boolean;
}

export interface UpdateExerciseTypeDto {
  name: string;
  description: string;
  requiresSets: boolean;
  requiresReps: boolean;
  requiresDuration: boolean;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ExerciseTypesService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/api/planning/exercise-types`;

  async getTypes(): Promise<ExerciseTypeDto[]> {
    return await this.http.get<ExerciseTypeDto[]>(this.apiUrl).toPromise() || [];
  }

  async createType(dto: CreateExerciseTypeDto): Promise<ExerciseTypeDto> {
    return await this.http.post<ExerciseTypeDto>(this.apiUrl, dto).toPromise() as ExerciseTypeDto;
  }

  async updateType(id: string, dto: UpdateExerciseTypeDto): Promise<ExerciseTypeDto> {
    return await this.http.put<ExerciseTypeDto>(`${this.apiUrl}/${id}`, dto).toPromise() as ExerciseTypeDto;
  }

  async deleteType(id: string): Promise<void> {
    await this.http.delete<void>(`${this.apiUrl}/${id}`).toPromise();
  }
}
