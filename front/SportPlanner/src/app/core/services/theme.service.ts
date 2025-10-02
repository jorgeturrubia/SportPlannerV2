import { Injectable, signal, effect } from '@angular/core';

export type Theme = 'light' | 'dark' | 'system';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  // Signals
  private readonly THEME_KEY = 'sportplanner-theme';
  theme = signal<Theme>(this.getInitialTheme());
  isDarkMode = signal<boolean>(false);

  constructor() {
    // Initialize theme on service creation
    this.applyTheme();

    // Listen to system theme changes
    if (typeof window !== 'undefined') {
      const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
      mediaQuery.addEventListener('change', () => {
        if (this.theme() === 'system') {
          this.applyTheme();
        }
      });
    }

    // Effect to persist theme changes
    effect(() => {
      const currentTheme = this.theme();
      if (typeof window !== 'undefined') {
        localStorage.setItem(this.THEME_KEY, currentTheme);
      }
      this.applyTheme();
    });
  }

  /**
   * Get initial theme from localStorage or default to 'system'
   */
  private getInitialTheme(): Theme {
    if (typeof window === 'undefined') {
      return 'system';
    }

    const savedTheme = localStorage.getItem(this.THEME_KEY) as Theme;
    if (savedTheme && ['light', 'dark', 'system'].includes(savedTheme)) {
      return savedTheme;
    }

    return 'system';
  }

  /**
   * Check if dark mode should be active based on current theme
   */
  private shouldUseDarkMode(): boolean {
    const currentTheme = this.theme();

    if (currentTheme === 'dark') {
      return true;
    }

    if (currentTheme === 'light') {
      return false;
    }

    // System theme
    if (typeof window !== 'undefined') {
      return window.matchMedia('(prefers-color-scheme: dark)').matches;
    }

    return false;
  }

  /**
   * Apply theme to document
   */
  private applyTheme(): void {
    if (typeof document === 'undefined') {
      return;
    }

    const darkMode = this.shouldUseDarkMode();
    this.isDarkMode.set(darkMode);

    if (darkMode) {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
  }

  /**
   * Set theme and apply it
   */
  setTheme(theme: Theme): void {
    this.theme.set(theme);
  }

  /**
   * Toggle between light and dark mode (ignores system)
   */
  toggleTheme(): void {
    const currentTheme = this.theme();
    
    if (currentTheme === 'system') {
      // If currently using system, toggle based on current appearance
      this.setTheme(this.isDarkMode() ? 'light' : 'dark');
    } else {
      // Toggle between light and dark
      this.setTheme(currentTheme === 'dark' ? 'light' : 'dark');
    }
  }

  /**
   * Get the current effective theme (resolves 'system' to actual theme)
   */
  getEffectiveTheme(): 'light' | 'dark' {
    return this.isDarkMode() ? 'dark' : 'light';
  }
}
