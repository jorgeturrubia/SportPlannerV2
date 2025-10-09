import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';

export interface CalendarEventDto {
  id: string;
  subscriptionId: string;
  teamId: string;
  teamName: string;
  workoutId: string;
  workoutName: string;
  trainingPlanId?: string;
  trainingPlanName?: string;
  scheduledDate: Date;
  durationMinutes: number;
  notes?: string;
  isCompleted: boolean;
  completedAt?: Date;
  createdAt: Date;
}

export interface CreateCalendarEventDto {
  teamId: string;
  workoutId: string;
  trainingPlanId?: string;
  scheduledDate: Date;
  durationMinutes: number;
  notes?: string;
}

export interface UpdateCalendarEventDto {
  scheduledDate: Date;
  durationMinutes: number;
  notes?: string;
}

@Injectable({
  providedIn: 'root',
})
export class CalendarService {
  private readonly apiUrl = `${environment.apiBaseUrl}/calendar`;

  async getEvents(
    startDate?: Date,
    endDate?: Date,
    teamId?: string,
    isCompleted?: boolean
  ): Promise<CalendarEventDto[]> {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate.toISOString());
    if (endDate) params.append('endDate', endDate.toISOString());
    if (teamId) params.append('teamId', teamId);
    if (isCompleted !== undefined) params.append('isCompleted', String(isCompleted));

    const url = `${this.apiUrl}${params.toString() ? '?' + params.toString() : ''}`;
    const response = await fetch(url, {
      credentials: 'include',
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch events: ${response.statusText}`);
    }

    const data = await response.json();
    return data.map((event: any) => ({
      ...event,
      scheduledDate: new Date(event.scheduledDate),
      completedAt: event.completedAt ? new Date(event.completedAt) : undefined,
      createdAt: new Date(event.createdAt),
    }));
  }

  async createEvent(dto: CreateCalendarEventDto): Promise<{ id: string }> {
    const response = await fetch(this.apiUrl, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
      body: JSON.stringify(dto),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Failed to create event: ${errorText}`);
    }

    return response.json();
  }

  async updateEvent(id: string, dto: UpdateCalendarEventDto): Promise<void> {
    const response = await fetch(`${this.apiUrl}/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
      body: JSON.stringify(dto),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Failed to update event: ${errorText}`);
    }
  }

  async deleteEvent(id: string): Promise<void> {
    const response = await fetch(`${this.apiUrl}/${id}`, {
      method: 'DELETE',
      credentials: 'include',
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Failed to delete event: ${errorText}`);
    }
  }

  async toggleCompletion(id: string): Promise<void> {
    const response = await fetch(`${this.apiUrl}/${id}/toggle-completion`, {
      method: 'POST',
      credentials: 'include',
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Failed to toggle completion: ${errorText}`);
    }
  }
}
