import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../../../shared/components/card/card.component';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { CardCarouselComponent } from '../../../../shared/components/card-carousel/card-carousel.component';
import { DynamicFormComponent, FormField } from '../../../../shared/components/dynamic-form/dynamic-form.component';

interface Team {
  id: number;
  name: string;
  sport: string;
  description: string;
  members: number;
  avatar: string;
}

@Component({
  selector: 'app-teams',
  standalone: true,
  imports: [CommonModule, CardComponent, ConfirmationDialogComponent, DynamicFormComponent, CardCarouselComponent],
  templateUrl: './teams.html',
  styleUrls: ['./teams.css']
})
export class TeamsPage {
  teams = signal<Team[]>([
    {
      id: 1,
      name: 'Team Alpha',
      sport: 'Basketball',
      description: 'A team of dedicated basketball players.',
      members: 12,
      avatar: '🏀'
    },
    {
      id: 2,
      name: 'Team Bravo',
      sport: 'Football',
      description: 'The best football team in the region.',
      members: 18,
      avatar: '⚽'
    },
    {
      id: 3,
      name: 'Team Charlie',
      sport: 'Tennis',
      description: 'A group of talented tennis players.',
      members: 8,
      avatar: '🎾'
    },
    {
      id: 4,
      name: 'Team Delta',
      sport: 'Volleyball',
      description: 'Competitive volleyball team with strong teamwork.',
      members: 14,
      avatar: '🏐'
    },
    {
      id: 5,
      name: 'Team Echo',
      sport: 'Swimming',
      description: 'Elite swimmers training for championships.',
      members: 10,
      avatar: '🏊'
    },
    {
      id: 6,
      name: 'Team Foxtrot',
      sport: 'Baseball',
      description: 'Professional baseball team with great batting lineup.',
      members: 20,
      avatar: '⚾'
    }
  ]);

  // Dialog and Form state
  isConfirmDialogOpen = signal(false);
  isFormOpen = signal(false);
  selectedTeam = signal<Team | null>(null);
  formTitle = 'Add New Team';

  teamFormConfig: FormField[] = [
    { key: 'name', label: 'Team Name', type: 'text', required: true },
    { key: 'sport', label: 'Sport', type: 'text', required: true },
    { key: 'description', label: 'Description', type: 'textarea' },
    { key: 'members', label: 'Members', type: 'number' }
  ];

  // --- Delete Logic ---
  openDeleteConfirm(team: Team): void {
    this.selectedTeam.set(team);
    this.isConfirmDialogOpen.set(true);
  }

  handleDeleteConfirm(): void {
    const teamToDelete = this.selectedTeam();
    if (teamToDelete) {
      this.teams.update(teams => teams.filter(t => t.id !== teamToDelete.id));
    }
    this.closeConfirmDialog();
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

  openEditForm(team: Team): void {
    this.selectedTeam.set(team);
    this.formTitle = `Edit ${team.name}`;
    this.isFormOpen.set(true);
  }

  handleFormSubmit(formData: any): void {
    const selected = this.selectedTeam();
    if (selected) {
      // Update existing team
      this.teams.update(teams =>
        teams.map(t => t.id === selected.id ? { ...t, ...formData } : t)
      );
    } else {
      // Add new team
      const newTeam: Team = {
        id: Date.now(), // simple id generation
        ...formData,
        avatar: '✨' // default avatar
      };
      this.teams.update(teams => [...teams, newTeam]);
    }
    this.closeForm();
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedTeam.set(null);
  }
}
