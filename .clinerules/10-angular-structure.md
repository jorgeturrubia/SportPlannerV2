# Angular 20 Structure

## Arquitectura por Features
Cada feature debe tener su propia carpeta con estructura completa:

```
src/app/features/
├── training/
│   ├── components/
│   │   ├── training-card/
│   │   └── training-form/
│   ├── pages/
│   │   ├── training-list/
│   │   └── training-detail/
│   ├── services/
│   ├── models/
│   ├── guards/
│   └── training.routes.ts
```

## Standalone Components
- **SIEMPRE** usar standalone components, NO NgModules
- Imports específicos por componente, no globales
- Lazy loading por `loadComponent` en rutas

```typescript
// Ejemplo: Standalone Component
@Component({
  selector: 'app-training-card',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule],
  template: `...`
})
export class TrainingCardComponent {}
```

## Lazy Routing
```typescript
// training.routes.ts
export const TRAINING_ROUTES: Route[] = [
  {
    path: '',
    loadComponent: () => import('./pages/training-list/training-list.component')
      .then(c => c.TrainingListComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./pages/training-detail/training-detail.component')
      .then(c => c.TrainingDetailComponent)
  }
];
```

## Material Design Integration
- **Imports por Feature**: No importar Material globalmente
- **Componentes Necesarios**: Solo importar componentes específicos por feature
- **Theming**: Configurar tema en nivel de aplicación

```typescript
// Ejemplo: Imports específicos
@Component({
  standalone: true,
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule
  ]
})
```

## Signals y State Management
- **Signals**: Para estado reactivo local
- **Services**: Para estado compartido entre features
- **RxJS**: Para operaciones asíncronas complejas

> MANDATORY: Use Angular Signals as the default reactivity mechanism for component and feature state. Do not use Subjects/BehaviorSubjects or component-level RxJS streams as the primary reactive state unless there's a justified, documented reason (add an ADR when deviating).

```typescript
// Ejemplo: Signal-based component
export class TrainingListComponent {
  private trainingService = inject(TrainingService);
  
  trainings = signal<Training[]>([]);
  loading = signal(false);
  
  async loadTrainings() {
    this.loading.set(true);
    const data = await this.trainingService.getTrainings();
    this.trainings.set(data);
    this.loading.set(false);
  }
}
```

## Estructura de Carpetas Completa
```
src/app/
├── core/
│   ├── guards/
│   ├── interceptors/
│   ├── services/
│   └── models/
├── shared/
│   ├── components/
│   ├── directives/
│   ├── pipes/
│   └── utilities/
├── features/
│   ├── auth/
│   ├── training/
│   ├── athlete/
│   ├── planning/
│   └── dashboard/
├── layout/
│   ├── header/
│   ├── sidebar/
│   └── footer/
└── app.routes.ts
```

## Component Communication
- **@Input/@Output**: Para comunicación padre-hijo
- **Services**: Para comunicación entre features no relacionadas
- **Signals**: Para estado reactivo compartido
- **EventBus**: Para eventos de aplicación global (usando services)

## Performance Optimization
- **OnPush**: Change detection strategy por defecto
- **TrackBy**: Para listas dinámicas
- **Lazy Loading**: Para todas las features
- **Preloading**: Strategy para módulos críticos
