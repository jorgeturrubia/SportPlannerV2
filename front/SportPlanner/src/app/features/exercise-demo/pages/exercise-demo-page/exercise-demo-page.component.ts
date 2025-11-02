import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExerciseAnimationPlayerComponent } from '../../components/exercise-animation-player/exercise-animation-player.component';
import { ExerciseSelectorComponent } from '../../components/exercise-selector/exercise-selector.component';
import { ExerciseAnimation } from '../../models/exercise-animation.model';
import { createBasketballDribbleDrill } from '../../data/basketball-exercises';

@Component({
  selector: 'app-exercise-demo-page',
  standalone: true,
  imports: [CommonModule, ExerciseAnimationPlayerComponent, ExerciseSelectorComponent],
  template: `
    <div class="exercise-demo-page">
      <!-- Page Header -->
      <div class="page-header">
        <div class="header-content">
          <div class="flex items-center gap-3">
            <div class="icon-wrapper">
              <svg class="w-8 h-8 text-white" fill="currentColor" viewBox="0 0 20 20">
                <path d="M10 12a2 2 0 100-4 2 2 0 000 4z"/>
                <path fill-rule="evenodd" d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z" clip-rule="evenodd"/>
              </svg>
            </div>
            <div>
              <h1 class="page-title">Visualización de Ejercicios</h1>
              <p class="page-subtitle">Sistema avanzado de animación para ejercicios deportivos</p>
            </div>
          </div>

          <!-- Toggle View Buttons -->
          <div class="view-toggle-buttons">
            <button 
              class="toggle-btn"
              [class.active]="showSelector()"
              (click)="showSelector.set(!showSelector())">
              <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path d="M9 2a1 1 0 000 2h2a1 1 0 100-2H9z"/>
                <path fill-rule="evenodd" d="M4 5a2 2 0 012-2 3 3 0 003 3h2a3 3 0 003-3 2 2 0 012 2v11a2 2 0 01-2 2H6a2 2 0 01-2-2V5zm3 4a1 1 0 000 2h.01a1 1 0 100-2H7zm3 0a1 1 0 000 2h3a1 1 0 100-2h-3zm-3 4a1 1 0 100 2h.01a1 1 0 100-2H7zm3 0a1 1 0 100 2h3a1 1 0 100-2h-3z" clip-rule="evenodd"/>
              </svg>
              {{ showSelector() ? 'Ocultar' : 'Mostrar' }} Selector
            </button>
            <button 
              class="toggle-btn"
              [class.active]="!showSelector()"
              (click)="loadExercise('basketball-dribble')">
              <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM9.555 7.168A1 1 0 008 8v4a1 1 0 001.555.832l3-2a1 1 0 000-1.664l-3-2z" clip-rule="evenodd"/>
              </svg>
              Ejemplo Demo
            </button>
          </div>
        </div>
      </div>

      <!-- Exercise Selector -->
      @if (showSelector()) {
        <div class="selector-wrapper">
          <app-exercise-selector 
            (exerciseSelected)="onExerciseSelected($event)" />
        </div>
      }

      <!-- Animation Player -->
      <div class="player-wrapper">
        @if (currentExercise()) {
          <app-exercise-animation-player 
            [exercise]="currentExercise()!"
            [autoPlay]="autoPlayEnabled()" />
        } @else {
          <div class="loading-state">
            <div class="loading-spinner"></div>
            <p class="loading-text">Selecciona un ejercicio para visualizarlo</p>
          </div>
        }
      </div>

      <!-- Info Cards -->
      <div class="info-cards-grid">
        <div class="info-card">
          <div class="info-card-icon bg-blue-500">
            <svg class="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 20 20">
              <path d="M9 2a1 1 0 000 2h2a1 1 0 100-2H9z"/>
              <path fill-rule="evenodd" d="M4 5a2 2 0 012-2 3 3 0 003 3h2a3 3 0 003-3 2 2 0 012 2v11a2 2 0 01-2 2H6a2 2 0 01-2-2V5zm3 4a1 1 0 000 2h.01a1 1 0 100-2H7zm3 0a1 1 0 000 2h3a1 1 0 100-2h-3zm-3 4a1 1 0 100 2h.01a1 1 0 100-2H7zm3 0a1 1 0 100 2h3a1 1 0 100-2h-3z" clip-rule="evenodd"/>
            </svg>
          </div>
          <div class="info-card-content">
            <h3 class="info-card-title">Sistema Basado en Frames</h3>
            <p class="info-card-description">
              Cada ejercicio está construido como una secuencia de frames interpolados suavemente
            </p>
          </div>
        </div>

        <div class="info-card">
          <div class="info-card-icon bg-green-500">
            <svg class="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M11.3 1.046A1 1 0 0112 2v5h4a1 1 0 01.82 1.573l-7 10A1 1 0 018 18v-5H4a1 1 0 01-.82-1.573l7-10a1 1 0 011.12-.38z" clip-rule="evenodd"/>
            </svg>
          </div>
          <div class="info-card-content">
            <h3 class="info-card-title">Animaciones Profesionales</h3>
            <p class="info-card-description">
              Efectos visuales avanzados con SVG: trails, glows, shadows y easing functions
            </p>
          </div>
        </div>

        <div class="info-card">
          <div class="info-card-icon bg-purple-500">
            <svg class="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 20 20">
              <path d="M13 6a3 3 0 11-6 0 3 3 0 016 0zM18 8a2 2 0 11-4 0 2 2 0 014 0zM14 15a4 4 0 00-8 0v3h8v-3zM6 8a2 2 0 11-4 0 2 2 0 014 0zM16 18v-3a5.972 5.972 0 00-.75-2.906A3.005 3.005 0 0119 15v3h-3zM4.75 12.094A5.973 5.973 0 004 15v3H1v-3a3 3 0 013.75-2.906z"/>
            </svg>
          </div>
          <div class="info-card-content">
            <h3 class="info-card-title">IA-Ready</h3>
            <p class="info-card-description">
              Estructura diseñada para que una IA pueda generar ejercicios desde descripciones de texto
            </p>
          </div>
        </div>

        <div class="info-card">
          <div class="info-card-icon bg-orange-500">
            <svg class="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M12.316 3.051a1 1 0 01.633 1.265l-4 12a1 1 0 11-1.898-.632l4-12a1 1 0 011.265-.633zM5.707 6.293a1 1 0 010 1.414L3.414 10l2.293 2.293a1 1 0 11-1.414 1.414l-3-3a1 1 0 010-1.414l3-3a1 1 0 011.414 0zm8.586 0a1 1 0 011.414 0l3 3a1 1 0 010 1.414l-3 3a1 1 0 11-1.414-1.414L16.586 10l-2.293-2.293a1 1 0 010-1.414z" clip-rule="evenodd"/>
            </svg>
          </div>
          <div class="info-card-content">
            <h3 class="info-card-title">Extensible y Modular</h3>
            <p class="info-card-description">
              Fácil de añadir nuevos deportes, elementos y tipos de animaciones
            </p>
          </div>
        </div>
      </div>

      <!-- Technical Info -->
      <div class="technical-info">
        <h3 class="technical-title">💡 Tecnologías Utilizadas</h3>
        <div class="tech-tags">
          <span class="tech-tag">Angular 20 Signals</span>
          <span class="tech-tag">SVG Animations</span>
          <span class="tech-tag">Tailwind CSS v4</span>
          <span class="tech-tag">RequestAnimationFrame</span>
          <span class="tech-tag">Easing Functions</span>
          <span class="tech-tag">Interpolación</span>
        </div>
        
        <div class="mt-4 p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg border border-blue-200 dark:border-blue-800">
          <p class="text-sm text-blue-800 dark:text-blue-200">
            <strong>🎯 Próximos pasos:</strong> Integración con IA para generar ejercicios automáticamente desde descripciones en lenguaje natural, 
            sistema de persistencia en base de datos, editor visual de ejercicios, y soporte para múltiples deportes.
          </p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    @import "tailwindcss" reference;
    
    .exercise-demo-page {
      @apply space-y-8 p-6 max-w-7xl mx-auto;
    }

    .page-header {
      @apply bg-gradient-to-br from-blue-50 to-indigo-50 dark:from-slate-800 dark:to-slate-900 
             rounded-2xl p-8 shadow-lg border border-blue-200 dark:border-slate-700;
    }

    .header-content {
      @apply space-y-6;
    }

    .icon-wrapper {
      @apply p-3 bg-blue-500 dark:bg-blue-600 rounded-xl shadow-md;
    }

    .page-title {
      @apply text-3xl md:text-4xl font-bold text-slate-900 dark:text-white;
    }

    .page-subtitle {
      @apply text-slate-700 dark:text-slate-300 mt-1;
    }

    .view-toggle-buttons {
      @apply flex flex-wrap gap-3;
    }

    .toggle-btn {
      @apply flex items-center gap-2 px-4 py-2 rounded-lg font-medium
             bg-white dark:bg-slate-800 text-slate-700 dark:text-slate-300
             border border-slate-300 dark:border-slate-700
             hover:bg-blue-50 dark:hover:bg-blue-900/20
             hover:border-blue-300 dark:hover:border-blue-700
             transition-all duration-200;
    }

    .toggle-btn.active {
      @apply bg-blue-600 text-white border-blue-600
             hover:bg-blue-700 hover:border-blue-700
             shadow-md;
    }

    .selector-wrapper {
      @apply bg-white dark:bg-slate-800 rounded-xl p-6 shadow-md
             border border-slate-200 dark:border-slate-700;
    }

    .player-wrapper {
      @apply min-h-[600px];
    }

    .loading-state {
      @apply flex flex-col items-center justify-center h-[600px] 
             bg-slate-50 dark:bg-slate-900 rounded-xl;
    }

    .loading-spinner {
      @apply w-12 h-12 border-4 border-blue-200 border-t-blue-600 rounded-full animate-spin;
    }

    .loading-text {
      @apply mt-4 text-slate-600 dark:text-slate-400 font-medium;
    }

    .info-cards-grid {
      @apply grid grid-cols-1 md:grid-cols-2 gap-6;
    }

    .info-card {
      @apply flex gap-4 p-6 bg-white dark:bg-slate-800 rounded-xl shadow-md
             border border-slate-200 dark:border-slate-700
             hover:shadow-lg transition-shadow duration-200;
    }

    .info-card-icon {
      @apply flex-shrink-0 w-12 h-12 rounded-lg flex items-center justify-center;
    }

    .info-card-content {
      @apply flex-1;
    }

    .info-card-title {
      @apply text-lg font-semibold text-slate-900 dark:text-white mb-1;
    }

    .info-card-description {
      @apply text-sm text-slate-600 dark:text-slate-400;
    }

    .technical-info {
      @apply bg-white dark:bg-slate-800 rounded-xl p-6 shadow-md
             border border-slate-200 dark:border-slate-700;
    }

    .technical-title {
      @apply text-xl font-bold text-slate-900 dark:text-white mb-4;
    }

    .tech-tags {
      @apply flex flex-wrap gap-2;
    }

    .tech-tag {
      @apply px-3 py-1 bg-slate-100 dark:bg-slate-700 text-slate-700 dark:text-slate-300
             rounded-full text-sm font-medium;
    }
  `]
})
export class ExerciseDemoPageComponent {
  currentExercise = signal<ExerciseAnimation | null>(null);
  showSelector = signal(true); // Mostrar el selector por defecto
  autoPlayEnabled = signal(false); // No autoplay hasta que se cargue un ejercicio

  constructor() {
    // No cargar ejercicio por defecto, esperar a que el usuario seleccione uno
  }

  loadExercise(exerciseId: string): void {
    // Cargar ejercicio de ejemplo
    if (exerciseId === 'basketball-dribble') {
      this.currentExercise.set(createBasketballDribbleDrill());
      this.showSelector.set(false); // Ocultar selector cuando se carga el demo
      this.autoPlayEnabled.set(true);
    }
  }

  onExerciseSelected(animation: ExerciseAnimation): void {
    this.currentExercise.set(animation);
    this.autoPlayEnabled.set(true);
  }
}
