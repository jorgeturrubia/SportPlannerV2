# Análisis Profundo: Problema con las Flechas de Expandir/Contraer

## 🔍 Hipótesis del Problema

El componente `objective-tree` no está reaccionando cuando presionas las flechas para expandir/contraer categorías y subcategorías.

### Posibles Causas:

1. **Los Signals no se actualizan correctamente** 
   - `expandedCategories` y `expandedSubcategories` son signals vacíos `{}`
   - Al hacer toggle, no se está actualizando el signal correctamente

2. **El binding en el HTML no está reaccionando**
   - `[class.expanded]` y `[class.rotated]` dependen de `expandedCategories()[pair[0]]`
   - Si el signal no se actualiza, el binding no cambia

3. **Los datos no se están pasando correctamente**
   - `[objectives]="planObjectives()"` podría no tener datos
   - `[selectedIds]="selectedIdsArray"` podría estar vacío

4. **El evento click no se está capturando**
   - El `(click)="toggleCategory(pair[0])"` podría no estar funcionando

5. **Computed vs Signal**
   - El `planObjectives()` es un signal en el padre
   - Cuando cambias objetivos, puede estar recalculando todo

## 📊 Flujo de Datos

```
plan-goals-manager.component
    │
    ├─ planObjectives = signal<PlanObjective[]> ([])
    │  ├─ Se carga en loadPlanObjectives()
    │  └─ Se actualiza cuando añades/quitas objetivos
    │
    ├─ selectedIdsArray getter
    │  └─ return this.planObjectives().map(p => p.id)
    │
    └─→ app-objective-tree
         │
         ├─ [objectives]="planObjectives()"      ← ENTRADA
         ├─ [selectedIds]="selectedIdsArray"     ← ENTRADA
         ├─ [addingMode]="isEditingValue"        ← ENTRADA
         │
         ├─ @Input objectives setter
         │  ├─ _objectives.set(v || [])
         │  └─ updateExpandedFromSelection()
         │
         ├─ @Input selectedIds setter
         │  ├─ _selectedIds = v || []
         │  └─ updateExpandedFromSelection()
         │
         ├─ expandedCategories = signal({})      ← ESTADO INTERNO
         ├─ expandedSubcategories = signal({})   ← ESTADO INTERNO
         │
         ├─ grouped = computed()
         ├─ groupedPairs = computed()
         │
         ├─ toggleCategory(cat: string)          ← EVENTO CLICK
         └─ toggleSubcategories(cat, sub)        ← EVENTO CLICK
```

## 🔄 Ciclo de Actualización

### Cuando Presionas una Flecha (esperado):

1. `toggleCategory(pair[0])` se ejecuta
2. `expandedCategories.update()` se llama
3. El signal se actualiza con `{ ...s, [cat]: newValue }`
4. Angular detecta el cambio (change detection)
5. El binding `[class.expanded]="expandedCategories()[pair[0]]"` se re-evalúa
6. La clase `.expanded` se aplica/quita
7. La flecha rota (CSS transition)

### Posible Problema:

Si `expandedCategories()[pair[0]]` siempre devuelve `undefined`, entonces:
- `[class.expanded]="undefined"` → class NO se aplica
- El `*ngIf="expandedCategories()[pair[0]]"` → false → contenido NO se muestra

## 🧪 Plan de Debug

### Paso 1: Verificar que los datos se pasan correctamente
En la consola, verás:
```
[ObjectiveTree] SET objectives: [...]
[ObjectiveTree] SET selectedIds: [...]
```

### Paso 2: Verificar que el toggle se captura
Cuando presionas, verás:
```
[ObjectiveTree] toggleCategory('Technical Skills') - current: undefined, new: true
[ObjectiveTree] expandedCategories BEFORE: {}
[ObjectiveTree] expandedCategories AFTER: {"Technical Skills": true}
```

### Paso 3: Verificar que el binding reacciona
En el HTML, hay un DIV debug que muestra el estado:
```
DEBUG: expandedCategories = {"Technical Skills": true}
DEBUG: expandedSubcategories = {}
```

### Paso 4: Verificar que el HTML se renderiza
Si el contenido se muestra después del toggle, el problema es CSS.

## 🐛 Problemas Identificados

### 1. Logical Issue en updateExpandedFromSelection()
```typescript
// ¿Qué pasa si tienes 0 objetivos seleccionados?
if (selected.size === 0) return;  // ← Retorna aquí

// ¿Qué pasa si tienes > 5 objetivos seleccionados?
// El código "expand all" NO se ejecuta
// Solo se expanden las categorías que contienen items seleccionados
```

**Implicación**: Si inicialmente no hay objetivos seleccionados, TODO está colapsado. El usuario debe presionar para expandir.

### 2. Possible Race Condition
```typescript
// En el parent:
planObjectives = signal([])

// El setter cambia:
set objectives(v) {
  _objectives.set(v)
  updateExpandedFromSelection()  // ← Se ejecuta CADA VEZ que cambian los datos
}

// Si planObjectives() se actualiza constantemente,
// updateExpandedFromSelection() se ejecuta constantemente,
// lo que podría forzar la expansión aunque el usuario haya presionado "contraer"
```

### 3. Edge Case en Toggle
```typescript
toggleCategory(cat: string) {
  // Cuando presionas por primera vez (cat no existe en el objeto):
  expandedCategories()[cat]  // undefined
  undefined !== true         // true
  newValue = true            // Expande ✓

  // Cuando presionas por segunda vez (cat existe con valor true):
  expandedCategories()[cat]  // true
  true !== true              // false
  newValue = false           // Contrae ✓

  // PERO si updateExpandedFromSelection() se ejecuta en medio:
  // expandedCategories.update(s => ({ ...s, [cat]: true }))
  // El estado vuelve a true, cancelando el toggle del usuario
}
```

## ✅ Soluciones Posibles

### Solución 1: Agregar bandera para evitar actualización automática
```typescript
private userInitiatedChange = false;

toggleCategory(cat: string) {
  this.userInitiatedChange = true;
  // ... toggle logic
  // después de actualizar:
  setTimeout(() => this.userInitiatedChange = false, 0);
}

updateExpandedFromSelection() {
  if (this.userInitiatedChange) return;  // ← NO actualices si el usuario presionó
  // ... auto-expand logic
}
```

### Solución 2: Separar estado de "auto-expand" del estado de "user-toggle"
```typescript
autoExpandedCategories = signal<Record<string, boolean>>({});  // Auto-managed
userExpandedCategories = signal<Record<string, boolean>>({});  // User-managed

// En el template, usar el user state si existe, sino usar auto state
[class.expanded]="userExpandedCategories()[cat] ?? autoExpandedCategories()[cat]"
```

### Solución 3: Desactivar updateExpandedFromSelection para no-empty plans
```typescript
updateExpandedFromSelection() {
  const selected = new Set(this._selectedIds || []);
  
  // SOLO auto-expand si es el primer load (planObjectives vacío antes)
  if (this.planObjectives().length === 0) return;
  
  // ... resto del código
}
```

## 🎯 Siguiente Paso

Abre el navegador, abre la consola (F12), y presiona una flecha.
Comparte los logs que ves:
1. ¿Se ve `[ObjectiveTree] toggleCategory(...)`?
2. ¿Se ve el estado BEFORE y AFTER?
3. ¿El DEBUG muestra el estado actualizado?

Esto nos dirá si el problema es:
- ✅ Lógica TypeScript (debug mostrará el error)
- ✅ Binding HTML (debug no mostrará cambios)
- ✅ CSS (debug mostrará cambios pero UI no cambia)
- ✅ Timing (updateExpandedFromSelection interfiere)
