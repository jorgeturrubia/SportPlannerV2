import { Component, signal, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { EntityPageLayoutComponent } from '../../../../shared/components/entity-page-layout/entity-page-layout.component';
import { CardComponent } from '../../../../shared/components/card/card.component';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { CardCarouselComponent } from '../../../../shared/components/card-carousel/card-carousel.component';
import { DynamicFormComponent, FormField, FormLayout } from '../../../../shared/components/dynamic-form/dynamic-form.component';
import { TeamsService } from '../../services/teams.service';
import { SubscriptionContextService } from '../../../../core/subscription/services/subscription-context.service';
import { MasterDataService } from '../../services/master-data.service';
import { NotificationService } from '../../../../shared/notifications/notification.service';

// Enums from backend
export enum TeamColor {
  Rojo = 0,
  Azul = 1,
  Verde = 2,
  Amarillo = 3,
  Negro = 4,
  Blanco = 5,
  Naranja = 6,
  Morado = 7,
  Rosa = 8,
  Marron = 9,
  Gris = 10,
  Celeste = 11
}

export enum Sport {
  Football = 0,
  Basketball = 1,
  Handball = 2
}

// Backend DTOs
interface GenderResponse {
  id: string;
  name: string;
  code: string;
  description?: string;
  isActive: boolean;
}

interface AgeGroupResponse {
  id: string;
  name: string;
  code: string;
  minAge: number;
  maxAge: number;
  sport: Sport;
  sortOrder: number;
  isActive: boolean;
}

interface TeamCategoryResponse {
  id: string;
  name: string;
  code: string;
  description?: string;
  sortOrder: number;
  sport: Sport;
  isActive: boolean;
}

interface TeamResponse {
  id: string;
  subscriptionId: string;
  name: string;
  color: TeamColor;
  sport: Sport;
  description?: string;
  coachSubscriptionUserId?: string;
  coachFirstName?: string;
  coachLastName?: string;
  coachEmail?: string;
  season?: string;
  maxPlayers: number;
  currentPlayersCount: number;
  lastMatchDate?: string;
  allowMixedGender: boolean;
  isActive: boolean;
  category: TeamCategoryResponse;
  gender: GenderResponse;
  ageGroup: AgeGroupResponse;
  createdAt: string;
}

interface CreateTeamRequest {
  name: string;
  color: TeamColor;
  teamCategoryId: string;
  genderId: string;
  ageGroupId: string;
  description?: string;
  coachSubscriptionUserId?: string;
  season?: string;
  allowMixedGender?: boolean;
}

@Component({
  selector: 'app-teams',
  standalone: true,
  imports: [CommonModule, TranslateModule, EntityPageLayoutComponent, CardComponent, ConfirmationDialogComponent, DynamicFormComponent, CardCarouselComponent],
  templateUrl: './teams.html',
  styleUrls: ['./teams.css']
})
export class TeamsPage implements OnInit {
  private teamsService = inject(TeamsService);
  private subscriptionContext = inject(SubscriptionContextService);
  private masterDataService = inject(MasterDataService);
  private ns = inject(NotificationService);

  teams = signal<TeamResponse[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  // Master data options
  genders = signal<GenderResponse[]>([]);
  ageGroups = signal<AgeGroupResponse[]>([]);
  teamCategories = signal<TeamCategoryResponse[]>([]);

  // Dialog and Form state
  isConfirmDialogOpen = signal(false);
  isFormOpen = signal(false);
  selectedTeam = signal<TeamResponse | null>(null);
  formTitle = 'Add New Team';

  teamFormLayout: FormLayout = {
    columns: 2, // 2 columns grid
    fields: []  // Will be populated by computed
  };

  teamFormConfig = computed<FormField[]>(() => [
    { key: 'name', label: 'Team Name', type: 'text', required: true, colspan: 2 },
    { key: 'color', label: 'Color', type: 'select', required: true, options: this.getColorOptions(), colspan: 1 },
    {
      key: 'teamCategoryId',
      label: 'Category',
      type: 'select',
      required: true,
      options: this.teamCategories().map(c => ({ value: c.id, label: c.name })),
      colspan: 1
    },
    {
      key: 'genderId',
      label: 'Gender',
      type: 'select',
      required: true,
      options: this.genders().map(g => ({ value: g.id, label: g.name })),
      colspan: 1
    },
    {
      key: 'ageGroupId',
      label: 'Age Group',
      type: 'select',
      required: true,
      options: this.ageGroups().map(a => ({ value: a.id, label: a.name })),
      colspan: 1
    },
    { key: 'description', label: 'Description', type: 'textarea', colspan: 2 }
  ]);

  async ngOnInit(): Promise<void> {
    // Ensure subscription is loaded first
    await this.subscriptionContext.loadSubscription();

    await Promise.all([
      this.loadTeams(),
      this.loadMasterData()
    ]);
  }

  private async loadMasterData(): Promise<void> {
    try {
      const subscription = this.subscriptionContext.subscription();
      if (!subscription) {
        console.error('No subscription available to load master data');
        return;
      }

      const sport = this.parseSportToEnum(subscription.sport);

      const [genders, ageGroups, teamCategories] = await Promise.all([
        this.masterDataService.getGenders(),
        this.masterDataService.getAgeGroups(sport),
        this.masterDataService.getTeamCategories(sport)
      ]);

      this.genders.set(genders);
      this.ageGroups.set(ageGroups);
      this.teamCategories.set(teamCategories);
    } catch (err: any) {
      console.error('Failed to load master data:', err);
      this.ns.error(err?.message ?? 'Error al cargar datos maestros', 'Error');
    }
  }

  private parseSportToEnum(sportStr: string): number {
    switch (sportStr) {
      case 'Football': return Sport.Football;
      case 'Basketball': return Sport.Basketball;
      case 'Handball': return Sport.Handball;
      default: return Sport.Football;
    }
  }

  private async loadTeams(): Promise<void> {
    try {
      this.isLoading.set(true);
      this.error.set(null);

      const subscriptionId = await this.subscriptionContext.ensureSubscriptionId();
      const teams = await this.teamsService.getTeams(subscriptionId);

      this.teams.set(teams || []);
    } catch (err: any) {
      console.error('Failed to load teams:', err);
      this.error.set(err.message || 'Failed to load teams');
      this.ns.error(err?.message ?? 'Error al cargar equipos', 'Error');
      this.teams.set([]);
    } finally {
      this.isLoading.set(false);
    }
  }

  getColorOptions(): Array<{ value: any; label: string }> {
    return Object.keys(TeamColor)
      .filter(key => !isNaN(Number(TeamColor[key as any])))
      .map(key => ({
        value: TeamColor[key as any],
        label: key
      }));
  }

  getSportIcon(sport: Sport): string {
    switch (sport) {
      case Sport.Football: return '⚽';
      case Sport.Basketball: return '🏀';
      case Sport.Handball: return '🤾';
      default: return '🏆';
    }
  }

  getSportName(sport: Sport): string {
    switch (sport) {
      case Sport.Football: return 'Football';
      case Sport.Basketball: return 'Basketball';
      case Sport.Handball: return 'Handball';
      default: return 'Unknown';
    }
  }

  getColorName(color: TeamColor): string {
    return TeamColor[color] || 'Unknown';
  }


  // --- Delete Logic ---
  openDeleteConfirm(team: TeamResponse): void {
    this.selectedTeam.set(team);
    this.isConfirmDialogOpen.set(true);
  }

  async handleDeleteConfirm(): Promise<void> {
    const teamToDelete = this.selectedTeam();
    if (!teamToDelete) return;

    try {
      const subscriptionId = await this.subscriptionContext.ensureSubscriptionId();
      const success = await this.teamsService.deleteTeam(subscriptionId, teamToDelete.id);

      if (success) {
        this.ns.success('Equipo eliminado', 'Equipos');
        this.teams.update(teams => teams.filter(t => t.id !== teamToDelete.id));
      } else {
        this.error.set('Failed to delete team');
        this.ns.error('No se pudo eliminar el equipo', 'Error');
      }
    } catch (err: any) {
      console.error('Failed to delete team:', err);
      this.error.set(err.message || 'Failed to delete team');
      this.ns.error(err?.message ?? 'No se pudo eliminar el equipo', 'Error');
    } finally {
      this.closeConfirmDialog();
    }
  }

  closeConfirmDialog(): void {
    this.isConfirmDialogOpen.set(false);
    this.selectedTeam.set(null);
  }

  // --- Add/Edit Logic ---
  openAddForm(): void {
    this.selectedTeam.set(null);
    this.formTitle = 'Add New Team';
    this.isFormOpen.set(true);
  }

  openEditForm(team: TeamResponse): void {
    this.selectedTeam.set(team);
    this.formTitle = `Edit ${team.name}`;
    this.isFormOpen.set(true);
  }

  async handleFormSubmit(formData: any): Promise<void> {
    const selected = this.selectedTeam();

    try {
      const subscriptionId = await this.subscriptionContext.ensureSubscriptionId();

      if (selected) {
        // Update existing team
        const updatePayload = {
          name: formData.name,
          color: Number(formData.color),
          description: formData.description
        };

        const updatedTeam = await this.teamsService.updateTeam(
          subscriptionId,
          selected.id,
          updatePayload
        );

        if (updatedTeam) {
          this.teams.update(teams =>
            teams.map(t => t.id === selected.id ? updatedTeam : t)
          );
          this.ns.success('Equipo actualizado', 'Equipos');
        }
      } else {
        // Add new team - Using values from form
        const createPayload: CreateTeamRequest = {
          name: formData.name,
          color: Number(formData.color),
          teamCategoryId: formData.teamCategoryId,
          genderId: formData.genderId,
          ageGroupId: formData.ageGroupId,
          description: formData.description,
          allowMixedGender: false
        };

        const newTeamId = await this.teamsService.createTeam(subscriptionId, createPayload);

        if (newTeamId) {
          // Reload teams to get the complete data
          await this.loadTeams();
          this.ns.success('Equipo creado', 'Equipos');
        }
      }

      this.closeForm();
    } catch (err: any) {
      console.error('Failed to save team:', err);
      this.error.set(err.message || 'Failed to save team');
      this.ns.error(err?.message ?? 'No se pudo guardar el equipo', 'Error');
    }
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedTeam.set(null);
  }
}
