import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ContentOwnership } from './objectives.service';

export interface WorkoutExerciseDetailDto {
  exerciseId: string;
  order: number;
  sets?: number;
  reps?: number;
  durationSeconds?: number;
  intensity?: string;
  restSeconds?: number;
}

export interface WorkoutDto {
  id: string;
  subscriptionId?: string;
  ownership: ContentOwnership;
  fecha: string;
  objectiveId?: string;
  objectiveName?: string;
  estimatedDurationMinutes?: number;
  notes?: string;
  isActive: boolean;
  exercises: WorkoutExerciseDetailDto[];
  createdAt: string;
  isEditable: boolean;
}

export interface CreateWorkoutDto {
  fecha: string; // Fecha del workout
  objectiveId?: string;
  estimatedDurationMinutes?: number;
  notes?: string;
  exercises: WorkoutExerciseDetailDto[];
}

export interface UpdateWorkoutDto {
  id: string;
  fecha: string; // Fecha del workout
  objectiveId?: string;
  estimatedDurationMinutes?: number;
  notes?: string;
  isActive: boolean;
  exercises: WorkoutExerciseDetailDto[];
}

@Injectable({
  providedIn: 'root'
})
export class WorkoutsService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/api/planning/workouts`;

  async getWorkouts(): Promise<WorkoutDto[]> {
    return await this.http.get<WorkoutDto[]>(this.apiUrl).toPromise() || [];
  }

  async getWorkout(id: string): Promise<WorkoutDto> {
    return await this.http.get<WorkoutDto>(`${this.apiUrl}/${id}`).toPromise() as WorkoutDto;
  }

  async createWorkout(dto: CreateWorkoutDto): Promise<{ id: string }> {
    return await this.http.post<{ id: string }>(this.apiUrl, dto).toPromise() as { id: string };
  }

  async updateWorkout(id: string, dto: UpdateWorkoutDto): Promise<void> {
    await this.http.put<void>(`${this.apiUrl}/${id}`, dto).toPromise();
  }

  async deleteWorkout(id: string): Promise<void> {
    await this.http.delete<void>(`${this.apiUrl}/${id}`).toPromise();
  }

  async cloneWorkout(id: string): Promise<{ id: string }> {
    return await this.http.post<{ id: string }>(`${this.apiUrl}/${id}/clone`, {}).toPromise() as { id: string };
  }
}
