import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  imports: [CommonModule],
  selector: 'app-teams-blank',
  template: `
    <div class="p-6">
      <h1 class="text-2xl font-bold">Teams</h1>
      <p class="mt-2 text-slate-500">Página en blanco — aquí iremos dando forma.</p>
    </div>
  `
})
export class TeamsBlankComponent {}
