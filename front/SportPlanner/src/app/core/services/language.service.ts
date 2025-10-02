import { Injectable, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

export type Language = 'es' | 'en' | 'fr' | 'de';

export interface LanguageInfo {
  code: Language;
  name: string;
  nativeName: string;
  flag: string;
}

@Injectable({
  providedIn: 'root'
})
export class LanguageService {
  private readonly LANGUAGE_KEY = 'sportplanner-language';
  
  // Available languages
  readonly languages: LanguageInfo[] = [
    { code: 'es', name: 'Spanish', nativeName: 'Español', flag: '🇪🇸' },
    { code: 'en', name: 'English', nativeName: 'English', flag: '🇬🇧' },
    { code: 'fr', name: 'French', nativeName: 'Français', flag: '🇫🇷' },
    { code: 'de', name: 'German', nativeName: 'Deutsch', flag: '🇩🇪' }
  ];

  // Current language signal
  currentLanguage = signal<Language>(this.getInitialLanguage());

  constructor(private translate: TranslateService) {
    // Set default language
    this.translate.setDefaultLang('es');
    
    // Use saved or detected language
    this.setLanguage(this.currentLanguage());
  }

  /**
   * Get initial language from localStorage or browser
   */
  private getInitialLanguage(): Language {
    if (typeof window === 'undefined') {
      return 'es';
    }

    // Try to get from localStorage
    const saved = localStorage.getItem(this.LANGUAGE_KEY) as Language;
    if (saved && this.isValidLanguage(saved)) {
      return saved;
    }

    // Try to detect from browser
    const browserLang = this.translate.getBrowserLang();
    if (browserLang && this.isValidLanguage(browserLang)) {
      return browserLang as Language;
    }

    // Default to Spanish
    return 'es';
  }

  /**
   * Check if language code is valid
   */
  private isValidLanguage(lang: string): boolean {
    return this.languages.some(l => l.code === lang);
  }

  /**
   * Set the current language
   */
  setLanguage(lang: Language): void {
    if (!this.isValidLanguage(lang)) {
      console.warn(`Invalid language: ${lang}. Using default.`);
      lang = 'es';
    }

    this.currentLanguage.set(lang);
    this.translate.use(lang);
    
    if (typeof window !== 'undefined') {
      localStorage.setItem(this.LANGUAGE_KEY, lang);
      // Update HTML lang attribute for accessibility
      document.documentElement.lang = lang;
    }
  }

  /**
   * Get language info by code
   */
  getLanguageInfo(code: Language): LanguageInfo | undefined {
    return this.languages.find(l => l.code === code);
  }

  /**
   * Get current language info
   */
  getCurrentLanguageInfo(): LanguageInfo {
    return this.getLanguageInfo(this.currentLanguage()) || this.languages[0];
  }

  /**
   * Translate a key instantly (synchronous)
   */
  instant(key: string, params?: any): string {
    return this.translate.instant(key, params);
  }

  /**
   * Translate a key (asynchronous - returns Observable)
   */
  get(key: string, params?: any) {
    return this.translate.get(key, params);
  }
}
