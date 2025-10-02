import { Component, signal, inject } from '@angular/core';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { Navbar } from './features/shared/components/navbar/navbar';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs';
import { LanguageService } from './services/language.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('SportPlanner');
  showNavbar = signal(true);
  
  // Inyectar LanguageService para inicializarlo
  private languageService = inject(LanguageService);

  constructor(private router: Router) {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        // Ocultar navbar en las rutas /auth y /dashboard
        this.showNavbar.set(!event.url.includes('/auth') && !event.url.includes('/dashboard'));
      });
  }
}
