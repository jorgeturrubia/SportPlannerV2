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
  templateUrl: './teams-page.visual.html',
  styleUrls: ['./teams-page.visual.css']
})
export class TeamsPage {
  teams = signal<any[]>([]);
  loading = signal(false);
  showForm = signal(false);
  viewMode = signal<'grid' | 'board' | 'carousel' | 'timeline' | 'hexagon' | 'datatable' | 'galaxy'>('grid');
  editing: any = null;
  error = signal<string | null>(null);
  Math = Math;

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
      const arr = Array.isArray(data) ? data : [];
      // If backend returns empty (dev), fallback to fake teams for demo
      if (arr.length === 0) {
        const { FAKE_TEAMS } = await import('./fake-teams');
        this.teams.set(FAKE_TEAMS);
      } else {
        this.teams.set(arr);
      }
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

  toggleView(mode: 'grid' | 'board' | 'carousel' | 'timeline' | 'hexagon' | 'datatable' | 'galaxy') {
    this.viewMode.set(mode);
  }

  // Para la vista de tabla avanzada
  expandedRows = signal<Set<string>>(new Set());
  sortColumn = signal<string>('name');
  sortDirection = signal<'asc' | 'desc'>('asc');
  searchTerm = signal<string>('');

  toggleRow(teamId: string) {
    const expanded = new Set(this.expandedRows());
    if (expanded.has(teamId)) {
      expanded.delete(teamId);
    } else {
      expanded.add(teamId);
    }
    this.expandedRows.set(expanded);
  }

  isRowExpanded(teamId: string): boolean {
    return this.expandedRows().has(teamId);
  }

  sortBy(column: string) {
    if (this.sortColumn() === column) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(column);
      this.sortDirection.set('asc');
    }
  }

  get filteredAndSortedTeams() {
    let filtered = this.teams();

    // Filtrar por búsqueda
    const search = this.searchTerm().toLowerCase();
    if (search) {
      filtered = filtered.filter(t =>
        t.name.toLowerCase().includes(search) ||
        t.description?.toLowerCase().includes(search)
      );
    }

    // Ordenar
    const col = this.sortColumn();
    const dir = this.sortDirection();

    return filtered.sort((a, b) => {
      let aVal = a[col];
      let bVal = b[col];

      if (col === 'members') {
        aVal = a.members.length;
        bVal = b.members.length;
      } else if (col === 'createdAt') {
        aVal = new Date(a.createdAt).getTime();
        bVal = new Date(b.createdAt).getTime();
      }

      if (aVal < bVal) return dir === 'asc' ? -1 : 1;
      if (aVal > bVal) return dir === 'asc' ? 1 : -1;
      return 0;
    });
  }

  getCapacityStatus(team: any): 'low' | 'medium' | 'high' | 'full' {
    const percentage = (team.members.length / 15) * 100;
    if (percentage >= 100) return 'full';
    if (percentage >= 75) return 'high';
    if (percentage >= 40) return 'medium';
    return 'low';
  }

  // Para la vista Galaxy
  selectedPlanet = signal<any | null>(null);
  galaxyAnimationFrame: number | null = null;

  selectPlanet(team: any) {
    this.selectedPlanet.set(team);
  }

  closePlanetDetail() {
    this.selectedPlanet.set(null);
  }

  getPlanetSize(team: any): number {
    // Tamaño base 60px + hasta 80px según capacidad
    const percentage = team.members.length / 15;
    return 60 + (percentage * 80);
  }

  getOrbitRadius(index: number, total: number): number {
    // Distribución en órbitas concéntricas
    const baseRadius = 180;
    const orbitSpacing = 100;
    const orbitLevel = Math.floor(index / 3);
    return baseRadius + (orbitLevel * orbitSpacing);
  }

  getOrbitSpeed(index: number): number {
    // Velocidades diferentes para cada órbita (más lento cuanto más lejos)
    return 20 + (index * 3);
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

  // Helper used from the template to build avatar URLs safely
  avatarUrl(name: unknown) {
    const s = typeof name === 'string' ? name : String(name ?? '');
    return `https://ui-avatars.com/api/?name=${encodeURIComponent(s)}&background=ffffff&color=000`;
  }
}
