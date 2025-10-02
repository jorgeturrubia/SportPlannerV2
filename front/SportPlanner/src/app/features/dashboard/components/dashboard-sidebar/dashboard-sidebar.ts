import { Component, input, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard-sidebar',
  imports: [],
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
}
