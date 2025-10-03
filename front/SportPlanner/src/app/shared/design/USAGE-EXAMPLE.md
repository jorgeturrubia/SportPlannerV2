# 📚 Ejemplo de Uso - SportDemoTablesComponent

## Integración en tu componente

### 1. Importar el componente

```typescript
import { Component, signal } from '@angular/core';
import { SportDemoTablesComponent } from '@shared/design/sport-demo-tables.component';

@Component({
  selector: 'app-my-teams',
  standalone: true,
  imports: [SportDemoTablesComponent],
  template: `
    <div class="p-6">
      <h1 class="text-2xl font-bold mb-4">Mis Equipos</h1>

      <app-sport-demo-tables
        [items]="teams()"
        [viewMode]="currentView()"
        [maxCapacity]="15"
        [showCreateButton]="true"
        (onEdit)="handleEdit($event)"
        (onDelete)="handleDelete($event)"
        (onCreate)="openCreateDialog()"
        (onSelect)="handleSelect($event)"
        (viewModeChange)="currentView.set($event)">
      </app-sport-demo-tables>
    </div>
  `
})
export class MyTeamsComponent {
  teams = signal([
    {
      id: 't1',
      name: 'Águilas FC',
      color: '#ef4444',
      description: 'Equipo juvenil con enfoque en técnica y táctica.',
      members: [
        { id: 'm1', name: 'Juan Pérez' },
        { id: 'm2', name: 'María García' },
        { id: 'm3', name: 'Carlos López' }
      ],
      createdAt: new Date('2024-01-15')
    },
    // ... más equipos
  ]);

  currentView = signal<'grid' | 'board' | 'carousel' | 'timeline' | 'hexagon' | 'datatable' | 'galaxy'>('datatable');

  handleEdit(team: any) {
    console.log('Editar equipo:', team);
    // Abre tu modal/form de edición
  }

  handleDelete(team: any) {
    if (confirm(`¿Eliminar ${team.name}?`)) {
      // Lógica de eliminación
      this.teams.update(teams => teams.filter(t => t.id !== team.id));
    }
  }

  openCreateDialog() {
    console.log('Crear nuevo equipo');
    // Abre tu modal/form de creación
  }

  handleSelect(team: any) {
    console.log('Equipo seleccionado:', team);
    // Útil para la vista Galaxy cuando se selecciona un planeta
  }
}
```

## 2. Vistas Disponibles

### DataTable (Tabla Avanzada) - Por defecto
```typescript
currentView.set('datatable');
```

Características:
- ✅ Búsqueda en tiempo real
- ✅ Ordenación por columnas
- ✅ Filas expandibles con detalles
- ✅ Badges de estado por capacidad
- ✅ Mini gráficos integrados
- ✅ Avatares de miembros
- ✅ Responsive y dark mode

### Grid (Cuadrícula)
```typescript
currentView.set('grid');
```
Vista clásica en tarjetas organizadas en grid.

### Board (Tablero Horizontal)
```typescript
currentView.set('board');
```
Vista tipo Kanban con scroll horizontal.

### Carousel (Carrusel 3D)
```typescript
currentView.set('carousel');
```
Efecto 3D con perspectiva y rotación.

### Timeline (Línea de Tiempo)
```typescript
currentView.set('timeline');
```
Visualización cronológica horizontal.

### Hexagon (Hexagonal)
```typescript
currentView.set('hexagon');
```
Patrón de panal de abejas.

### Galaxy (Sistema Solar) - ⭐ ÚNICO
```typescript
currentView.set('galaxy');
```

¡La vista más innovadora! Sistema solar interactivo con:
- 🌞 Sol central animado con llamaradas
- 🪐 Planetas orbitando (equipos)
- ✨ Estrellas parpadeantes
- 🎯 Panel lateral al seleccionar planeta
- 🔄 Órbitas con diferentes velocidades
- 💫 Anillos planetarios para equipos grandes

## 3. Estructura de Datos

Cada item debe seguir esta interfaz:

```typescript
interface TeamItem {
  id: string | number;           // Identificador único
  name: string;                  // Nombre del equipo
  color: string;                 // Color HEX (#ef4444)
  description?: string;          // Descripción opcional
  members: Array<{               // Array de miembros
    id: string | number;
    name: string;
  }>;
  createdAt: Date | string;      // Fecha de creación
}
```

## 4. Personalización

### Cambiar capacidad máxima
```html
<app-sport-demo-tables
  [items]="teams()"
  [maxCapacity]="20"  <!-- Default: 15 -->
  ...>
</app-sport-demo-tables>
```

### Ocultar botón de crear
```html
<app-sport-demo-tables
  [items]="teams()"
  [showCreateButton]="false"
  ...>
</app-sport-demo-tables>
```

### Vista inicial
```html
<app-sport-demo-tables
  [items]="teams()"
  [viewMode]="'galaxy'"  <!-- Inicia en vista Galaxy -->
  ...>
</app-sport-demo-tables>
```

## 5. Eventos

### onEdit
Se emite cuando el usuario hace click en el botón editar de un item.

```typescript
handleEdit(item: any) {
  this.router.navigate(['/teams', item.id, 'edit']);
}
```

### onDelete
Se emite cuando el usuario hace click en el botón eliminar.

```typescript
async handleDelete(item: any) {
  if (confirm(`¿Eliminar ${item.name}?`)) {
    await this.teamsService.delete(item.id);
    await this.loadTeams();
  }
}
```

### onCreate
Se emite cuando el usuario hace click en el botón crear.

```typescript
openCreateDialog() {
  this.dialog.open(CreateTeamDialogComponent);
}
```

### onSelect
Se emite cuando se selecciona un item (principalmente útil en vista Galaxy).

```typescript
handleSelect(item: any) {
  console.log('Planeta seleccionado:', item);
  this.selectedTeam.set(item);
}
```

### viewModeChange
Se emite cuando cambia la vista actual.

```typescript
handleViewChange(mode: string) {
  localStorage.setItem('preferredView', mode);
}
```

## 6. Estilos Personalizados

El componente incluye sus propios estilos, pero puedes sobrescribirlos:

```css
/* En tu componente padre */
::ng-deep app-sport-demo-tables {
  .datatable-container {
    /* Tus estilos personalizados */
  }

  .planet-surface {
    /* Personalizar planetas en vista Galaxy */
  }
}
```

## 7. Ejemplo Completo con Backend

```typescript
import { Component, signal, inject, OnInit } from '@angular/core';
import { SportDemoTablesComponent } from '@shared/design/sport-demo-tables.component';
import { TeamsService } from './teams.service';

@Component({
  selector: 'app-teams-page',
  standalone: true,
  imports: [SportDemoTablesComponent],
  template: `
    <div class="p-6">
      <app-sport-demo-tables
        [items]="teams()"
        [viewMode]="viewMode()"
        [maxCapacity]="15"
        (onEdit)="editTeam($event)"
        (onDelete)="deleteTeam($event)"
        (onCreate)="createTeam()">
      </app-sport-demo-tables>
    </div>
  `
})
export class TeamsPageComponent implements OnInit {
  private teamsService = inject(TeamsService);

  teams = signal<any[]>([]);
  viewMode = signal<any>('datatable');

  async ngOnInit() {
    await this.loadTeams();
  }

  async loadTeams() {
    try {
      const data = await this.teamsService.getAll();
      this.teams.set(data);
    } catch (error) {
      console.error('Error loading teams:', error);
    }
  }

  async editTeam(team: any) {
    // Implementar lógica de edición
  }

  async deleteTeam(team: any) {
    if (confirm(`¿Eliminar ${team.name}?`)) {
      try {
        await this.teamsService.delete(team.id);
        await this.loadTeams();
      } catch (error) {
        console.error('Error deleting team:', error);
      }
    }
  }

  createTeam() {
    // Implementar lógica de creación
  }
}
```

## 8. Tips y Mejores Prácticas

### Performance
- Usa señales (`signal`) para datos reactivos
- Implementa track function en ngFor para grandes listas
- Considera paginación para más de 50 items

### UX
- Guarda la vista preferida del usuario en localStorage
- Muestra feedback al usuario después de acciones
- Implementa confirmación para acciones destructivas

### Accesibilidad
- Los botones tienen atributos title para tooltips
- Usa textos descriptivos en eventos
- Asegúrate de que los colores tengan buen contraste

---

**¿Preguntas o problemas?** Consulta el README.md principal o revisa el código fuente en `sport-demo-tables.component.ts`
