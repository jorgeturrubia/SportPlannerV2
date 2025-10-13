import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

export enum ContentOwnership {
  System = 0,
  User = 1,
  MarketplaceUser = 2
}

export enum Sport {
  Football = 0,
  Basketball = 1,
  Handball = 2
}

export interface ObjectiveTechniqueDto {
  description: string;
  order: number;
}

export interface ObjectiveDto {
  id: string;
  subscriptionId?: string;
  ownership: ContentOwnership;
  sport: Sport;
  name: string;
  description: string;
  objectiveCategoryId: string;
  objectiveSubcategoryId?: string;
  level: number;
  isActive: boolean;
  sourceMarketplaceItemId?: string;
  techniques: ObjectiveTechniqueDto[];
  createdAt: string;
  updatedAt?: string;
  isSystemContent: boolean;
  isUserContent: boolean;
  isMarketplaceContent: boolean;
  isEditable: boolean;
}

export interface CreateObjectiveDto {
  sport: Sport;
  name: string;
  description: string;
  objectiveCategoryId: string;
  objectiveSubcategoryId?: string;
  level?: number;
  techniques: ObjectiveTechniqueDto[];
}

export interface UpdateObjectiveDto {
  id: string;
  name: string;
  description: string;
  objectiveCategoryId: string;
  objectiveSubcategoryId?: string;
  level?: number | null;
  techniques: ObjectiveTechniqueDto[];
}

@Injectable({
  providedIn: 'root'
})
export class ObjectivesService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/api/planning/objectives`;

  async getObjectives(): Promise<ObjectiveDto[]> {
    return await this.http.get<ObjectiveDto[]>(this.apiUrl).toPromise() || [];
  }

  async getObjective(id: string): Promise<ObjectiveDto | null> {
    return await this.http.get<ObjectiveDto>(`${this.apiUrl}/${id}`).toPromise() || null;
  }

  async createObjective(objective: CreateObjectiveDto): Promise<string> {
    const response = await this.http.post<{ id: string }>(this.apiUrl, objective).toPromise();
    return response?.id || '';
  }

  async updateObjective(id: string, objective: UpdateObjectiveDto): Promise<void> {
    await this.http.put<void>(`${this.apiUrl}/${id}`, objective).toPromise();
  }

  async deleteObjective(id: string): Promise<void> {
    await this.http.delete<void>(`${this.apiUrl}/${id}`).toPromise();
  }

  async cloneObjective(id: string): Promise<string> {
    const response = await this.http.post<{ id: string }>(`${this.apiUrl}/${id}/clone`, {}).toPromise();
    return response?.id || '';
  }
}
