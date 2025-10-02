import { Component, signal, computed, output, inject, effect } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ThemeService } from '../../../../core/services/theme.service';
import { LanguageService, SupportedLanguage } from '../../../../services/language.service';

@Component({
  selector: 'app-dashboard-navbar',
  imports: [TranslateModule],
  templateUrl: './dashboard-navbar.html'
})
export class DashboardNavbar {
  // Services
  private router = inject(Router);
  private translateService = inject(TranslateService);
  protected themeService = inject(ThemeService);
  protected languageService = inject(LanguageService);

  // Outputs
  toggleSidebar = output<void>();

  // Signals
  userMenuOpen = signal(false);
  languageMenuOpen = signal(false);
  userName = signal('Coach Pro');
  // store role key (will be translated in template)
  userRole = signal<'coach' | 'athlete' | 'admin'>('coach');
  userEmail = signal('coach@sportplanner.com');

  // Translation signals that update when language changes
  searchPlaceholder = signal('Buscar...');
  myProfileText = signal('Mi Perfil');
  settingsText = signal('Configuración');
  logoutText = signal('Cerrar Sesión');

  constructor() {
    // Esperar a que las traducciones estén listas antes de actualizar
    this.translateService.onDefaultLangChange.subscribe(() => {
      console.log('Default language loaded, updating translations');
      this.updateTranslations();
    });

    // Update translations whenever language changes
    effect(() => {
      const currentLang = this.languageService.currentLanguage();
      console.log('Language changed in navbar, updating translations:', currentLang);
      
      // Esperar un tick para asegurar que las traducciones estén cargadas
      setTimeout(() => {
        this.updateTranslations();
        console.log('Translations updated to:', {
          search: this.searchPlaceholder(),
          profile: this.myProfileText(),
          settings: this.settingsText(),
          logout: this.logoutText()
        });
      }, 100);
    });

    // Update translations when user menu opens (important for @if blocks)
    effect(() => {
      if (this.userMenuOpen()) {
        console.log('User menu opened, updating translations');
        setTimeout(() => this.updateTranslations(), 50);
      }
    });

    // Subscribe to translation changes - THIS IS THE KEY ONE
    this.translateService.onLangChange.subscribe(() => {
      console.log('TranslateService.onLangChange triggered');
      setTimeout(() => this.updateTranslations(), 100);
    });
  }

  private updateTranslations(): void {
    const search = this.translateService.instant('common.search');
    const profile = this.translateService.instant('user.myProfile');
    const settings = this.translateService.instant('user.settings');
    const logout = this.translateService.instant('user.logout');
    
    console.log('Getting translations:', { search, profile, settings, logout });
    
    this.searchPlaceholder.set(search);
    this.myProfileText.set(profile);
    this.settingsText.set(settings);
    this.logoutText.set(logout);
  }

  // Computed
  userInitials = computed(() => {
    const name = this.userName();
    const parts = name.split(' ');
    if (parts.length >= 2) {
      return parts[0][0] + parts[1][0];
    }
    return parts[0][0] + (parts[0][1] || '');
  });

  // Language flags emoji map
  languageFlags: Record<SupportedLanguage, string> = {
    es: '🇪🇸',
    en: '🇬🇧',
    fr: '🇫🇷',
    de: '🇩🇪'
  };

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

  toggleLanguageMenu() {
    this.languageMenuOpen.update(val => !val);
  }

  changeLanguage(lang: SupportedLanguage) {
    console.log('Changing language to:', lang);
    this.languageService.setLanguage(lang);
    this.languageMenuOpen.set(false);
  }

  // Getter para forzar la reactividad del idioma actual
  get currentLang() {
    return this.languageService.currentLanguage();
  }
}
