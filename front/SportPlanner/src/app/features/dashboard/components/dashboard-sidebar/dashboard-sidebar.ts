import { Component, input, inject } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-dashboard-sidebar',
  imports: [TranslateModule],
  templateUrl: './dashboard-sidebar.html'
})
export class DashboardSidebar {
  // Services
  private router = inject(Router);

  // Inputs
  isCollapsed = input<boolean>(false);

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

  goToMessages() {
    this.router.navigate(['/dashboard/messages']);
  }

  goToSettings() {
    this.router.navigate(['/dashboard/settings']);
  }

  goToHelp() {
    this.router.navigate(['/dashboard/help']);
  }
}
