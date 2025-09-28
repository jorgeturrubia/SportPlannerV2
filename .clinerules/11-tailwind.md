# Tailwind CSS v4

## Configuración de Tema
- **Tema único**: Configurar SOLO en archivo `@theme`
- **CSS Variables**: Usar custom properties para valores dinámicos
- **No Global Styles**: Evitar estilos globales fuera del tema

```css
/* @theme configuration */
@theme {
  --color-primary-50: #eff6ff;
  --color-primary-500: #3b82f6;
  --color-primary-900: #1e3a8a;
  
  --spacing-xs: 0.5rem;
  --spacing-sm: 1rem;
  --spacing-md: 1.5rem;
}
```

## Responsive Design
- **Container Queries**: Preferir sobre media queries cuando sea posible
- **Grid System**: Usar CSS Grid con Tailwind utilities
- **Breakpoints**: Mobile-first approach

```html
<!-- Ejemplo: Responsive Grid -->
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
  <div class="bg-white p-4 rounded-lg shadow">...</div>
</div>
```

## Import Strategy
- **Single Import**: Solo `import "tailwindcss"` en main
- **No Media Queries**: Evitar `@media` queries globales custom
- **Component Styles**: Usar @apply con moderación

```scss
// main.scss
@import "tailwindcss";

// Component-specific (solo si es necesario)
.training-card {
  @apply bg-white rounded-lg shadow-md p-6;
}
```

## Utility Classes Organization
```html
<!-- Estructura recomendada de clases -->
<div class="
  <!-- Layout -->
  flex flex-col gap-4
  <!-- Spacing -->
  p-6 m-4
  <!-- Colors -->
  bg-white text-gray-900
  <!-- Typography -->
  text-lg font-semibold
  <!-- Interactive -->
  hover:bg-gray-50 focus:ring-2
  <!-- Responsive -->
  md:flex-row lg:gap-6
">
```

## Componentes Específicos SportPlanner
```html
<!-- Training Card -->
<div class="bg-white rounded-xl shadow-sm border border-gray-200 
           hover:shadow-md transition-shadow duration-200
           p-6 space-y-4">
  
<!-- Dashboard Grid -->
<div class="grid grid-cols-1 lg:grid-cols-3 gap-6
           auto-rows-min">
  
<!-- Form Layout -->
<form class="space-y-6 max-w-md mx-auto">
  <div class="space-y-2">
    <label class="text-sm font-medium text-gray-700">
    <input class="w-full px-3 py-2 border border-gray-300 
                  rounded-md focus:ring-primary-500 
                  focus:border-primary-500">
  </div>
</form>
```

## Performance y Optimización
- **PurgeCSS**: Configurar para eliminar clases no usadas
- **Tree Shaking**: Optimización automática en build
- **Critical CSS**: Inline critical styles para LCP

## Design System Integration
- **Tokens**: Usar design tokens en tema
- **Spacing Scale**: Mantener escala consistente
- **Color Palette**: Definir paleta semántica

```css
/* Design Tokens Example */
@theme {
  /* Primary Colors */
  --color-primary: var(--color-blue-600);
  --color-secondary: var(--color-indigo-600);
  
  /* Semantic Colors */
  --color-success: var(--color-green-600);
  --color-warning: var(--color-yellow-600);
  --color-error: var(--color-red-600);
  
  /* Spacing */
  --spacing-section: 4rem;
  --spacing-component: 1.5rem;
  --spacing-element: 0.75rem;
}
```

## Dark Mode Support
```html
<!-- Dark mode classes -->
<div class="bg-white dark:bg-gray-900 
           text-gray-900 dark:text-white
           border-gray-200 dark:border-gray-700">
```
