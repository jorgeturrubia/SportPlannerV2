import { Component, input, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard-sidebar',
  imports: [TranslateModule, CommonModule],
  templateUrl: './dashboard-sidebar.html'
})
export class DashboardSidebar {
  // Services
  private router = inject(Router);

  // Inputs
  isCollapsed = input<boolean>(false);

  // State
  designMenuOpen = signal(false);
  masterDataMenuOpen = signal(false);

  // Methods
  goToHome() {
    this.router.navigate(['/dashboard']);
  }

  goToTeams() {
    this.router.navigate(['/dashboard/teams']);
  }

  goToCalendar() {
    this.router.navigate(['/dashboard/calendar']);
  }

  goToObjectives() {
    this.router.navigate(['/dashboard/objectives']);
  }

  goToMessages() {
    this.router.navigate(['/dashboard/messages']);
  }

  goToSettings() {
    this.router.navigate(['/dashboard/settings']);
  }

  goToHelp() {
    this.router.navigate(['/dashboard/help']);
  }

  toggleDesignMenu() {
    this.designMenuOpen.update(open => !open);
  }

  goToDesignTables() {
    this.router.navigate(['/dashboard/design/tables']);
  }

  goToDesignCards() {
    this.router.navigate(['/dashboard/design/cards']);
  }

  goToDesignForms() {
    this.router.navigate(['/dashboard/design/forms']);
  }

  toggleMasterDataMenu() {
    this.masterDataMenuOpen.update(open => !open);
  }

  goToGenders() {
    this.router.navigate(['/dashboard/master-data/genders']);
  }

  goToAgeGroups() {
    this.router.navigate(['/dashboard/master-data/age-groups']);
  }

  goToCategories() {
    this.router.navigate(['/dashboard/master-data/categories']);
  }

  goToObjectiveCategories() {
    this.router.navigate(['/dashboard/master-data/objective-categories']);
  }

  goToObjectiveSubcategories() {
    this.router.navigate(['/dashboard/master-data/objective-subcategories']);
  }

  goToExerciseCategories() {
    this.router.navigate(['/dashboard/master-data/exercise-categories']);
  }

  goToExerciseTypes() {
    this.router.navigate(['/dashboard/master-data/exercise-types']);
  }

  goToTrainingPlans() {
    this.router.navigate(['/dashboard/training-plans']);
  }

  goToExercises() {
    this.router.navigate(['/dashboard/exercises']);
  }

  goToWorkouts() {
    this.router.navigate(['/dashboard/workouts']);
  }
}
