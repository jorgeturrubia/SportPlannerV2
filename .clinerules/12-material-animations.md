# Material Design & Animations

## Material Components Strategy
- **Modular Imports**: Usar solo módulos necesarios en cada feature
- **Tree Shaking**: Evitar imports de todo `@angular/material`
- **CDK Integration**: Aprovechar Angular CDK para funcionalidades avanzadas

```typescript
// Ejemplo: Imports específicos por feature
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
```

## Animation Architecture
Declarar animaciones en archivos separados:

```
src/app/shared/animations/
├── slide.animations.ts
├── fade.animations.ts
├── scale.animations.ts
└── index.ts
```

## Accessibility Compliance
- **ARIA Labels**: Todos los componentes interactivos
- **Keyboard Navigation**: Soporte completo de teclado
- **Screen Readers**: Texto descriptivo para lectores de pantalla
- **Color Contrast**: Cumplir WCAG 2.1 AA

## Performance Optimization
- **OnPush Strategy**: Para componentes con Material
- **Lazy Loading**: Para módulos de Material por feature
- **Change Detection**: Optimizar con trackBy functions
