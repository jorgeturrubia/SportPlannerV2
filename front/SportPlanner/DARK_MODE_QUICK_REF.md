# 🌓 Dark Mode Quick Reference

## Para agregar Dark Mode a un nuevo componente:

### 1. Fondos
```html
class="bg-white dark:bg-slate-800"
```

### 2. Textos
```html
class="text-slate-900 dark:text-slate-100"
```

### 3. Bordes
```html
class="border-slate-200 dark:border-slate-700"
```

### 4. Hover
```html
class="hover:bg-slate-100 dark:hover:bg-slate-700"
```

### 5. Transiciones
```html
class="transition-colors duration-200"
```

## Usar el ThemeService

```typescript
import { ThemeService } from '@core/services/theme.service';

export class MyComponent {
  themeService = inject(ThemeService);
  
  toggleTheme() {
    this.themeService.toggleTheme();
  }
}
```

## Ver más
- `DARK_MODE_GUIDE.md` - Guía completa
- `DARK_MODE_IMPLEMENTATION.md` - Detalles de implementación
