import { Injectable, signal, effect, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';

export type SupportedLanguage = 'es' | 'en' | 'fr' | 'de';

@Injectable({
  providedIn: 'root'
})
export class LanguageService {
  private platformId = inject(PLATFORM_ID);
  private isBrowser = isPlatformBrowser(this.platformId);
  
  // Available languages
  readonly availableLanguages: SupportedLanguage[] = ['es', 'en', 'fr', 'de'];
  
  // Current language signal
  readonly currentLanguage = signal<SupportedLanguage>('es');

  // Language names for display
  readonly languageNames: Record<SupportedLanguage, string> = {
    es: 'Español',
    en: 'English',
    fr: 'Français',
    de: 'Deutsch'
  };

  constructor(private translate: TranslateService) {
    // Set available languages
    this.translate.addLangs(this.availableLanguages);
    
    // Set default language
    this.translate.setDefaultLang('es');
    
    // Detect and set initial language
    const savedLang = this.getSavedLanguage();
    const browserLang = this.translate.getBrowserLang() as SupportedLanguage;
    const initialLang = savedLang || 
                       (this.availableLanguages.includes(browserLang) ? browserLang : 'es');
    
    // Use the language immediately
    this.translate.use(initialLang);
    this.currentLanguage.set(initialLang);

    // Save language to localStorage whenever it changes (only in browser)
    effect(() => {
      if (this.isBrowser) {
        const lang = this.currentLanguage();
        localStorage.setItem('sportplanner-language', lang);
      }
    });
  }

  /**
   * Change the current language
   */
  setLanguage(lang: SupportedLanguage): void {
    if (this.availableLanguages.includes(lang)) {
      // Primero cargamos las traducciones, LUEGO actualizamos el signal
      this.translate.use(lang).subscribe({
        next: () => {
          console.log('Language changed to:', lang);
          // Actualizar el signal DESPUÉS de que las traducciones estén cargadas
          this.currentLanguage.set(lang);
        },
        error: (err) => {
          console.error('Error loading translations:', err);
        }
      });
    }
  }

  /**
   * Get translation for a key
   */
  get(key: string | string[], params?: any) {
    return this.translate.get(key, params);
  }

  /**
   * Get instant translation for a key
   */
  instant(key: string | string[], params?: any): string {
    return this.translate.instant(key, params);
  }

  /**
   * Get language saved in localStorage
   */
  private getSavedLanguage(): SupportedLanguage | null {
    if (!this.isBrowser) {
      return null;
    }
    const saved = localStorage.getItem('sportplanner-language');
    return saved && this.availableLanguages.includes(saved as SupportedLanguage)
      ? (saved as SupportedLanguage)
      : null;
  }
}
