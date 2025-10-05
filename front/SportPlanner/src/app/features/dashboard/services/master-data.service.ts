import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../../environments/environment';

export interface GenderResponse {
  id: string;
  name: string;
  code: string;
  description?: string;
  isActive: boolean;
}

export interface AgeGroupResponse {
  id: string;
  name: string;
  code: string;
  minAge: number;
  maxAge: number;
  sport: number;
  sortOrder: number;
  isActive: boolean;
}

export interface TeamCategoryResponse {
  id: string;
  name: string;
  code: string;
  description?: string;
  sortOrder: number;
  sport: number;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class MasterDataService {
  private http = inject(HttpClient);
  private platformId = inject(PLATFORM_ID);
  private apiBaseUrl = environment.apiBaseUrl;

  private isBrowser() {
    return isPlatformBrowser(this.platformId);
  }

  // ===== GENDERS =====
  async getGenders() {
    if (!this.isBrowser()) return [] as GenderResponse[];
    return firstValueFrom(
      this.http.get<GenderResponse[]>(`${this.apiBaseUrl}/api/master-data/genders`)
    );
  }

  async createGender(data: Partial<GenderResponse>) {
    if (!this.isBrowser()) return null;
    return firstValueFrom(
      this.http.post<GenderResponse>(`${this.apiBaseUrl}/api/master-data/genders`, data)
    );
  }

  async updateGender(id: string, data: Partial<GenderResponse>) {
    if (!this.isBrowser()) return null;
    return firstValueFrom(
      this.http.put<GenderResponse>(`${this.apiBaseUrl}/api/master-data/genders/${id}`, data)
    );
  }

  async deleteGender(id: string) {
    if (!this.isBrowser()) return false;
    try {
      await firstValueFrom(
        this.http.delete(`${this.apiBaseUrl}/api/master-data/genders/${id}`)
      );
      return true;
    } catch {
      return false;
    }
  }

  // ===== AGE GROUPS =====
  async getAgeGroups(sport?: number) {
    if (!this.isBrowser()) return [] as AgeGroupResponse[];
    const params = sport !== undefined ? `?sport=${sport}` : '';
    return firstValueFrom(
      this.http.get<AgeGroupResponse[]>(`${this.apiBaseUrl}/api/master-data/age-groups${params}`)
    );
  }

  async createAgeGroup(data: Partial<AgeGroupResponse>) {
    if (!this.isBrowser()) return null;
    return firstValueFrom(
      this.http.post<AgeGroupResponse>(`${this.apiBaseUrl}/api/master-data/age-groups`, data)
    );
  }

  async updateAgeGroup(id: string, data: Partial<AgeGroupResponse>) {
    if (!this.isBrowser()) return null;
    return firstValueFrom(
      this.http.put<AgeGroupResponse>(`${this.apiBaseUrl}/api/master-data/age-groups/${id}`, data)
    );
  }

  async deleteAgeGroup(id: string) {
    if (!this.isBrowser()) return false;
    try {
      await firstValueFrom(
        this.http.delete(`${this.apiBaseUrl}/api/master-data/age-groups/${id}`)
      );
      return true;
    } catch {
      return false;
    }
  }

  // ===== TEAM CATEGORIES =====
  async getTeamCategories(sport?: number) {
    if (!this.isBrowser()) return [] as TeamCategoryResponse[];
    const params = sport !== undefined ? `?sport=${sport}` : '';
    return firstValueFrom(
      this.http.get<TeamCategoryResponse[]>(`${this.apiBaseUrl}/api/master-data/team-categories${params}`)
    );
  }

  async createTeamCategory(data: Partial<TeamCategoryResponse>) {
    if (!this.isBrowser()) return null;
    return firstValueFrom(
      this.http.post<TeamCategoryResponse>(`${this.apiBaseUrl}/api/master-data/team-categories`, data)
    );
  }

  async updateTeamCategory(id: string, data: Partial<TeamCategoryResponse>) {
    if (!this.isBrowser()) return null;
    return firstValueFrom(
      this.http.put<TeamCategoryResponse>(`${this.apiBaseUrl}/api/master-data/team-categories/${id}`, data)
    );
  }

  async deleteTeamCategory(id: string) {
    if (!this.isBrowser()) return false;
    try {
      await firstValueFrom(
        this.http.delete(`${this.apiBaseUrl}/api/master-data/team-categories/${id}`)
      );
      return true;
    } catch {
      return false;
    }
  }
}
