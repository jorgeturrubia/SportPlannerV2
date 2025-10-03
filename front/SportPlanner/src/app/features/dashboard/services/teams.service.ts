import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TeamsService {
  private http = inject(HttpClient);
  private platformId = inject(PLATFORM_ID);
  private apiBaseUrl = environment.apiBaseUrl;

  // Helper to detect browser vs SSR
  private isBrowser() {
    return isPlatformBrowser(this.platformId);
  }

  async getTeams(subscriptionId: string) {
    if (!this.isBrowser()) {
      // During SSR return empty list to avoid server fetch
      return [] as any[];
    }
    return firstValueFrom(
      this.http.get<any[]>(`${this.apiBaseUrl}/api/subscriptions/${subscriptionId}/teams`)
    );
  }

  async getTeam(subscriptionId: string, teamId: string) {
    if (!this.isBrowser()) return null;
    return firstValueFrom(
      this.http.get<any>(`${this.apiBaseUrl}/api/subscriptions/${subscriptionId}/teams/${teamId}`)
    );
  }

  async createTeam(subscriptionId: string, payload: any) {
    if (!this.isBrowser()) return null;
    return firstValueFrom(
      this.http.post<any>(`${this.apiBaseUrl}/api/subscriptions/${subscriptionId}/teams`, payload)
    );
  }

  async updateTeam(subscriptionId: string, teamId: string, payload: any) {
    if (!this.isBrowser()) return null;
    return firstValueFrom(
      this.http.put<any>(`${this.apiBaseUrl}/api/subscriptions/${subscriptionId}/teams/${teamId}`, payload)
    );
  }

  async deleteTeam(subscriptionId: string, teamId: string) {
    if (!this.isBrowser()) return false;
    try {
      await firstValueFrom(
        this.http.delete(`${this.apiBaseUrl}/api/subscriptions/${subscriptionId}/teams/${teamId}`)
      );
      return true;
    } catch {
      return false;
    }
  }
}
