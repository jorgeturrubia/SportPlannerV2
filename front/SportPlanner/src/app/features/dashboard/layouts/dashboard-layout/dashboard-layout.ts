import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DashboardNavbar } from '../../components/dashboard-navbar/dashboard-navbar';
import { DashboardSidebar } from '../../components/dashboard-sidebar/dashboard-sidebar';

@Component({
  selector: 'app-dashboard-layout',
  imports: [RouterOutlet, DashboardNavbar, DashboardSidebar],
  templateUrl: './dashboard-layout.html'
})
export class DashboardLayout {
  isSidebarCollapsed = signal(false);

  toggleSidebar() {
    this.isSidebarCollapsed.update(val => !val);
  }
}
