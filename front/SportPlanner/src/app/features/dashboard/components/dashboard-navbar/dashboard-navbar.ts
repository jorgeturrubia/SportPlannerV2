import { Component, signal, computed, output } from '@angular/core';

@Component({
  selector: 'app-dashboard-navbar',
  imports: [],
  templateUrl: './dashboard-navbar.html'
})
export class DashboardNavbar {
  // Outputs
  toggleSidebar = output<void>();

  // Signals
  isDarkMode = signal(false);
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
  toggleTheme() {
    this.isDarkMode.update(val => !val);
    // TODO: Implement theme switching logic
  }

  toggleUserMenu() {
    this.userMenuOpen.update(val => !val);
  }
}
