import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TeamsService } from '../../services/teams.service';

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

  private teamsService = new TeamsService();

  // TODO: get real subscriptionId from context/route
  subscriptionId = '00000000-0000-0000-0000-000000000000';

  constructor(private router: Router) {
    this.load();
  }

  async load() {
    this.loading.set(true);
    try {
      const data = await this.teamsService.getTeams(this.subscriptionId);
      this.teams.set(Array.isArray(data) ? data : []);
    } catch (err) {
      console.error(err);
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
      const payload = {
        name: form.name,
        color: form.color,
        teamCategoryId: form.teamCategoryId || null,
        genderId: form.genderId || null,
        ageGroupId: form.ageGroupId || null,
        description: form.description
      };

      if (this.editing && this.editing.id) {
        await this.teamsService.updateTeam(this.subscriptionId, this.editing.id, payload);
      } else {
        await this.teamsService.createTeam(this.subscriptionId, payload);
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
      await this.teamsService.deleteTeam(this.subscriptionId, team.id);
      await this.load();
    } catch (err) {
      console.error(err);
      alert('Error al eliminar');
    }
  }
}
