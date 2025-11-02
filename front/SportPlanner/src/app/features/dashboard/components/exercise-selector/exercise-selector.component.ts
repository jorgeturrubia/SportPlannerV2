import { Component, input, output, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExerciseDto } from '../../services/exercises.service';

export interface SelectedExercise {
  exerciseId: string;
  order: number;
  sets?: number;
  reps?: number;
  durationSeconds?: number;
  restSeconds?: number;
  notes?: string;
}

@Component({
  selector: 'app-exercise-selector',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './exercise-selector.component.html',
  styleUrls: ['./exercise-selector.component.css']
})
export class ExerciseSelectorComponent {
  // Inputs
  availableExercises = input<ExerciseDto[]>([]);
  selectedExercises = input<SelectedExercise[]>([]);
  
  // Outputs
  exercisesChange = output<SelectedExercise[]>();
  
  // Internal state
  searchTerm = signal('');
  showAvailablePanel = signal(false);
  editingExercise = signal<SelectedExercise | null>(null);
  
  // Computed
  filteredExercises = computed(() => {
    const term = this.searchTerm().toLowerCase();
    const selectedIds = this.selectedExercises().map(e => e.exerciseId);
    
    return this.availableExercises()
      .filter(ex => !selectedIds.includes(ex.id))
      .filter(ex => 
        ex.name.toLowerCase().includes(term) || 
        (ex.description?.toLowerCase().includes(term) ?? false)
      );
  });
  
  selectedWithDetails = computed(() => {
    const exercises = this.availableExercises();
    return this.selectedExercises()
      .sort((a, b) => a.order - b.order)
      .map(selected => {
        const exercise = exercises.find(ex => ex.id === selected.exerciseId);
        return {
          ...selected,
          name: exercise?.name ?? 'Unknown',
          description: exercise?.description ?? '',
          videoUrl: exercise?.videoUrl
        };
      });
  });
  
  addExercise(exercise: ExerciseDto): void {
    const current = [...this.selectedExercises()];
    const maxOrder = current.length > 0 ? Math.max(...current.map(e => e.order)) : 0;
    
    const newExercise: SelectedExercise = {
      exerciseId: exercise.id,
      order: maxOrder + 1,
      sets: exercise.defaultSets ?? 3,
      reps: exercise.defaultReps ?? 10,
      durationSeconds: exercise.defaultDurationSeconds ?? 300,
      restSeconds: 60,
      notes: ''
    };
    
    this.exercisesChange.emit([...current, newExercise]);
    this.searchTerm.set('');
    this.showAvailablePanel.set(false);
  }
  
  removeExercise(exerciseId: string): void {
    const current = this.selectedExercises().filter(e => e.exerciseId !== exerciseId);
    // Reorder
    const reordered = current.map((ex, idx) => ({ ...ex, order: idx + 1 }));
    this.exercisesChange.emit(reordered);
  }
  
  moveUp(exerciseId: string): void {
    const current = [...this.selectedExercises()].sort((a, b) => a.order - b.order);
    const index = current.findIndex(e => e.exerciseId === exerciseId);
    
    if (index > 0) {
      [current[index], current[index - 1]] = [current[index - 1], current[index]];
      const reordered = current.map((ex, idx) => ({ ...ex, order: idx + 1 }));
      this.exercisesChange.emit(reordered);
    }
  }
  
  moveDown(exerciseId: string): void {
    const current = [...this.selectedExercises()].sort((a, b) => a.order - b.order);
    const index = current.findIndex(e => e.exerciseId === exerciseId);
    
    if (index < current.length - 1) {
      [current[index], current[index + 1]] = [current[index + 1], current[index]];
      const reordered = current.map((ex, idx) => ({ ...ex, order: idx + 1 }));
      this.exercisesChange.emit(reordered);
    }
  }
  
  openEditDialog(exercise: SelectedExercise): void {
    this.editingExercise.set({ ...exercise });
  }
  
  closeEditDialog(): void {
    this.editingExercise.set(null);
  }
  
  saveEdit(): void {
    const edited = this.editingExercise();
    if (!edited) return;
    
    const current = this.selectedExercises().map(ex => 
      ex.exerciseId === edited.exerciseId ? edited : ex
    );
    
    this.exercisesChange.emit(current);
    this.closeEditDialog();
  }
  
  toggleAvailablePanel(): void {
    this.showAvailablePanel.update(v => !v);
  }
  
  formatDuration(seconds?: number): string {
    if (!seconds) return '0:00';
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  }
}
