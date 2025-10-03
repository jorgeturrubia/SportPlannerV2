/**
 * Design Components Library
 * Componentes reutilizables e innovadores para SportPlanner
 */

export { SportDemoTablesComponent } from './sport-demo-tables.component';

// Tipos y utilidades
export type ViewMode = 'grid' | 'board' | 'carousel' | 'timeline' | 'hexagon' | 'datatable' | 'galaxy';

export interface DemoTableItem {
  id: string | number;
  name: string;
  color: string;
  description?: string;
  members: Array<{
    id: string | number;
    name: string;
  }>;
  createdAt: Date | string;
  [key: string]: any; // Permite campos adicionales
}
