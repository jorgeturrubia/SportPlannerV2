# 🎨 Design Components Library

Esta carpeta contiene componentes de diseño reutilizables e innovadores para SportPlanner.

## 📁 Componentes Disponibles

### `sport-demo-tables`

Componente de demostración con **7 vistas únicas** para visualizar datos en formato de equipos/grupos.

#### 🌟 Vistas Disponibles

1. **Grid** - Vista en cuadrícula clásica con tarjetas
2. **Board** - Vista horizontal tipo Kanban
3. **DataTable** - Tabla avanzada con expansión, búsqueda y ordenación
4. **Carousel 3D** - Carrusel con perspectiva 3D
5. **Timeline** - Línea de tiempo horizontal
6. **Hexagon** - Patrón hexagonal tipo panal de abejas
7. **Galaxy** - ⭐ Sistema solar interactivo (ÚNICO Y REVOLUCIONARIO)

#### 📖 Uso

```typescript
import { SportDemoTablesComponent } from '@shared/design/sport-demo-tables.component';

@Component({
  imports: [SportDemoTablesComponent],
  // ...
})
export class MyComponent {
  teams = signal([
    {
      id: 't1',
      name: 'Equipo A',
      color: '#ef4444',
      description: 'Descripción del equipo',
      members: [{ id: 'm1', name: 'Juan' }, /* ... */],
      createdAt: new Date()
    }
  ]);

  handleEdit(team: any) {
    console.log('Edit:', team);
  }

  handleDelete(team: any) {
    console.log('Delete:', team);
  }
}
```

```html
<app-sport-demo-tables
  [items]="teams()"
  [viewMode]="'galaxy'"
  [maxCapacity]="15"
  [showCreateButton]="true"
  (onEdit)="handleEdit($event)"
  (onDelete)="handleDelete($event)"
  (onCreate)="openCreateDialog()"
  (onSelect)="handleSelection($event)">
</app-sport-demo-tables>
```

#### 📥 Inputs

| Input | Tipo | Default | Descripción |
|-------|------|---------|-------------|
| `items` | `any[]` | `[]` | Array de elementos a visualizar |
| `viewMode` | `'grid' \| 'board' \| 'carousel' \| 'timeline' \| 'hexagon' \| 'datatable' \| 'galaxy'` | `'grid'` | Modo de visualización activo |
| `maxCapacity` | `number` | `15` | Capacidad máxima por item |
| `showCreateButton` | `boolean` | `true` | Mostrar botón de crear |

#### 📤 Outputs

| Output | Tipo | Descripción |
|--------|------|-------------|
| `onEdit` | `EventEmitter<any>` | Se emite cuando se hace click en editar |
| `onDelete` | `EventEmitter<any>` | Se emite cuando se hace click en eliminar |
| `onCreate` | `EventEmitter<void>` | Se emite cuando se hace click en crear |
| `onSelect` | `EventEmitter<any>` | Se emite cuando se selecciona un item (ej: planeta en galaxy) |
| `viewModeChange` | `EventEmitter<string>` | Se emite cuando cambia el modo de vista |

#### 🎨 Estructura de Datos Esperada

Cada item debe tener:

```typescript
interface Item {
  id: string | number;
  name: string;
  color: string;  // Hex color code
  description?: string;
  members: Array<{ id: string | number; name: string }>;
  createdAt: Date | string;
  // Campos opcionales adicionales...
}
```

#### ✨ Características Destacadas

##### Vista Galaxy 🌌

- **Sol central animado** con llamaradas solares
- **Planetas orbitando** con física simulada
- **Tamaño dinámico** basado en capacidad
- **Órbitas concéntricas** con velocidades diferentes
- **Panel lateral** con detalles al seleccionar planeta
- **Estrellas parpadeantes** de fondo
- **Anillos planetarios** para items con alta capacidad

##### Vista DataTable 📊

- **Búsqueda en tiempo real**
- **Ordenación por columnas**
- **Filas expandibles** con detalles completos
- **Badges de estado** con colores semánticos
- **Mini gráficos** integrados
- **Dark mode** completo

#### 🎯 Casos de Uso

- Visualización de equipos deportivos
- Dashboard de proyectos
- Gestión de grupos/categorías
- Presentaciones interactivas
- Demos de UX/UI
- Portfolio de diseño

## 🚀 Próximos Componentes

- `sport-chart-showcase` - Gráficos y visualizaciones de datos
- `sport-card-gallery` - Galería de estilos de tarjetas
- `sport-navigation-patterns` - Patrones de navegación innovadores

---

**Creado con ❤️ para SportPlanner**
