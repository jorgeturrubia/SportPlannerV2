import { Component, input } from '@angular/core';

@Component({
  selector: 'app-dashboard-sidebar',
  imports: [],
  templateUrl: './dashboard-sidebar.html'
})
export class DashboardSidebar {
  isCollapsed = input<boolean>(false);
}
