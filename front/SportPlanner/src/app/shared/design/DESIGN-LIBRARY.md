# 🎨 SportPlanner Design Library

## 📂 Estructura de la Carpeta

```
src/app/shared/design/
├── README.md                           # Documentación principal
├── USAGE-EXAMPLE.md                    # Ejemplos de uso detallados
├── DESIGN-LIBRARY.md                   # Este archivo (resumen general)
├── index.ts                            # Exports principales
├── sport-demo-tables.component.ts      # Componente de demostración
├── sport-demo-tables.component.html    # Template del componente
└── sport-demo-tables.component.css     # Estilos (todas las 7 vistas)
```

## 🌟 Componentes Disponibles

### 1. SportDemoTablesComponent

**Archivo**: `sport-demo-tables.component.ts`

**Descripción**: Componente showcase con 7 vistas innovadoras para visualizar datos en formato de equipos/grupos.

**Vistas incluidas**:

| Vista | Descripción | Características Destacadas |
|-------|-------------|---------------------------|
| **Grid** | Cuadrícula clásica | Tarjetas organizadas, responsive, hover effects |
| **Board** | Tablero horizontal | Tipo Kanban, scroll horizontal, avatares apilados |
| **DataTable** ⭐ | Tabla avanzada | Búsqueda, ordenación, expansión, badges, mini gráficos |
| **Carousel** | Carrusel 3D | Perspectiva 3D, rotación, profundidad, transiciones |
| **Timeline** | Línea de tiempo | Vista cronológica, conectores, alternancia arriba/abajo |
| **Hexagon** | Patrón hexagonal | Panal de abejas, clip-path, hover overlay |
| **Galaxy** 🚀 | Sistema solar | Planetas orbitando, sol animado, física orbital, panel lateral |

## 🎯 Vista Galaxy (Innovación Principal)

La vista **Galaxy** es completamente única y revolucionaria:

### Características

- ☀️ **Sol Central**: Animado con llamaradas solares y corona rotante
- 🪐 **Planetas Orbitando**: Cada equipo es un planeta con órbita real
- 📏 **Tamaño Dinámico**: Proporcional a la capacidad del equipo
- 🎨 **Colores Personalizados**: Gradientes basados en el color del equipo
- 🌍 **Texturas Planetarias**: Superficies animadas con rotación independiente
- 💍 **Anillos**: Para equipos con >10 miembros
- ✨ **Estrellas**: Fondo espacial con estrellas parpadeantes
- 🎯 **Interactivo**: Click en planeta abre panel lateral con detalles
- 📊 **Panel Lateral**: Estadísticas completas, lista de miembros, acciones
- 🌑 **Satélites**: Mini lunas orbitando algunos planetas

### Tecnología

- CSS animations puras (sin librerías externas)
- Transform 3D para efectos de profundidad
- Keyframes personalizados para órbitas
- Backdrop filter para efectos glassmorphism
- Gradientes radiales para atmósferas

## 📦 Instalación y Uso

### Importación

```typescript
import { SportDemoTablesComponent } from '@shared/design';
// O
import { SportDemoTablesComponent } from '@shared/design/sport-demo-tables.component';
```

### Uso Básico

```html
<app-sport-demo-tables
  [items]="myTeams()"
  [viewMode]="'galaxy'"
  [maxCapacity]="15"
  (onEdit)="handleEdit($event)"
  (onDelete)="handleDelete($event)">
</app-sport-demo-tables>
```

**📖 Ver ejemplos completos**: [USAGE-EXAMPLE.md](./USAGE-EXAMPLE.md)

## 🎨 Personalización de Estilos

Todos los estilos están en `sport-demo-tables.component.css` organizados por vista:

```css
/* Vista Grid */
.team-card { ... }

/* Vista DataTable */
.datatable-container { ... }

/* Vista Galaxy */
.galaxy-container { ... }
.sun { ... }
.planet-surface { ... }

/* etc. */
```

### Sobrescribir estilos

```scss
::ng-deep app-sport-demo-tables {
  .datatable-container {
    // Tus personalizaciones
  }
}
```

## 🔧 API del Componente

### Inputs

| Input | Tipo | Default | Descripción |
|-------|------|---------|-------------|
| `items` | `any[]` | `[]` | Datos a visualizar |
| `viewMode` | `ViewMode` | `'grid'` | Vista activa |
| `maxCapacity` | `number` | `15` | Capacidad máxima por item |
| `showCreateButton` | `boolean` | `true` | Mostrar botón crear |

### Outputs

| Output | Payload | Descripción |
|--------|---------|-------------|
| `onEdit` | `item: any` | Click en editar |
| `onDelete` | `item: any` | Click en eliminar |
| `onCreate` | `void` | Click en crear |
| `onSelect` | `item: any` | Selección de item (Galaxy) |
| `viewModeChange` | `mode: string` | Cambio de vista |

### Métodos Públicos

- `toggleRow(itemId: string)` - Expandir/colapsar fila (DataTable)
- `sortBy(column: string)` - Ordenar por columna (DataTable)
- `selectPlanet(item: any)` - Seleccionar planeta (Galaxy)
- `closePlanetDetail()` - Cerrar panel (Galaxy)

## 📊 Estructura de Datos

```typescript
interface DemoTableItem {
  id: string | number;
  name: string;
  color: string;  // HEX color code
  description?: string;
  members: Array<{
    id: string | number;
    name: string;
  }>;
  createdAt: Date | string;
}
```

## 🎯 Casos de Uso

1. **Dashboards de equipos deportivos**
2. **Gestión de proyectos**
3. **Visualización de grupos/categorías**
4. **Portfolio de diseño UX/UI**
5. **Demos interactivas**
6. **Presentaciones de productos**
7. **Sistemas de gestión de organizaciones**

## 🚀 Roadmap

### Próximos Componentes

- `sport-chart-showcase` - Biblioteca de gráficos animados
- `sport-card-gallery` - Galería de estilos de tarjetas
- `sport-navigation-patterns` - Patrones de navegación
- `sport-form-builders` - Constructores de formularios dinámicos
- `sport-animation-library` - Biblioteca de animaciones reutilizables

### Mejoras Futuras para sport-demo-tables

- [ ] Modo de edición inline
- [ ] Drag & drop para reordenar
- [ ] Exportación a PDF/Excel
- [ ] Zoom y pan en vista Galaxy
- [ ] Filtros avanzados
- [ ] Paginación integrada
- [ ] Temas de color personalizables
- [ ] Animaciones de transición entre vistas

## 📝 Notas de Desarrollo

### Código Fuente Completo

El código HTML completo con todas las 7 vistas está disponible en:
```
src/app/features/dashboard/pages/teams-page/teams-page.visual.html
```

Este archivo contiene la implementación completa de:
- Grid (líneas 30-63)
- Board (líneas 65-103)
- Galaxy (líneas 105-374)
- DataTable (líneas 377-564)
- Carousel 3D (líneas 566-640)
- Timeline (líneas 643-723)
- Hexagon (líneas 726-774)

### CSS Completo

Todos los estilos están copiados en:
```
src/app/shared/design/sport-demo-tables.component.css
```

Incluye más de 370 líneas de CSS con:
- Animaciones keyframes personalizadas
- Efectos 3D y perspectiva
- Dark mode completo
- Responsive breakpoints
- Hover states
- Transiciones suaves

## 🎓 Aprende Más

- **README.md**: Documentación general del componente
- **USAGE-EXAMPLE.md**: Ejemplos de código y casos de uso
- **Source Code**: Revisa los archivos .ts, .html y .css para implementaciones específicas

## 🤝 Contribuciones

Esta librería fue creada como parte del proyecto SportPlanner para demostrar capacidades innovadoras de UI/UX.

### Agregar Nuevas Vistas

1. Añade la vista al template HTML
2. Crea los estilos correspondientes en CSS
3. Actualiza el tipo `ViewMode` en index.ts
4. Documenta en README.md y USAGE-EXAMPLE.md

---

**Creado con ❤️ y mucha innovación para SportPlanner**

*Última actualización: 2025-10-03*
