import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

export interface TrainingScheduleDto {
  trainingDays: number[];
  hoursPerDay: { [key: number]: number };
  totalWeeks: number;
  totalSessions: number;
  totalHours: number;
}

export interface PlanObjectiveDto {
  objectiveId: string;
  objectiveName: string;
  priority: number;
  targetSessions: number;
  level?: number;
}

export interface TrainingPlanDto {
  id: string;
  subscriptionId: string;
  name: string;
  startDate: string;
  endDate: string;
  schedule: TrainingScheduleDto;
  isActive: boolean;
  marketplaceItemId?: string;
  objectives: PlanObjectiveDto[];
  createdAt: string;
  updatedAt?: string;
  durationDays: number;
  isTargetSessionsBalanced: boolean;
}

export interface CreateTrainingPlanDto {
  name: string;
  startDate: string;
  endDate: string;
  schedule: TrainingScheduleDto;
  isActive?: boolean;
  objectives?: AddObjectiveToPlanDto[];
}

export interface UpdateTrainingPlanDto {
  id: string;
  name: string;
  startDate: string;
  endDate: string;
  schedule: TrainingScheduleDto;
  isActive: boolean;
}

export interface AddObjectiveToPlanDto {
  objectiveId: string;
  priority: number;
  targetSessions: number;
}

@Injectable({
  providedIn: 'root'
})
export class TrainingPlansService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/api/planning/training-plans`;

  async getPlans(): Promise<TrainingPlanDto[]> {
    return await this.http.get<TrainingPlanDto[]>(this.apiUrl).toPromise() || [];
  }

  async getPlan(id: string): Promise<TrainingPlanDto> {
    console.log('📡 TrainingPlansService.getPlan - Requesting plan with ID:', id);
    const plan = await this.http.get<TrainingPlanDto>(`${this.apiUrl}/${id}`).toPromise() as TrainingPlanDto;
    console.log('📥 TrainingPlansService.getPlan - Received plan:', plan);
    console.log('📥 startDate type:', typeof plan.startDate, 'value:', plan.startDate);
    console.log('📥 endDate type:', typeof plan.endDate, 'value:', plan.endDate);
    return plan;
  }

  async createPlan(dto: CreateTrainingPlanDto): Promise<{ id: string }> {
    console.log('📤 TrainingPlansService.createPlan - Sending DTO:', dto);
    console.log('📤 startDate type:', typeof dto.startDate, 'value:', dto.startDate);
    console.log('📤 endDate type:', typeof dto.endDate, 'value:', dto.endDate);

    const result = await this.http.post<{ id: string }>(this.apiUrl, dto).toPromise() as { id: string };

    console.log('📥 TrainingPlansService.createPlan - Response:', result);
    return result;
  }

  async updatePlan(id: string, dto: UpdateTrainingPlanDto): Promise<void> {
    console.log('📤 TrainingPlansService.updatePlan - ID:', id, 'DTO:', dto);
    console.log('📤 startDate type:', typeof dto.startDate, 'value:', dto.startDate);
    console.log('📤 endDate type:', typeof dto.endDate, 'value:', dto.endDate);

    await this.http.put<void>(`${this.apiUrl}/${id}`, dto).toPromise();

    console.log('📥 TrainingPlansService.updatePlan - Success for ID:', id);
  }

  async addObjectiveToPlan(planId: string, dto: AddObjectiveToPlanDto): Promise<void> {
    await this.http.post<void>(`${this.apiUrl}/${planId}/objectives`, dto).toPromise();
  }

  async addObjectivesToPlan(planId: string, objectives: AddObjectiveToPlanDto[]): Promise<void> {
    const request = { objectives };
    await this.http.post<void>(`${this.apiUrl}/${planId}/objectives/batch`, request).toPromise();
  }

  async removeObjectiveFromPlan(planId: string, objectiveId: string): Promise<void> {
    await this.http.delete<void>(`${this.apiUrl}/${planId}/objectives/${objectiveId}`).toPromise();
  }
}
