import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/home/pages/home-page/home-page').then(m => m.HomePage)
  },
  {
    path: 'auth',
    loadComponent: () => import('./features/auth/pages/auth-page/auth-page').then(m => m.AuthPage)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/layouts/dashboard-layout/dashboard-layout').then(m => m.DashboardLayout),
    children: [
      {
        path: '',
        loadComponent: () => import('./features/dashboard/pages/dashboard-home/dashboard-home').then(m => m.DashboardHome)
      }
      ,
      {
        path: 'teams',
        loadComponent: () => import('./features/dashboard/pages/teams-page/teams-page').then(m => m.TeamsPage)
      }
    ]
  }
];
