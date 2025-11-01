# Sistema de Plantillas de Canchas Deportivas 🏀⚽🤾

Sistema profesional de renderizado de canchas deportivas para el sistema de animación de ejercicios.

## 🎯 Características

- **Pista de Baloncesto Completa (FIBA)**: Dimensiones oficiales 28m x 15m con todas las líneas reglamentarias
- **Sistema Extensible**: Preparado para añadir más deportes (fútbol, balonmano, voleibol, fútbol sala)
- **Renderizado SVG**: Gráficos vectoriales escalables de alta calidad
- **Personalizable**: Opciones para mostrar/ocultar elementos, colores personalizados
- **Proporciones Realistas**: Respeta las dimensiones oficiales de cada deporte

## 📐 Cancha de Baloncesto (Implementada)

### Elementos Incluidos

✅ **Perímetro de la cancha** (28m × 15m)  
✅ **Línea central**  
✅ **Círculo central** (radio 1.8m)  
✅ **Zonas restringidas** (4.9m × 5.8m) con semicírculos  
✅ **Líneas de tiros libres** (3.6m de la línea de fondo)  
✅ **Líneas de 3 puntos** (radio 6.75m desde canasta)  
✅ **Canastas y aros** con tableros  
✅ **Marcas de posición** para rebotes  
✅ **Marcas decorativas** en esquinas  

### Opciones de Renderizado

```typescript
interface CourtRenderOptions {
  showLines?: boolean;              // Mostrar líneas del campo (default: true)
  showThreePointLine?: boolean;     // Mostrar línea de 3 puntos (default: true)
  showKeyArea?: boolean;            // Mostrar zonas restringidas (default: true)
  showCenterCircle?: boolean;       // Mostrar círculo central (default: true)
  lineColor?: string;               // Color de las líneas (default: '#FFFFFF')
  lineWidth?: number;               // Grosor de las líneas (default: 0.15)
  opacity?: number;                 // Opacidad general (default: 1)
}
```

## 🚀 Uso

### Crear un ejercicio con cancha de baloncesto

```typescript
import { AnimationBuilder } from './models/exercise-animation.model';

const builder = new AnimationBuilder();

// Añadir frames con jugadores, balón, etc...
builder.addFrame(500, (frame) => {
  builder.addPlayer(frame, 'player1', 20, 50, 'P1', '#3B82F6');
  builder.addBall(frame, 20, 50);
});

// Construir ejercicio con configuración de cancha
const exercise = builder.build({
  id: 'my-drill',
  name: 'Mi Ejercicio',
  description: 'Descripción del ejercicio',
  sport: 'basketball',
  court: {
    type: 'basketball',
    renderOptions: {
      showLines: true,
      showThreePointLine: true,
      showKeyArea: true,
      showCenterCircle: true,
      lineColor: '#FFFFFF',      // Líneas blancas
      lineWidth: 0.15,            // Líneas finas
      opacity: 1                  // Totalmente visible
    },
    backgroundColor: '#C17B3A'    // Color parquet
  },
  metadata: {
    difficulty: 'intermediate',
    playerCount: 2
  }
});
```

### Personalizar colores y apariencia

```typescript
// Cancha con líneas más gruesas y oscuras
court: {
  type: 'basketball',
  renderOptions: {
    lineColor: '#1E3A8A',        // Azul oscuro
    lineWidth: 0.25,             // Más gruesas
    opacity: 0.8                 // Ligeramente transparente
  },
  backgroundColor: '#D4A373'     // Madera clara
}

// Cancha minimalista (solo perímetro)
court: {
  type: 'basketball',
  renderOptions: {
    showLines: true,
    showThreePointLine: false,   // Sin línea de 3 puntos
    showKeyArea: false,          // Sin zonas
    showCenterCircle: false      // Sin círculo central
  }
}
```

## 📊 Sistema de Coordenadas

### Basketball (28m × 15m)

- **ViewBox**: `0 0 186.7 100`
- **Aspect Ratio**: `28:15` (1.867:1)
- **Escala**: 1 unidad SVG = 0.15m real

### Coordenadas de Ejercicios

Los ejercicios usan coordenadas **0-100** (porcentaje):

```typescript
// Jugador en el centro de la cancha
builder.addPlayer(frame, 'player1', 50, 50, 'P1');
// → Se traduce automáticamente a coordenadas SVG (93.35, 50)

// Jugador en esquina inferior izquierda
builder.addPlayer(frame, 'player2', 10, 90, 'P2');
// → Se traduce a (18.67, 90)
```

El componente `ExerciseAnimationPlayerComponent` maneja la conversión automáticamente mediante `getCourtX()` y `getCourtY()`.

## 🔮 Deportes Próximamente

### Fútbol ⚽
- Dimensiones: 105m × 68m
- Aspect Ratio: `105:68` (1.544:1)
- Elementos: Líneas laterales, áreas, círculo central, córners

### Balonmano 🤾
- Dimensiones: 40m × 20m
- Aspect Ratio: `2:1`
- Elementos: Áreas de portería, línea de 9m, línea de 7m

### Voleibol 🏐
- Dimensiones: 18m × 9m
- Aspect Ratio: `2:1`
- Elementos: Red central, líneas de ataque, zonas de servicio

### Fútbol Sala
- Dimensiones: 40m × 20m
- Aspect Ratio: `2:1`
- Elementos: Áreas, líneas laterales, córners

## 🎨 Colores por Defecto

| Deporte | Color Fondo | Descripción |
|---------|-------------|-------------|
| Basketball | `#C17B3A` | Parquet madera |
| Football | `#4A7C44` | Verde césped |
| Handball | `#E0A567` | Madera clara |
| Volleyball | `#D4A373` | Madera |
| Futsal | `#C17B3A` | Parquet |

## 📝 Añadir Nuevos Deportes

Para añadir un nuevo deporte, modifica `court-templates.model.ts`:

```typescript
// 1. Añadir dimensiones oficiales
export const COURT_DIMENSIONS: Record<CourtType, CourtDimensions> = {
  // ... existentes
  newSport: {
    width: 100,
    height: 50,
    aspectRatio: '100 / 50'
  }
};

// 2. Crear función de renderizado
function renderNewSportCourt(options: CourtRenderOptions = {}): string {
  return `
    <g class="new-sport-court-template">
      <!-- SVG content here -->
      <rect ... />
      <line ... />
    </g>
  `;
}

// 3. Registrar en COURT_TEMPLATES
export const COURT_TEMPLATES: Record<CourtType, CourtTemplate> = {
  // ... existentes
  newSport: {
    type: 'newSport',
    name: 'Nombre del Deporte',
    dimensions: COURT_DIMENSIONS.newSport,
    backgroundColor: '#HEXCOLOR',
    renderSVG: renderNewSportCourt
  }
};
```

## 🔧 Arquitectura Técnica

```
court-templates.model.ts
├── CourtType (enum)
├── CourtDimensions (dimensiones reales)
├── CourtTemplate (interfaz de plantilla)
├── CourtRenderOptions (opciones de renderizado)
├── COURT_DIMENSIONS (constantes)
├── COURT_TEMPLATES (registro de plantillas)
└── Funciones de renderizado por deporte
    ├── renderBasketballCourt()
    ├── renderFootballField() (placeholder)
    └── renderHandballCourt() (placeholder)
```

## 🎬 Ejemplo Completo

Ver `basketball-exercises.ts` para un ejemplo completo de ejercicio animado con la pista de baloncesto.

## 📚 Referencias

- [FIBA Official Basketball Rules](https://www.fiba.basketball/rules)
- Dimensiones oficiales de canchas deportivas
- SVG Specification para gráficos vectoriales

---

**Autor**: SportPlanner Development Team  
**Versión**: 1.0  
**Última actualización**: 2025-11-01
