import { Routes } from '@angular/router';
import { authGuard } from './core/auth/guards/auth.guard';
import { subscriptionGuard } from './core/subscription/guards/subscription.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/home/pages/home-page/home-page').then(m => m.HomePage)
  },
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.authRoutes)
  },
  {
    path: 'subscription/select',
    loadComponent: () => import('./features/subscription/pages/subscription-selection-page/subscription-selection-page').then(m => m.SubscriptionSelectionPage),
    canActivate: [authGuard] // Requires auth but NOT subscription
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/layouts/dashboard-layout/dashboard-layout').then(m => m.DashboardLayout),
    canActivate: [authGuard, subscriptionGuard], // Requires auth AND subscription
    children: [
      {
        path: '',
        loadComponent: () => import('./features/dashboard/pages/dashboard-home/dashboard-home').then(m => m.DashboardHome)
      },
      {
        path: 'teams',
        loadComponent: () => import('./features/dashboard/pages/teams/teams').then(m => m.TeamsPage)
      },
      {
        path: 'objectives',
        loadComponent: () => import('./features/dashboard/pages/objectives/objectives').then(m => m.ObjectivesPage)
      },
      {
        path: 'master-data',
        children: [
          {
            path: 'genders',
            loadComponent: () => import('./features/dashboard/pages/master-data/genders/genders.page').then(m => m.GendersPage)
          },
          {
            path: 'age-groups',
            loadComponent: () => import('./features/dashboard/pages/master-data/age-groups/age-groups.page').then(m => m.AgeGroupsPage)
          },
          {
            path: 'categories',
            loadComponent: () => import('./features/dashboard/pages/master-data/categories/categories.page').then(m => m.CategoriesPage)
          }
        ]
      },
      {
        path: 'design',
        children: [
          // {
          //   path: 'tables',
          //   loadComponent: () => import('./shared/design/sport-tables-design/sport-tables-design').then(m => m.SportTablesDesign)
          // },
          // {
          //   path: 'cards',
          //   loadComponent: () => import('./features/dashboard/pages/cards-showcase/cards-showcase').then(m => m.CardsShowcase)
          // },
          // {
          //   path: 'forms',
          //   loadComponent: () => import('./shared/design/sport-tables-design/sport-tables-design').then(m => m.SportTablesDesign)
          // }
        ]
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
