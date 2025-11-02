import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ContentOwnership } from './objectives.service';

export interface ExerciseDto {
  id: string;
  subscriptionId?: string;
  ownership: ContentOwnership;
  name: string;
  description: string;
  videoUrl?: string;
  imageUrl?: string;
  instructions?: string;
  defaultSets?: number;
  defaultReps?: number;
  defaultDurationSeconds?: number;
  defaultIntensity?: string;
  animationJson?: string;
  isActive: boolean;
  createdAt: string;
  objectiveIds: string[];
}

export interface CreateExerciseDto {
  name: string;
  description: string;
  videoUrl?: string;
  imageUrl?: string;
  instructions?: string;
  defaultSets?: number;
  defaultReps?: number;
  defaultDurationSeconds?: number;
  defaultIntensity?: string;
  animationJson?: string;
}

export interface UpdateExerciseDto {
  id: string;
  name: string;
  description: string;
  videoUrl?: string;
  imageUrl?: string;
  instructions?: string;
  defaultSets?: number;
  defaultReps?: number;
  defaultDurationSeconds?: number;
  defaultIntensity?: string;
  animationJson?: string;
  isActive: boolean;
  objectiveIds: string[];
}

@Injectable({
  providedIn: 'root'
})
export class ExercisesService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/api/planning/exercises`;

  async getExercises(): Promise<ExerciseDto[]> {
    return await this.http.get<ExerciseDto[]>(this.apiUrl).toPromise() || [];
  }

  async getExercise(id: string): Promise<ExerciseDto> {
    return await this.http.get<ExerciseDto>(`${this.apiUrl}/${id}`).toPromise() as ExerciseDto;
  }

  async createExercise(dto: CreateExerciseDto): Promise<{ id: string }> {
    return await this.http.post<{ id: string }>(this.apiUrl, dto).toPromise() as { id: string };
  }

  async updateExercise(id: string, dto: UpdateExerciseDto): Promise<void> {
    await this.http.put<void>(`${this.apiUrl}/${id}`, dto).toPromise();
  }

  async deleteExercise(id: string): Promise<void> {
    await this.http.delete<void>(`${this.apiUrl}/${id}`).toPromise();
  }

  async cloneExercise(id: string): Promise<{ id: string }> {
    return await this.http.post<{ id: string }>(`${this.apiUrl}/${id}/clone`, {}).toPromise() as { id: string };
  }
}
