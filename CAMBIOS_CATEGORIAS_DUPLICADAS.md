# 🔧 FIX: Categorías y Subcategorías Duplicadas - 20/10/2025

## 📋 Problema Identificado

En la página de **Gestión de Objetivos** (`plan-goals-manager.component`), las categorías y subcategorías llegaban duplicadas y no filtradas por deporte.

### Causa Raíz

1. **Línea 147 original** en `plan-goals-manager.component.ts`:
```typescript
const [objectives, categories] = await Promise.all([
  this.objectivesService.getObjectives(),
  this.categoriesService.getCategories()  // ❌ SIN PARÁMETRO SPORT
]);
```

2. El método `getCategories()` del servicio no recibía el parámetro `sport`, por lo que:
   - **Cargaba categorías de todos los deportes** (Basketball, Football, Handball)
   - **Sin filtrar por el deporte de la suscripción actual**
   - Potencialmente devolvía **registros duplicados**

3. Las subcategorías se cargaban sin deduplicación

---

## ✅ Cambios Realizados

### Archivo: `plan-goals-manager.component.ts`

#### 1. Actualizar método `loadData()` (línea 144-169)

**Antes:**
```typescript
async loadData(): Promise<void> {
  this.isLoading.set(true);
  try {
    const [objectives, categories] = await Promise.all([
      this.objectivesService.getObjectives(),
      this.categoriesService.getCategories()  // ❌ SIN SPORT
    ]);

    this.allObjectives.set(objectives || []);
    this.categories.set(categories || []);
    // ...
  }
}
```

**Después:**
```typescript
async loadData(): Promise<void> {
  this.isLoading.set(true);
  try {
    // Get sport from subscription context
    const subscription = this.subscriptionContext.subscription();
    const sport = subscription ? this.parseSportToEnum(subscription.sport) : undefined;

    // Load objectives and categories - passing sport to filter by sport
    const [objectives, categories] = await Promise.all([
      this.objectivesService.getObjectives(),
      this.categoriesService.getCategories(sport)  // ✅ CON SPORT!
    ]);

    this.allObjectives.set(objectives || []);
    
    // Deduplicate categories by id (in case API returns duplicates)
    const uniqueCategories = this.deduplicateById(categories || []);
    this.categories.set(uniqueCategories);
    // ...
  }
}
```

**Cambios clave:**
- ✅ Obtener `sport` de `subscriptionContext`
- ✅ Pasar `sport` a `getCategories(sport)` para filtrar por deporte
- ✅ Deduplicar categorías con `deduplicateById()`

---

#### 2. Actualizar método `onCategoryChange()` (línea 171-182)

**Antes:**
```typescript
async onCategoryChange(categoryId: string | null): Promise<void> {
  this.filterState.update(f => ({ ...f, categoryId, subcategoryId: null }));

  if (categoryId) {
    const subs = await this.subcategoriesService.getSubcategories(categoryId);
    this.subcategories.set(subs || []);  // ❌ SIN DEDUPLICACIÓN
  } else {
    this.subcategories.set([]);
  }
}
```

**Después:**
```typescript
async onCategoryChange(categoryId: string | null): Promise<void> {
  this.filterState.update(f => ({ ...f, categoryId, subcategoryId: null }));

  if (categoryId) {
    const subs = await this.subcategoriesService.getSubcategories(categoryId);
    // Deduplicate subcategories by id (in case API returns duplicates)
    const uniqueSubcategories = this.deduplicateById(subs || []);
    this.subcategories.set(uniqueSubcategories);
  } else {
    this.subcategories.set([]);
  }
}
```

**Cambios clave:**
- ✅ Deduplicar subcategorías con `deduplicateById()`

---

#### 3. Agregar método `deduplicateById()` (nueva función)

**Agregado al final de la clase:**

```typescript
private deduplicateById<T extends { id: string }>(items: T[]): T[] {
  const seen = new Set<string>();
  const result: T[] = [];
  
  for (const item of items) {
    if (!seen.has(item.id)) {
      seen.add(item.id);
      result.push(item);
    }
  }
  
  return result;
}
```

**Funcionalidad:**
- Utiliza un `Set` para rastrear IDs ya procesados
- Evita duplicados manteniendo solo la primera ocurrencia
- Es genérico y funciona con cualquier objeto que tenga `id`

---

## 🧪 Verificación

✅ **Build completado exitosamente:**
```
Application bundle generation complete. [23.121 seconds]
Output location: dist/SportPlanner
```

No hay errores de compilación ni advertencias relacionadas con los cambios.

---

## 🎯 Impacto

| Componente | Antes | Después |
|-----------|-------|---------|
| Categorías | Todas (repetidas) | Solo del deporte actual, sin duplicados |
| Subcategorías | Repetidas | Sin duplicados |
| Performance | Carga innecesaria | ✅ Optimizado |
| UX | Confuso | ✅ Claro y ordenado |

---

## 📊 Flujo Corregido

```
1. Usuario abre "Gestión de Objetivos"
   ↓
2. Se obtiene sport de subscriptionContext (ej: "Basketball")
   ↓
3. Se llama getCategories(Sport.Basketball)
   ↓
4. API retorna categorías solo de Basketball
   ↓
5. Se deduplicarán si hay registros repetidos
   ↓
6. Usuario ve categorías limpias y sin duplicados
   ↓
7. Al seleccionar categoría → Se cargan subcategorías filtradas
   ↓
8. Se deduplicarán subcategorías también
```

---

## 🔍 Servicio Backend Esperado

El servicio `ObjectiveCategoriesService.getCategories(sport?)` debería:

```typescript
async getCategories(sport?: Sport): Promise<ObjectiveCategoryDto[]> {
  const params: Record<string, string> = {};
  if (sport !== undefined) {
    params['sport'] = sport.toString();  // ✅ Pasar como query param
  }
  return await this.http.get<ObjectiveCategoryDto[]>(this.apiUrl, { params }).toPromise() || [];
}
```

**El backend debe responder con categorías filtradas por `?sport=1` (Basketball)**

---

## ✨ Siguientes Pasos Recomendados

1. **Testear en navegador:**
   - Abrir página de Gestión de Objetivos
   - Verificar que solo aparecen categorías del deporte actual
   - Seleccionar categoría y verificar subcategorías sin duplicados

2. **Verificar en backend:**
   - Confirmar que `/api/planning/objective-categories?sport=1` retorna solo categorías de Basketball
   - Confirmar que no hay registros duplicados en la BD

3. **Documentar en backend (si aplica):**
   - Si falta implementar el filtro `?sport` en la API, agregar esa funcionalidad

4. **Revisar otros componentes:**
   - Buscar si hay otros lugares donde se carga `getCategories()` sin pasar sport
   - Aplicar el mismo fix si es necesario

---

**Archivo modificado:** `plan-goals-manager.component.ts`
**Líneas afectadas:** 144-169, 171-182, y función nueva agregada
**Build Status:** ✅ SUCCESS
**Fecha:** 20 de Octubre de 2025
