import { Routes } from '@angular/router';

export const EXERCISE_DEMO_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/exercise-demo-page/exercise-demo-page.component').then(
        (m) => m.ExerciseDemoPageComponent
      ),
  },
];
