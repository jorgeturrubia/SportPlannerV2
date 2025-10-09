import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { CalendarService, CalendarEventDto, CreateCalendarEventDto } from '../../services/calendar.service';
import { TeamsService } from '../../services/teams.service';
import { WorkoutsService } from '../../services/workouts.service';
import { DynamicFormComponent, FormField } from '../../../../shared/components/dynamic-form/dynamic-form.component';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { NotificationService } from '../../../../shared/notifications/notification.service';

interface CalendarDay {
  date: Date;
  isCurrentMonth: boolean;
  events: CalendarEventDto[];
}

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule, TranslateModule, DynamicFormComponent, ConfirmationDialogComponent],
  templateUrl: './calendar.page.html',
})
export class CalendarPage {
  private calendarService = inject(CalendarService);
  private teamsService = inject(TeamsService);
  private workoutsService = inject(WorkoutsService);
  private notificationService = inject(NotificationService);

  // State
  currentDate = signal(new Date());
  events = signal<CalendarEventDto[]>([]);
  teams = signal<any[]>([]);
  workouts = signal<any[]>([]);

  selectedTeamId = signal<string | null>(null);
  showEventForm = signal(false);
  showDeleteConfirm = signal(false);
  eventToDelete = signal<CalendarEventDto | null>(null);
  editingEvent = signal<CalendarEventDto | null>(null);
  selectedDate = signal<Date | null>(null);

  // Computed
  currentMonth = computed(() => {
    const date = this.currentDate();
    return date.toLocaleString('default', { month: 'long', year: 'numeric' });
  });

  calendarDays = computed(() => {
    const date = this.currentDate();
    const year = date.getFullYear();
    const month = date.getMonth();

    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const daysInMonth = lastDay.getDate();
    const startingDayOfWeek = firstDay.getDay();

    const days: CalendarDay[] = [];

    // Previous month days
    const prevMonthLastDay = new Date(year, month, 0).getDate();
    for (let i = startingDayOfWeek - 1; i >= 0; i--) {
      const dayDate = new Date(year, month - 1, prevMonthLastDay - i);
      days.push({
        date: dayDate,
        isCurrentMonth: false,
        events: this.getEventsForDate(dayDate),
      });
    }

    // Current month days
    for (let day = 1; day <= daysInMonth; day++) {
      const dayDate = new Date(year, month, day);
      days.push({
        date: dayDate,
        isCurrentMonth: true,
        events: this.getEventsForDate(dayDate),
      });
    }

    // Next month days to complete the grid
    const remainingDays = 42 - days.length; // 6 weeks * 7 days
    for (let day = 1; day <= remainingDays; day++) {
      const dayDate = new Date(year, month + 1, day);
      days.push({
        date: dayDate,
        isCurrentMonth: false,
        events: this.getEventsForDate(dayDate),
      });
    }

    return days;
  });

  formConfig = computed<FormField[]>(() => [
    {
      key: 'teamId',
      label: 'Equipo',
      type: 'select',
      required: true,
      options: this.teams().map((t) => ({ label: t.name, value: t.id })),
    },
    {
      key: 'workoutId',
      label: 'Entreno',
      type: 'select',
      required: true,
      options: this.workouts().map((w) => ({ label: w.name, value: w.id })),
    },
    {
      key: 'scheduledDate',
      label: 'Fecha y Hora',
      type: 'text',
      required: true,
    },
    {
      key: 'durationMinutes',
      label: 'Duración (minutos)',
      type: 'number',
      required: true,
    },
    {
      key: 'notes',
      label: 'Notas',
      type: 'textarea',
      required: false,
    },
  ]);

  async ngOnInit() {
    await this.loadData();
  }

  async loadData() {
    try {
      // TODO: Get actual subscription ID from auth context
      const mockSubscriptionId = '00000000-0000-0000-0000-000000000000';
      const [teams, workouts] = await Promise.all([
        this.teamsService.getTeams(mockSubscriptionId),
        this.workoutsService.getWorkouts(),
      ]);
      this.teams.set(teams);
      this.workouts.set(workouts);
      await this.loadEvents();
    } catch (error) {
      console.error('Error loading data:', error);
      this.notificationService.error('Error cargando datos');
    }
  }

  async loadEvents() {
    try {
      const date = this.currentDate();
      const startDate = new Date(date.getFullYear(), date.getMonth(), 1);
      const endDate = new Date(date.getFullYear(), date.getMonth() + 1, 0);

      const events = await this.calendarService.getEvents(
        startDate,
        endDate,
        this.selectedTeamId() || undefined
      );
      this.events.set(events);
    } catch (error) {
      console.error('Error loading events:', error);
      this.notificationService.error('Error cargando eventos');
    }
  }

  getEventsForDate(date: Date): CalendarEventDto[] {
    return this.events().filter((event) => {
      const eventDate = new Date(event.scheduledDate);
      return (
        eventDate.getFullYear() === date.getFullYear() &&
        eventDate.getMonth() === date.getMonth() &&
        eventDate.getDate() === date.getDate()
      );
    });
  }

  previousMonth() {
    const date = new Date(this.currentDate());
    date.setMonth(date.getMonth() - 1);
    this.currentDate.set(date);
    this.loadEvents();
  }

  nextMonth() {
    const date = new Date(this.currentDate());
    date.setMonth(date.getMonth() + 1);
    this.currentDate.set(date);
    this.loadEvents();
  }

  todayView() {
    this.currentDate.set(new Date());
    this.loadEvents();
  }

  onDayClick(day: CalendarDay) {
    this.selectedDate.set(day.date);
    this.openEventForm();
  }

  openEventForm() {
    this.editingEvent.set(null);
    this.showEventForm.set(true);
  }

  closeEventForm() {
    this.showEventForm.set(false);
    this.editingEvent.set(null);
    this.selectedDate.set(null);
  }

  async onFormSubmit(formData: any) {
    try {
      const dto: CreateCalendarEventDto = {
        teamId: formData.teamId,
        workoutId: formData.workoutId,
        scheduledDate: new Date(formData.scheduledDate),
        durationMinutes: Number(formData.durationMinutes),
        notes: formData.notes || undefined,
      };

      await this.calendarService.createEvent(dto);
      this.notificationService.success('Evento creado exitosamente');
      this.closeEventForm();
      await this.loadEvents();
    } catch (error) {
      console.error('Error creating event:', error);
      this.notificationService.error('Error creando evento');
    }
  }

  confirmDelete(event: CalendarEventDto) {
    this.eventToDelete.set(event);
    this.showDeleteConfirm.set(true);
  }

  async deleteEvent() {
    const event = this.eventToDelete();
    if (!event) return;

    try {
      await this.calendarService.deleteEvent(event.id);
      this.notificationService.success('Evento eliminado');
      this.showDeleteConfirm.set(false);
      this.eventToDelete.set(null);
      await this.loadEvents();
    } catch (error) {
      console.error('Error deleting event:', error);
      this.notificationService.error('Error eliminando evento');
    }
  }

  async toggleCompletion(event: CalendarEventDto) {
    try {
      await this.calendarService.toggleCompletion(event.id);
      this.notificationService.success(
        event.isCompleted ? 'Marcado como incompleto' : 'Marcado como completado'
      );
      await this.loadEvents();
    } catch (error) {
      console.error('Error toggling completion:', error);
      this.notificationService.error('Error actualizando estado');
    }
  }

  async filterByTeam(teamId: string | null) {
    this.selectedTeamId.set(teamId);
    await this.loadEvents();
  }

  isToday(date: Date): boolean {
    const today = new Date();
    return (
      date.getFullYear() === today.getFullYear() &&
      date.getMonth() === today.getMonth() &&
      date.getDate() === today.getDate()
    );
  }
}
