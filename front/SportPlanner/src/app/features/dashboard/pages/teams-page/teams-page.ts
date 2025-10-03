import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TeamsService } from '../../services/teams.service';
import { SubscriptionContextService } from '../../../../core/subscription/services/subscription-context.service';

@Component({
  selector: 'app-teams-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teams-page.html',
  styleUrls: ['./teams-page.css']
})
export class TeamsPage {
  teams = signal<any[]>([]);
  loading = signal(false);
  showForm = signal(false);
  editing: any = null;
  error = signal<string | null>(null);

  private teamsService = inject(TeamsService);
  private subscriptionContext = inject(SubscriptionContextService);
  private router = inject(Router);

  constructor() {
    this.initialize();
  }

  private async initialize() {
    try {
      // Load subscription first
      await this.subscriptionContext.loadSubscription();

      // Then load teams
      await this.load();
    } catch (err) {
      console.error('Failed to initialize teams page:', err);
      this.error.set('No se pudo cargar la información de suscripción');
    }
  }

  async load() {
    this.loading.set(true);
    this.error.set(null);

    try {
      const subscriptionId = await this.subscriptionContext.ensureSubscriptionId();
      const data = await this.teamsService.getTeams(subscriptionId);
      this.teams.set(Array.isArray(data) ? data : []);
    } catch (err) {
      console.error('Failed to load teams:', err);
      this.error.set('No se pudieron cargar los equipos');
      this.teams.set([]);
    } finally {
      this.loading.set(false);
    }
  }

  openCreate() {
    this.editing = null;
    this.showForm.set(true);
  }

  openEdit(team: any) {
    this.editing = { ...team };
    this.showForm.set(true);
  }

  async submit(form: any) {
    try {
      const subscriptionId = await this.subscriptionContext.ensureSubscriptionId();

      const payload = {
        name: form.name,
        color: form.color,
        teamCategoryId: form.teamCategoryId || null,
        genderId: form.genderId || null,
        ageGroupId: form.ageGroupId || null,
        description: form.description
      };

      if (this.editing && this.editing.id) {
        await this.teamsService.updateTeam(subscriptionId, this.editing.id, payload);
      } else {
        await this.teamsService.createTeam(subscriptionId, payload);
      }

      this.showForm.set(false);
      await this.load();
    } catch (err) {
      console.error(err);
      alert('Error al guardar el equipo');
    }
  }

  async remove(team: any) {
    if (!confirm('¿Eliminar equipo?')) return;

    try {
      const subscriptionId = await this.subscriptionContext.ensureSubscriptionId();
      await this.teamsService.deleteTeam(subscriptionId, team.id);
      await this.load();
    } catch (err) {
      console.error(err);
      alert('Error al eliminar');
    }
  }
}
