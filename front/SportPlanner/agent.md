# Frontend Agent Instructions - SportPlanner Angular App

> **Complete self-contained guide for Angular 20 development with Tailwind CSS v4**  
> **Context**: Standalone components, Signals, NO Angular Material

---

## 📑 Table of Contents

1. [Project Structure](#-project-structure)
2. [Quick Commands](#-quick-commands)
3. [Core Principles](#-core-principles)
4. [Angular 20 Architecture](#-angular-20-architecture)
5. [Tailwind CSS Styling](#-tailwind-css-styling)
6. [Tailwind Component Library](#-tailwind-component-library)
7. [Authentication & Supabase](#-authentication--supabase)
8. [Global Notifications](#-global-notifications)
9. [Testing Standards](#-testing-standards)
10. [State Management](#-state-management)
11. [Security Best Practices](#-security-best-practices)
12. [Common Patterns](#-common-patterns)
13. [Pre-Commit Checklist](#-pre-commit-checklist)

---

## ⚠️ CRITICAL RULE: NO Angular Material

**MANDATORY**: All UI components MUST be built with **Tailwind CSS only**.

❌ **NEVER import or use**:
- Any `@angular/material` modules (MatCardModule, MatButtonModule, MatIconModule, etc.)
- Material Design components
- Material Icons font
- `mat-` prefixed components

✅ **ALWAYS use**:
- Tailwind utility classes for all styling
- Heroicons or Lucide for icons
- Custom components in `shared/components/`

---

## 📁 Project Structure

```
front/SportPlanner/
├── src/
│   ├── app/
│   │   ├── features/              # Feature modules (lazy-loaded)
│   │   │   ├── training/
│   │   │   │   ├── components/    # Presentational components
│   │   │   │   ├── pages/         # Smart/container components (route targets)
│   │   │   │   ├── services/      # Feature-specific services
│   │   │   │   ├── models/        # Feature-specific types/interfaces
│   │   │   │   └── training.routes.ts  # Feature routing
│   │   │   ├── teams/
│   │   │   ├── athletes/
│   │   │   └── subscriptions/
│   │   ├── core/                  # Singleton services, auth, guards
│   │   │   ├── auth/
│   │   │   ├── services/
│   │   │   ├── guards/
│   │   │   ├── interceptors/
│   │   │   └── models/
│   │   ├── shared/                # Reusable components, directives, pipes
│   │   │   ├── components/
│   │   │   ├── directives/
│   │   │   ├── pipes/
│   │   │   └── notifications/     # Global notification system
│   │   ├── app.component.ts       # Root component (standalone)
│   │   ├── app.config.ts          # App configuration & providers
│   │   └── app.routes.ts          # Root routing
│   ├── environments/              # Environment config (NOT committed secrets)
│   ├── styles.css                 # Global styles (Tailwind imports)
│   └── index.html
├── angular.json                   # Angular workspace config
├── tailwind.config.js             # Tailwind v4 configuration
├── tsconfig.json                  # TypeScript configuration
└── package.json                   # Dependencies & scripts
```

---

## 🚀 Quick Commands

```bash
# Development
npm start                # Dev server (http://localhost:4200)
npm run watch            # Build in watch mode

# Testing
npm test                 # Run unit tests (Karma + Jasmine)
npm run test:coverage    # Generate coverage report

# Build
npm run build            # Production build
npm run build:dev        # Development build

# Code Quality
npm run lint             # ESLint check
npm run format           # Prettier format

# Generate
ng generate component features/training/components/training-card --standalone
ng generate service core/services/api
```

---

## ⚡ Core Principles

### 1. **Standalone Components Only**
❌ **NO NgModules** - Use standalone: true for all components, directives, pipes

```typescript
@Component({
  selector: 'app-training-card',
  standalone: true,
  imports: [CommonModule, RouterLink], // Only Angular essentials, no Material
  templateUrl: './training-card.component.html',
  styleUrls: ['./training-card.component.css']
})
export class TrainingCardComponent {}
```

### 2. **Dependency Injection with `inject()`**
✅ **Use `inject()`** function, not constructor injection

```typescript
export class TrainingListComponent {
  private readonly trainingService = inject(TrainingService);
  private readonly router = inject(Router);
  private readonly notificationService = inject(NotificationService);
  
  // ... component logic
}
```

### 3. **Signals for Reactive State** (MANDATORY)
✅ **Use Signals** as the default reactive mechanism  
❌ **Avoid BehaviorSubject/Subject** for primary state

```typescript
export class TrainingListComponent {
  private readonly trainingService = inject(TrainingService);
  
  // ✅ GOOD: Signals for reactive state
  trainings = signal<Training[]>([]);
  isLoading = signal(false);
  selectedTraining = signal<Training | null>(null);
  
  async loadTrainings() {
    this.isLoading.set(true);
    const data = await this.trainingService.getTrainings();
    this.trainings.set(data);
    this.isLoading.set(false);
  }
  
  // Computed values
  trainingCount = computed(() => this.trainings().length);
  hasTrainings = computed(() => this.trainings().length > 0);
}
```

#### When to Use RxJS
- **Complex async streams** (e.g., debounced search, polling)
- **Event composition** (combineLatest, merge, switchMap)
- **Integration with Angular Router/Forms** (inherently RxJS-based)

```typescript
// ✅ OK: RxJS for complex async operations
searchQuery$ = new BehaviorSubject<string>('');
filteredTrainings = toSignal(
  this.searchQuery$.pipe(
    debounceTime(300),
    switchMap(query => this.trainingService.search(query))
  ),
  { initialValue: [] }
);
```

### 4. **Feature-Based Structure**
Each feature has:
```
features/training/
├── components/        # Presentational components
├── pages/            # Smart/container components (route targets)
├── services/         # Feature-specific services
├── models/           # Feature-specific types/interfaces
├── guards/           # Feature-specific route guards (if needed)
└── training.routes.ts # Feature routing config
```

### 5. **Lazy Loading via `loadComponent`**
```typescript
// app.routes.ts
export const routes: Routes = [
  {
    path: 'training',
    loadChildren: () => import('./features/training/training.routes')
      .then(m => m.TRAINING_ROUTES)
  },
  {
    path: 'teams',
    loadComponent: () => import('./features/teams/pages/teams-list/teams-list.component')
      .then(c => c.TeamsListComponent)
  }
];
```

---

## 🏗️ Angular 20 Architecture

### Standalone Components Pattern

```typescript
// ✅ CORRECT: Standalone component
@Component({
  selector: 'app-training-card',
  standalone: true,
  imports: [CommonModule, RouterLink], // Import what you need
  template: `
    <div class="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition">
      <h3 class="text-lg font-semibold text-gray-900">{{ training().name }}</h3>
      <p class="text-gray-600 mt-2">{{ training().description }}</p>
      <button 
        class="mt-4 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition"
        (click)="viewDetails()">
        View Details
      </button>
    </div>
  `
})
export class TrainingCardComponent {
  training = input.required<Training>();
  
  viewDetails() {
    // Navigate or emit event
  }
}
```

```typescript
// ❌ WRONG: NgModule-based component
@NgModule({
  declarations: [TrainingCardComponent],  // ❌ Don't use NgModules
  imports: [CommonModule],
  exports: [TrainingCardComponent]
})
export class TrainingModule {}
```

### Component Communication

**Parent → Child**: Use `input<T>()` signal inputs
```typescript
export class TrainingCardComponent {
  training = input.required<Training>();
  showDetails = input<boolean>(false); // Optional with default
}
```

**Child → Parent**: Use `output<T>()` signal outputs
```typescript
export class TrainingCardComponent {
  training = input.required<Training>();
  delete = output<number>();
  
  onDelete() {
    this.delete.emit(this.training().id);
  }
}

// Parent template
<app-training-card 
  [training]="training()" 
  (delete)="handleDelete($event)">
</app-training-card>
```

### Routing Structure

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
      .then(c => c.TrainingDetailComponent),
    canActivate: [authGuard]
  },
  {
    path: 'create',
    loadComponent: () => import('./pages/training-form/training-form.component')
      .then(c => c.TrainingFormComponent),
    canActivate: [authGuard]
  }
];
```

---

## 🎨 Tailwind CSS Styling

### Tailwind CSS v4 Configuration

**Theme Configuration**
```css
/* @theme configuration in styles.css */
@theme {
  /* Primary Colors */
  --color-primary-50: #eff6ff;
  --color-primary-500: #3b82f6;
  --color-primary-900: #1e3a8a;
  
  /* Spacing */
  --spacing-xs: 0.5rem;
  --spacing-sm: 1rem;
  --spacing-md: 1.5rem;
  --spacing-lg: 2rem;
  
  /* Semantic Colors */
  --color-success: #10b981;
  --color-warning: #f59e0b;
  --color-error: #ef4444;
}
```

### Responsive Design
- **Mobile-first**: Start with mobile, add breakpoints as needed
- **Breakpoints**: `sm:` (640px), `md:` (768px), `lg:` (1024px), `xl:` (1280px), `2xl:` (1536px)
- **Container Queries**: Use when component layout depends on container size

```html
<!-- Responsive Grid -->
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
  <div class="bg-white p-4 rounded-lg shadow">...</div>
</div>
```

### Utility Classes Organization
```html
<!-- Structure recomendada de clases -->
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

### Dark Mode Support
```html
<!-- Dark mode classes -->
<div class="bg-white dark:bg-gray-900 
           text-gray-900 dark:text-white
           border-gray-200 dark:border-gray-700">
  <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100">Title</h1>
</div>
```

### Component Styles Best Practices
- **Primary approach**: Tailwind utilities in templates
- **Component CSS**: Only for complex animations or highly reusable patterns
- **No Angular Material**: Build everything with Tailwind
- **Animations**: Use Tailwind transition utilities (transition, duration, ease, animate-*)

```css
/* Component-specific (solo si es necesario) */
.training-card {
  @apply bg-white rounded-lg shadow-md p-6
         hover:shadow-lg transition-shadow duration-200;
}
```

### SportPlanner-Specific Patterns
```html
<!-- Training Card -->
<div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm 
           border border-gray-200 dark:border-gray-700
           hover:shadow-md transition-shadow duration-200
           p-6 space-y-4">
  
<!-- Dashboard Grid -->
<div class="grid grid-cols-1 lg:grid-cols-3 gap-6 auto-rows-min">
  
<!-- Form Layout -->
<form class="space-y-6 max-w-md mx-auto">
  <div class="space-y-2">
    <label class="text-sm font-medium text-gray-700 dark:text-gray-300">
    <input class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600
                  rounded-md focus:ring-primary-500 focus:border-primary-500">
  </div>
</form>
```

---

## 🧩 Tailwind Component Library

### Design System Tokens

**Colors**
```css
/* Primary (Blue) */
bg-blue-50 to bg-blue-900
text-blue-50 to text-blue-900
border-blue-50 to border-blue-900

/* Success (Green) */
bg-green-500, text-green-700, border-green-600

/* Warning (Yellow) */
bg-yellow-500, text-yellow-700, border-yellow-600

/* Danger (Red) */
bg-red-500, text-red-700, border-red-600

/* Neutral (Gray) */
bg-gray-50 to bg-gray-900
```

**Spacing Scale**
```
gap-2 (0.5rem), gap-4 (1rem), gap-6 (1.5rem), gap-8 (2rem)
p-2, p-4, p-6, p-8
m-2, m-4, m-6, m-8
```

### Button Variants

**Primary Button**
```html
<button class="px-4 py-2 bg-blue-600 text-white rounded-md font-medium
               hover:bg-blue-700 active:bg-blue-800
               focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2
               disabled:bg-gray-400 disabled:cursor-not-allowed
               transition duration-150 ease-in-out">
  Primary Action
</button>
```

**Secondary Button**
```html
<button class="px-4 py-2 bg-white text-gray-700 border border-gray-300 rounded-md font-medium
               hover:bg-gray-50 active:bg-gray-100
               focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2
               disabled:bg-gray-100 disabled:text-gray-400 disabled:cursor-not-allowed
               transition duration-150 ease-in-out">
  Secondary Action
</button>
```

**Danger Button**
```html
<button class="px-4 py-2 bg-red-600 text-white rounded-md font-medium
               hover:bg-red-700 active:bg-red-800
               focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2
               disabled:bg-gray-400 disabled:cursor-not-allowed
               transition duration-150 ease-in-out">
  Delete
</button>
```

**Icon Button**
```html
<button class="p-2 text-gray-600 hover:bg-gray-100 rounded-md
               focus:outline-none focus:ring-2 focus:ring-blue-500
               transition duration-150">
  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
  </svg>
</button>
```

### Card Components

**Basic Card**
```html
<div class="bg-white dark:bg-gray-800 rounded-lg shadow-md border border-gray-200 dark:border-gray-700 p-6">
  <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Card Title</h3>
  <p class="text-gray-600 dark:text-gray-300">Card content goes here</p>
</div>
```

**Interactive Card**
```html
<div class="bg-white dark:bg-gray-800 rounded-lg shadow-md border border-gray-200 dark:border-gray-700
            hover:shadow-lg hover:border-blue-300 dark:hover:border-blue-600
            transition-all duration-200 p-6 cursor-pointer">
  <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Training Plan</h3>
  <p class="text-gray-600 dark:text-gray-300">Click to view details</p>
</div>
```

**Card with Header and Footer**
```html
<div class="bg-white dark:bg-gray-800 rounded-lg shadow-md border border-gray-200 dark:border-gray-700 overflow-hidden">
  <!-- Header -->
  <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-900">
    <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Training Session</h3>
  </div>
  
  <!-- Body -->
  <div class="p-6">
    <p class="text-gray-600 dark:text-gray-300">Session content and details</p>
  </div>
  
  <!-- Footer -->
  <div class="px-6 py-4 border-t border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-900 flex justify-end gap-3">
    <button class="px-4 py-2 text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-md">
      Cancel
    </button>
    <button class="px-4 py-2 bg-blue-600 text-white hover:bg-blue-700 rounded-md">
      Save
    </button>
  </div>
</div>
```

### Form Components

**Input Field**
```html
<div class="space-y-2">
  <label for="name" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
    Name
  </label>
  <input 
    type="text" 
    id="name"
    class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm
           focus:ring-2 focus:ring-blue-500 focus:border-blue-500
           dark:bg-gray-700 dark:text-white
           disabled:bg-gray-100 disabled:cursor-not-allowed
           placeholder:text-gray-400"
    placeholder="Enter name">
  
  <!-- Error state -->
  <p class="text-sm text-red-600 dark:text-red-400">This field is required</p>
</div>
```

**Textarea**
```html
<div class="space-y-2">
  <label for="description" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
    Description
  </label>
  <textarea 
    id="description"
    rows="4"
    class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm
           focus:ring-2 focus:ring-blue-500 focus:border-blue-500
           dark:bg-gray-700 dark:text-white
           resize-none"
    placeholder="Enter description"></textarea>
</div>
```

**Select Dropdown**
```html
<div class="space-y-2">
  <label for="sport" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
    Sport
  </label>
  <select 
    id="sport"
    class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm
           focus:ring-2 focus:ring-blue-500 focus:border-blue-500
           dark:bg-gray-700 dark:text-white
           cursor-pointer">
    <option value="">Select a sport</option>
    <option value="football">Football</option>
    <option value="basketball">Basketball</option>
  </select>
</div>
```

**Checkbox**
```html
<div class="flex items-center">
  <input 
    type="checkbox" 
    id="terms"
    class="w-4 h-4 text-blue-600 border-gray-300 rounded
           focus:ring-2 focus:ring-blue-500
           dark:border-gray-600 dark:bg-gray-700">
  <label for="terms" class="ml-2 text-sm text-gray-700 dark:text-gray-300">
    I agree to the terms and conditions
  </label>
</div>
```

### Modal Component

```html
<!-- Backdrop -->
<div 
  class="fixed inset-0 bg-black bg-opacity-50 z-40 transition-opacity"
  (click)="closeModal()">
</div>

<!-- Modal -->
<div class="fixed inset-0 z-50 flex items-center justify-center p-4">
  <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full transform transition-all">
    
    <!-- Header -->
    <div class="flex items-center justify-between px-6 py-4 border-b border-gray-200 dark:border-gray-700">
      <h2 class="text-xl font-semibold text-gray-900 dark:text-white">Modal Title</h2>
      <button 
        (click)="closeModal()"
        class="p-1 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 rounded-md
               focus:outline-none focus:ring-2 focus:ring-blue-500">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
        </svg>
      </button>
    </div>
    
    <!-- Body -->
    <div class="px-6 py-4">
      <p class="text-gray-600 dark:text-gray-300">Modal content</p>
    </div>
    
    <!-- Footer -->
    <div class="flex justify-end gap-3 px-6 py-4 border-t border-gray-200 dark:border-gray-700">
      <button class="px-4 py-2 text-gray-700 dark:text-gray-300 hover:bg-gray-100 rounded-md">Cancel</button>
      <button class="px-4 py-2 bg-blue-600 text-white hover:bg-blue-700 rounded-md">Confirm</button>
    </div>
  </div>
</div>
```

### Alert Components

**Success Alert**
```html
<div class="flex items-start gap-3 p-4 bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded-lg">
  <svg class="w-5 h-5 text-green-600 dark:text-green-400 flex-shrink-0 mt-0.5" fill="currentColor" viewBox="0 0 20 20">
    <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
  </svg>
  <div class="flex-1">
    <h3 class="text-sm font-medium text-green-800 dark:text-green-300">Success</h3>
    <p class="text-sm text-green-700 dark:text-green-400 mt-1">Changes saved successfully.</p>
  </div>
</div>
```

**Error Alert**
```html
<div class="flex items-start gap-3 p-4 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg">
  <svg class="w-5 h-5 text-red-600 dark:text-red-400 flex-shrink-0 mt-0.5" fill="currentColor" viewBox="0 0 20 20">
    <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
  </svg>
  <div class="flex-1">
    <h3 class="text-sm font-medium text-red-800 dark:text-red-300">Error</h3>
    <p class="text-sm text-red-700 dark:text-red-400 mt-1">Problem processing request.</p>
  </div>
</div>
```

### Loading States

**Spinner**
```html
<div class="flex items-center justify-center">
  <div class="w-8 h-8 border-4 border-blue-200 border-t-blue-600 rounded-full animate-spin"></div>
</div>
```

**Skeleton Loader**
```html
<div class="animate-pulse space-y-4">
  <div class="h-4 bg-gray-200 dark:bg-gray-700 rounded w-3/4"></div>
  <div class="h-4 bg-gray-200 dark:bg-gray-700 rounded w-full"></div>
  <div class="h-4 bg-gray-200 dark:bg-gray-700 rounded w-5/6"></div>
</div>
```

---

## 🔐 Authentication & Supabase

### Supabase Client Setup

```typescript
// core/auth/auth.service.ts
import { Injectable } from '@angular/core';
import { createClient, SupabaseClient, Session } from '@supabase/supabase-js';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private supabase: SupabaseClient;
  
  constructor() {
    this.supabase = createClient(
      environment.supabaseUrl,
      environment.supabaseAnonKey
    );
  }
  
  async signIn(email: string, password: string) {
    const { data, error } = await this.supabase.auth.signInWithPassword({
      email,
      password
    });
    return { data, error };
  }
  
  async signUp(email: string, password: string) {
    const { data, error } = await this.supabase.auth.signUp({
      email,
      password
    });
    return { data, error };
  }
  
  async signOut() {
    await this.supabase.auth.signOut();
  }
  
  async getSession(): Promise<Session | null> {
    const { data } = await this.supabase.auth.getSession();
    return data.session;
  }
  
  getAccessToken(): string | null {
    // Implementation to get current token
    return null;
  }
}
```

### Auth Guards

```typescript
// core/guards/auth.guard.ts
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../auth/auth.service';

export const authGuard: CanActivateFn = async (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  const session = await authService.getSession();
  
  if (!session) {
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }
  
  return true;
};
```

### HTTP Interceptor (JWT)

```typescript
// core/interceptors/auth.interceptor.ts
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../auth/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getAccessToken();
  
  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }
  
  return next(req);
};
```

---

## 🔔 Global Notifications

**Location**: `src/app/shared/notifications/`

### Usage in Components

```typescript
export class MyComponent {
  private readonly ns = inject(NotificationService);
  
  async saveData() {
    try {
      await this.api.save();
      this.ns.success('Data saved successfully', 'Success');
    } catch (error) {
      this.ns.error('Failed to save data', 'Error');
    }
  }
}
```

### Notification Service API

```typescript
ns.success(message: string, title?: string);
ns.error(message: string, title?: string);
ns.warning(message: string, title?: string);
ns.info(message: string, title?: string);
```

---

## 🧪 Testing Standards

### Frontend Testing Framework
- **Unit Tests**: Jasmine + Karma
- **Coverage Target**: >80% for critical components and services
- **Test Structure**: AAA pattern (Arrange, Act, Assert)
- **Naming**: `describe('ComponentName', () => { it('should do something', () => {}) })`

### Unit Tests (Jasmine + Karma)

```typescript
describe('TrainingListComponent', () => {
  let component: TrainingListComponent;
  let fixture: ComponentFixture<TrainingListComponent>;
  let mockTrainingService: jasmine.SpyObj<TrainingService>;
  
  beforeEach(async () => {
    // Arrange: Create mocks
    mockTrainingService = jasmine.createSpyObj('TrainingService', ['getTrainings', 'deleteTraining']);
    
    await TestBed.configureTestingModule({
      imports: [TrainingListComponent], // Standalone component
      providers: [
        { provide: TrainingService, useValue: mockTrainingService }
      ]
    }).compileComponents();
    
    fixture = TestBed.createComponent(TrainingListComponent);
    component = fixture.componentInstance;
  });
  
  it('should load trainings on init', async () => {
    // Arrange
    const mockTrainings = [
      { id: 1, name: 'Test Training', description: 'Test' }
    ];
    mockTrainingService.getTrainings.and.returnValue(Promise.resolve(mockTrainings));
    
    // Act
    await component.loadTrainings();
    
    // Assert
    expect(component.trainings()).toEqual(mockTrainings);
    expect(component.isLoading()).toBe(false);
  });
  
  it('should handle error when loading fails', async () => {
    // Arrange
    mockTrainingService.getTrainings.and.returnValue(Promise.reject('Error'));
    
    // Act
    await component.loadTrainings();
    
    // Assert
    expect(component.isLoading()).toBe(false);
    expect(component.trainings().length).toBe(0);
  });
});
```

### Mocking Supabase

```typescript
describe('AuthService', () => {
  let service: AuthService;
  let mockSupabase: jasmine.SpyObj<SupabaseClient>;
  
  beforeEach(() => {
    mockSupabase = {
      auth: {
        signInWithPassword: jasmine.createSpy().and.returnValue(
          Promise.resolve({ 
            data: { session: { access_token: 'token' } }, 
            error: null 
          })
        ),
        getSession: jasmine.createSpy().and.returnValue(
          Promise.resolve({ 
            data: { session: { access_token: 'token' } }, 
            error: null 
          })
        ),
        signOut: jasmine.createSpy().and.returnValue(Promise.resolve())
      }
    } as any;
    
    // Inject mock into service
  });
  
  it('should sign in successfully', async () => {
    const result = await service.signIn('test@example.com', 'password');
    expect(result.data).toBeDefined();
    expect(result.error).toBeNull();
  });
});
```

### Component Testing Best Practices

```typescript
// Test Signals
it('should update signal value', () => {
  component.trainings.set([{ id: 1, name: 'Test' }]);
  expect(component.trainings().length).toBe(1);
  expect(component.trainingCount()).toBe(1);
});

// Test computed signals
it('should compute hasTrainings correctly', () => {
  expect(component.hasTrainings()).toBe(false);
  component.trainings.set([{ id: 1, name: 'Test' }]);
  expect(component.hasTrainings()).toBe(true);
});

// Test async operations
it('should handle async loading', fakeAsync(() => {
  component.loadTrainings();
  expect(component.isLoading()).toBe(true);
  
  tick(); // Simulate async completion
  
  expect(component.isLoading()).toBe(false);
}));
```

---

## 📦 State Management

### Service with Signals (Shared State)

```typescript
@Injectable({ providedIn: 'root' })
export class TrainingStateService {
  private readonly http = inject(HttpClient);
  
  // Private writable signals
  private readonly _trainings = signal<Training[]>([]);
  private readonly _loading = signal(false);
  private readonly _error = signal<string | null>(null);
  
  // Public read-only signals
  readonly trainings = this._trainings.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  
  // Computed signals
  readonly trainingCount = computed(() => this._trainings().length);
  readonly hasTrainings = computed(() => this._trainings().length > 0);
  
  // Actions
  async loadTrainings() {
    this._loading.set(true);
    this._error.set(null);
    
    try {
      const data = await firstValueFrom(
        this.http.get<Training[]>('/api/trainings')
      );
      this._trainings.set(data);
    } catch (error) {
      this._error.set('Failed to load trainings');
      console.error(error);
    } finally {
      this._loading.set(false);
    }
  }
  
  addTraining(training: Training) {
    this._trainings.update(list => [...list, training]);
  }
  
  updateTraining(id: number, updates: Partial<Training>) {
    this._trainings.update(list =>
      list.map(t => t.id === id ? { ...t, ...updates } : t)
    );
  }
  
  deleteTraining(id: number) {
    this._trainings.update(list => list.filter(t => t.id !== id));
  }
}
```

### Using Shared State in Components

```typescript
export class TrainingListComponent implements OnInit {
  private readonly stateService = inject(TrainingStateService);
  
  // Access shared state
  trainings = this.stateService.trainings;
  loading = this.stateService.loading;
  trainingCount = this.stateService.trainingCount;
  
  async ngOnInit() {
    await this.stateService.loadTrainings();
  }
  
  async handleDelete(id: number) {
    const confirmed = confirm('Delete this training?');
    if (confirmed) {
      this.stateService.deleteTraining(id);
    }
  }
}
```

---

## 🛡️ Security Best Practices

### Input Validation
- Validate all user inputs in components and forms
- Use Angular Forms with built-in validators
- Implement custom validators for business rules
- Sanitize HTML when rendering dynamic content

```typescript
// Form validation example
form = this.fb.group({
  email: ['', [Validators.required, Validators.email]],
  password: ['', [Validators.required, Validators.minLength(8)]],
  name: ['', [Validators.required, Validators.pattern(/^[a-zA-Z ]+$/)]]
});
```

### Environment Variables
- **Never commit** `environment.ts` with real secrets
- Use `environment.development.ts` for local dev
- Production secrets via CI/CD environment variables
- Use `.gitignore` to exclude `environment.ts`

```typescript
// environment.development.ts (safe to commit)
export const environment = {
  production: false,
  supabaseUrl: 'https://your-project.supabase.co',
  supabaseAnonKey: 'your-anon-key-here' // Public key, safe to expose
};
```

### XSS Prevention
- Angular sanitizes by default (no need to escape manually)
- Avoid `bypassSecurityTrustHtml` unless absolutely necessary
- Use `DomSanitizer` when rendering user-generated HTML

```typescript
// Safe: Angular sanitizes automatically
<div>{{ userInput }}</div>

// Unsafe: Only if you trust the source
<div [innerHTML]="sanitizer.sanitize(SecurityContext.HTML, userInput)"></div>
```

### CSRF Protection
- Backend uses token-based auth (JWT from Supabase)
- No cookie-based sessions = no CSRF vulnerability
- Always send JWT in Authorization header

---

## 🎯 Common Patterns

### API Service Pattern

```typescript
@Injectable({ providedIn: 'root' })
export class TrainingApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/trainings';
  
  getAll(): Observable<Training[]> {
    return this.http.get<Training[]>(this.baseUrl);
  }
  
  getById(id: number): Observable<Training> {
    return this.http.get<Training>(`${this.baseUrl}/${id}`);
  }
  
  create(training: CreateTrainingDto): Observable<Training> {
    return this.http.post<Training>(this.baseUrl, training);
  }
  
  update(id: number, training: UpdateTrainingDto): Observable<Training> {
    return this.http.put<Training>(`${this.baseUrl}/${id}`, training);
  }
  
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
```

### Form Handling Pattern

```typescript
export class TrainingFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly api = inject(TrainingApiService);
  private readonly ns = inject(NotificationService);
  
  form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    description: [''],
    duration: [0, [Validators.required, Validators.min(1)]],
    sport: ['', Validators.required]
  });
  
  isSubmitting = signal(false);
  
  async onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    
    this.isSubmitting.set(true);
    
    try {
      const value = this.form.getRawValue();
      await firstValueFrom(this.api.create(value));
      this.ns.success('Training created successfully');
      this.router.navigate(['/training']);
    } catch (error) {
      this.ns.error('Failed to create training');
      console.error(error);
    } finally {
      this.isSubmitting.set(false);
    }
  }
  
  // Helper for template
  hasError(field: string, error: string): boolean {
    const control = this.form.get(field);
    return !!(control && control.hasError(error) && control.touched);
  }
}
```

---

## ✅ Pre-Commit Checklist

Before committing code, verify:

- [ ] All components are standalone (`standalone: true`)
- [ ] Using `inject()` for DI (not constructor injection)
- [ ] Using Signals for reactive state (not BehaviorSubject for primary state)
- [ ] Proper lazy loading with `loadComponent` or `loadChildren`
- [ ] **NO Angular Material imports** (use Tailwind only)
- [ ] No hardcoded secrets or API keys in code
- [ ] Tests added/updated for new features
- [ ] No `console.log` or `debugger` statements
- [ ] Tailwind classes for styling (not inline styles)
- [ ] Icons from Heroicons/Lucide (not Material Icons)
- [ ] Imports are feature-specific (not global)
- [ ] Error handling with try/catch + notifications
- [ ] Forms use Angular Reactive Forms with validation
- [ ] All user inputs are validated
- [ ] Component files follow naming convention (`.component.ts`, `.component.html`)

---

## 🚫 Common Mistakes to Avoid

❌ Using NgModules (declarations, imports in @NgModule)  
❌ Constructor injection instead of `inject()`  
❌ BehaviorSubject/Subject for primary state (use Signals)  
❌ **Importing ANY Angular Material module** (MatCardModule, MatButtonModule, etc.)  
❌ **Using Material Icons** (use Heroicons or Lucide)  
❌ Hardcoded API URLs (use environment)  
❌ Inline styles instead of Tailwind utilities  
❌ Direct DOM manipulation (use Angular directives)  
❌ Mutable state operations (use `.set()` or `.update()`)  
❌ Custom CSS when Tailwind utilities exist  
❌ Committing `environment.ts` with real secrets  
❌ Missing error handling in async operations  
❌ Not using try/catch with async/await  
❌ Missing loading states in components  

---

## 📚 Key Files to Reference

- **App Config**: `src/app/app.config.ts` (providers, interceptors)
- **Root Component**: `src/app/app.component.ts`
- **Root Routes**: `src/app/app.routes.ts`
- **Auth Service**: `src/app/core/auth/auth.service.ts`
- **Notification Service**: `src/app/shared/notifications/notification.service.ts`
- **Environment**: `src/environments/environment.ts` (DO NOT COMMIT WITH SECRETS)
- **General Agent**: `../agent.md` (cross-cutting concerns, security, naming)
- **Backend Agent**: `../back/agent.md` (API contracts, data models)

---

**Last Updated**: 2025-10-06  
**Version**: 2.0 (Self-contained, no external dependencies)
