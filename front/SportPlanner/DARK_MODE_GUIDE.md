# 🌓 Guía de Dark Mode - SportPlanner

## Introducción

SportPlanner implementa un sistema completo de temas Dark/Light Mode usando Tailwind CSS v4 y Angular Signals. El tema se persiste en `localStorage` y puede responder a las preferencias del sistema operativo.

## 🎨 Arquitectura

### ThemeService

El servicio central de gestión de temas está ubicado en:
```
src/app/core/services/theme.service.ts
```

**Características:**
- ✅ Gestión de estado con Angular Signals
- ✅ Persistencia en localStorage
- ✅ Soporte para tema del sistema (auto)
- ✅ Tres modos: `light`, `dark`, `system`
- ✅ Sincronización automática con `matchMedia`

### Configuración de Tailwind CSS v4

En `src/styles.css`:
```css
@import "tailwindcss";

@custom-variant dark (&:where(.dark, .dark *));
```

Esta configuración permite usar la clase `.dark` en el elemento `<html>` para activar todos los estilos dark mode.

## 🚀 Uso en Componentes

### 1. Inyectar el ThemeService (Opcional)

Solo necesitas inyectar el servicio si quieres controlar el tema programáticamente:

```typescript
import { ThemeService } from '@core/services/theme.service';

export class MyComponent {
  protected themeService = inject(ThemeService);

  toggleTheme() {
    this.themeService.toggleTheme();
  }

  setDarkMode() {
    this.themeService.setTheme('dark');
  }
}
```

### 2. Aplicar Estilos Dark Mode en Templates

Usa el modificador `dark:` de Tailwind CSS para todos los estilos que deban cambiar en modo oscuro:

```html
<!-- Ejemplo básico -->
<div class="bg-white dark:bg-slate-900 text-slate-900 dark:text-slate-100">
  Contenido adaptable
</div>

<!-- Ejemplo con bordes -->
<div class="border border-slate-200 dark:border-slate-700">
  Con bordes adaptativos
</div>

<!-- Ejemplo con hover -->
<button class="hover:bg-slate-100 dark:hover:bg-slate-800">
  Botón con hover adaptativo
</button>
```

## 📋 Patrón de Colores Recomendado

### Fondos

| Elemento | Light Mode | Dark Mode |
|----------|-----------|-----------|
| Fondo principal | `bg-slate-50` | `dark:bg-slate-950` |
| Tarjetas/Cards | `bg-white` | `dark:bg-slate-800` |
| Elementos elevados | `bg-white` | `dark:bg-slate-900` |
| Hover suave | `hover:bg-slate-50` | `dark:hover:bg-slate-800` |
| Hover secundario | `hover:bg-slate-100` | `dark:hover:bg-slate-700` |

### Textos

| Tipo de Texto | Light Mode | Dark Mode |
|---------------|-----------|-----------|
| Texto principal | `text-slate-900` | `dark:text-slate-100` |
| Texto secundario | `text-slate-600` | `dark:text-slate-400` |
| Texto terciario | `text-slate-500` | `dark:text-slate-500` |
| Placeholders | `placeholder-slate-400` | `dark:placeholder-slate-500` |

### Bordes

| Elemento | Light Mode | Dark Mode |
|----------|-----------|-----------|
| Borde principal | `border-slate-200` | `dark:border-slate-700` |
| Borde secundario | `border-slate-100` | `dark:border-slate-800` |

### Colores de Acento

Los colores de acento (azul, verde, púrpura, etc.) deben ajustarse ligeramente:

| Color | Light Mode | Dark Mode |
|-------|-----------|-----------|
| Azul | `text-blue-600` | `dark:text-blue-400` |
| Verde | `text-green-600` | `dark:text-green-400` |
| Púrpura | `text-purple-600` | `dark:text-purple-400` |
| Naranja | `text-orange-600` | `dark:text-orange-400` |
| Rojo | `text-red-600` | `dark:text-red-400` |

### Fondos de Acento (con transparencia)

```html
<!-- Azul -->
<div class="bg-blue-100 dark:bg-blue-900/30">

<!-- Verde -->
<div class="bg-green-100 dark:bg-green-900/30">

<!-- Púrpura -->
<div class="bg-purple-100 dark:bg-purple-900/30">
```

## ✅ Checklist para Nuevos Componentes

Cuando crees un nuevo componente, asegúrate de:

- [ ] Agregar `dark:` variants para todos los colores de fondo
- [ ] Agregar `dark:` variants para todos los colores de texto
- [ ] Agregar `dark:` variants para todos los bordes
- [ ] Agregar `dark:` variants para estados hover/focus/active
- [ ] Agregar `dark:` variants para sombras si las usas
- [ ] Agregar `dark:` variants para gradientes si los usas
- [ ] Probar visualmente en ambos modos
- [ ] Agregar `transition-colors` para transiciones suaves

## 🎯 Ejemplo Completo: Card Component

```html
<div class="
  bg-white dark:bg-slate-800 
  border border-slate-200 dark:border-slate-700 
  rounded-xl 
  p-6 
  hover:shadow-lg 
  transition-all
">
  <!-- Header -->
  <div class="flex items-center justify-between mb-4">
    <h3 class="text-lg font-bold text-slate-900 dark:text-slate-100">
      Título
    </h3>
    <span class="text-xs text-slate-500 dark:text-slate-400">
      Hace 2 horas
    </span>
  </div>

  <!-- Content -->
  <p class="text-slate-600 dark:text-slate-300 mb-4">
    Contenido de la tarjeta con texto adaptativo
  </p>

  <!-- Badge -->
  <span class="
    inline-flex 
    items-center 
    px-3 
    py-1 
    rounded-full 
    text-sm 
    font-medium 
    bg-blue-100 dark:bg-blue-900/30 
    text-blue-600 dark:text-blue-400
  ">
    Estado
  </span>

  <!-- Button -->
  <button class="
    w-full 
    mt-4 
    py-2 
    bg-blue-600 dark:bg-blue-500 
    hover:bg-blue-700 dark:hover:bg-blue-600 
    text-white 
    rounded-lg 
    transition-colors
  ">
    Acción
  </button>
</div>
```

## 🔧 API del ThemeService

### Propiedades (Signals)

```typescript
themeService.theme() // 'light' | 'dark' | 'system'
themeService.isDarkMode() // boolean
```

### Métodos

```typescript
// Cambiar tema
themeService.setTheme('dark')
themeService.setTheme('light')
themeService.setTheme('system')

// Toggle entre light/dark
themeService.toggleTheme()

// Obtener tema efectivo (resuelve 'system')
themeService.getEffectiveTheme() // 'light' | 'dark'
```

## 💡 Tips y Mejores Prácticas

1. **Siempre incluye transiciones**: Agrega `transition-colors` o `transition-all` para cambios suaves
2. **Prueba ambos modos**: Asegúrate de que todo sea legible en ambos temas
3. **Usa transparencia para acentos**: En dark mode, usa `/30` u `/40` de opacidad para fondos de acento
4. **Mantén consistencia**: Sigue la tabla de colores recomendada
5. **No olvides los SVGs**: Los iconos también necesitan variants de color
6. **Estados interactivos**: Hover, focus y active también necesitan dark variants
7. **Contraste adecuado**: Asegúrate de que haya suficiente contraste en ambos modos

## 🐛 Troubleshooting

### El dark mode no se aplica
- Verifica que `styles.css` tenga la configuración `@custom-variant`
- Asegúrate de que el HTML tiene la clase `.dark` cuando debería
- Revisa la consola del navegador por errores

### Los colores no se ven bien
- Ajusta la intensidad de los colores (600 en light, 400 en dark)
- Usa transparencia en fondos de acento en dark mode
- Verifica el contraste con herramientas de accesibilidad

### El tema no persiste
- Verifica que localStorage esté habilitado
- Revisa que el ThemeService se esté inicializando correctamente
- Asegúrate de que el app no esté en modo SSR sin guards

## 📚 Referencias

- [Tailwind CSS v4 Dark Mode Documentation](https://tailwindcss.com/docs/dark-mode)
- [Angular Signals Documentation](https://angular.dev/guide/signals)
- [Web Accessibility Color Contrast](https://webaim.org/resources/contrastchecker/)

---

**Última actualización:** Octubre 2025
