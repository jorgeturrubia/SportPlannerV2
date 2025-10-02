import { Component, signal, computed, output, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ThemeService } from '../../../../core/services/theme.service';

@Component({
  selector: 'app-dashboard-navbar',
  imports: [],
  templateUrl: './dashboard-navbar.html'
})
export class DashboardNavbar {
  // Services
  private router = inject(Router);
  protected themeService = inject(ThemeService);

  // Outputs
  toggleSidebar = output<void>();

  // Signals
  userMenuOpen = signal(false);
  userName = signal('Coach Pro');
  userRole = signal('Entrenador');
  userEmail = signal('coach@sportplanner.com');

  // Computed
  userInitials = computed(() => {
    const name = this.userName();
    const parts = name.split(' ');
    if (parts.length >= 2) {
      return parts[0][0] + parts[1][0];
    }
    return parts[0][0] + (parts[0][1] || '');
  });

  // Methods
  goToHome() {
    this.router.navigate(['/dashboard']);
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }

  toggleUserMenu() {
    this.userMenuOpen.update(val => !val);
  }
}
