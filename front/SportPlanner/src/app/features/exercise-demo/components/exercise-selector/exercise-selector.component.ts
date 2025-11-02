import { Component, inject, signal, output, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExercisesService, ExerciseDto } from '../../../dashboard/services/exercises.service';
import { ExerciseAnimation } from '../../models/exercise-animation.model';

@Component({
  selector: 'app-exercise-selector',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './exercise-selector.component.html',
  styles: [`
    .line-clamp-3 {
      display: -webkit-box;
      -webkit-line-clamp: 3;
      -webkit-box-orient: vertical;
      overflow: hidden;
    }

    /* Custom scrollbar */
    .exercises-list::-webkit-scrollbar {
      width: 8px;
    }

    .exercises-list::-webkit-scrollbar-track {
      background-color: rgb(241 245 249);
      border-radius: 0.25rem;
    }

    .dark .exercises-list::-webkit-scrollbar-track {
      background-color: rgb(30 41 59);
    }

    .exercises-list::-webkit-scrollbar-thumb {
      background-color: rgb(203 213 225);
      border-radius: 0.25rem;
    }

    .dark .exercises-list::-webkit-scrollbar-thumb {
      background-color: rgb(71 85 105);
    }

    .exercises-list::-webkit-scrollbar-thumb:hover {
      background-color: rgb(148 163 184);
    }

    .dark .exercises-list::-webkit-scrollbar-thumb:hover {
      background-color: rgb(100 116 139);
    }
  `]
})
export class ExerciseSelectorComponent {
  private exercisesService = inject(ExercisesService);

  // State
  exercises = signal<ExerciseDto[]>([]);
  selectedExerciseId = signal<string | null>(null);
  isLoading = signal(false);
  error = signal<string | null>(null);
  searchTerm = signal('');

  // Output
  exerciseSelected = output<ExerciseAnimation>();

  // Computed
  filteredExercises = computed(() => {
    const term = this.searchTerm().toLowerCase();
    return this.exercises().filter(ex => 
      ex.name.toLowerCase().includes(term) || 
      ex.description.toLowerCase().includes(term)
    );
  });

  exercisesWithAnimation = computed(() => {
    return this.filteredExercises().filter(ex => ex.animationJson);
  });

  selectedExercise = computed(() => {
    const id = this.selectedExerciseId();
    return id ? this.exercises().find(ex => ex.id === id) : null;
  });

  async ngOnInit() {
    await this.loadExercises();
  }

  async loadExercises() {
    this.isLoading.set(true);
    this.error.set(null);
    
    try {
      const exercises = await this.exercisesService.getExercises();
      console.log('📋 Ejercicios cargados:', exercises.length);
      console.log('🎬 Ejercicios con animación:', exercises.filter(ex => ex.animationJson).length);
      console.log('📊 Detalle ejercicios:', exercises.map(ex => ({
        id: ex.id,
        name: ex.name,
        hasAnimation: !!ex.animationJson,
        animationLength: ex.animationJson?.length || 0
      })));
      this.exercises.set(exercises);
    } catch (err) {
      console.error('❌ Error loading exercises:', err);
      this.error.set('Error al cargar los ejercicios. Por favor, intenta de nuevo.');
    } finally {
      this.isLoading.set(false);
    }
  }

  onExerciseSelect(exerciseId: string) {
    this.selectedExerciseId.set(exerciseId);
    
    const exercise = this.exercises().find(ex => ex.id === exerciseId);
    
    if (!exercise) {
      console.error('❌ Ejercicio no encontrado:', exerciseId);
      return;
    }

    console.log('✅ Ejercicio seleccionado:', exercise.name);
    console.log('🎬 Tiene animación:', !!exercise.animationJson);
    
    if (!exercise.animationJson) {
      this.error.set('Este ejercicio no tiene una animación configurada. Por favor, añade datos de animación.');
      console.warn('⚠️ El ejercicio no tiene animationJson');
      return;
    }
    
    try {
      console.log('📝 Parseando JSON de animación...');
      const animation = JSON.parse(exercise.animationJson) as ExerciseAnimation;
      console.log('✅ Animación parseada correctamente:', animation.name);
      this.exerciseSelected.emit(animation);
      this.error.set(null); // Limpiar error previo
    } catch (err) {
      console.error('❌ Error parsing animation JSON:', err);
      console.error('❌ JSON problemático:', exercise.animationJson?.substring(0, 200));
      this.error.set('Error al cargar la animación del ejercicio. El formato JSON es inválido.');
    }
  }

  onSearchChange(term: string) {
    this.searchTerm.set(term);
  }

  clearSelection() {
    this.selectedExerciseId.set(null);
  }
}
