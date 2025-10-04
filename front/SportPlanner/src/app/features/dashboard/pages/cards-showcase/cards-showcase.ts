import { Component, signal, Input, computed, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { CardConfig } from '../../../../shared/design/models/card-config';
import { CardsMockService } from '../../services/cards-mock.service';
import { DynamicForm } from '../../../../shared/design/components/dynamic-form/dynamic-form';
import { ConfirmationDialog, ConfirmationConfig } from '../../../../shared/design/components/confirmation-dialog/confirmation-dialog';
import { FormConfig } from '../../../../shared/design/models/form-config';

interface AthleteCard {
  id: number;
  name: string;
  sport: string;
  level: string;
  age: number;
  country: string;
  status: 'Active' | 'Inactive' | 'On Leave' | 'Injured' | 'Recovery';
  totalSessions: number;
  lastSession: string;
  avatar: string;
  performance: number;
  nextSession?: string;
}

@Component({
  selector: 'app-cards-showcase',
  standalone: true,
  imports: [CommonModule, TranslateModule, FormsModule, DynamicForm, ConfirmationDialog],
  templateUrl: './cards-showcase.html',
  styleUrls: ['./cards-showcase.css']
})
export class CardsShowcase {
  private mock = inject(CardsMockService) as CardsMockService<AthleteCard>;
  constructor(private sanitizer: DomSanitizer) {
    // initialize mock with demo data
    this.mock.init(this.athletes());
  }

  // Demo data - replace with real data or provide via @Input() items
  athletes = signal<AthleteCard[]>([
    {
      id: 1,
      name: 'Emma Johnson',
      sport: 'Basketball',
      level: 'Professional',
      age: 24,
      country: 'USA',
      status: 'Active',
      totalSessions: 156,
      lastSession: '2025-10-03',
      avatar: 'EJ',
      performance: 92,
      nextSession: '2025-10-05'
    },
    {
      id: 2,
      name: 'Lucas Silva',
      sport: 'Football',
      level: 'Elite',
      age: 22,
      country: 'Brazil',
      status: 'Active',
      totalSessions: 203,
      lastSession: '2025-10-02',
      avatar: 'LS',
      performance: 95,
      nextSession: '2025-10-04'
    },
    {
      id: 3,
      name: 'Sophie Martin',
      sport: 'Tennis',
      level: 'Advanced',
      age: 26,
      country: 'France',
      status: 'Active',
      totalSessions: 189,
      lastSession: '2025-10-03',
      avatar: 'SM',
      performance: 88,
      nextSession: '2025-10-06'
    },
    {
      id: 4,
      name: 'David Kim',
      sport: 'Swimming',
      level: 'Professional',
      age: 21,
      country: 'South Korea',
      status: 'Recovery',
      totalSessions: 167,
      lastSession: '2025-09-28',
      avatar: 'DK',
      performance: 85,
      nextSession: '2025-10-07'
    },
    {
      id: 5,
      name: 'Anna Müller',
      sport: 'Athletics',
      level: 'Elite',
      age: 25,
      country: 'Germany',
      status: 'Active',
      totalSessions: 221,
      lastSession: '2025-10-03',
      avatar: 'AM',
      performance: 96,
      nextSession: '2025-10-05'
    },
    {
      id: 6,
      name: 'James Brown',
      sport: 'Cycling',
      level: 'Advanced',
      age: 28,
      country: 'UK',
      status: 'On Leave',
      totalSessions: 145,
      lastSession: '2025-09-25',
      avatar: 'JB',
      performance: 78,
      nextSession: '2025-10-15'
    },
    {
      id: 7,
      name: 'Maria Garcia',
      sport: 'Volleyball',
      level: 'Professional',
      age: 23,
      country: 'Spain',
      status: 'Active',
      totalSessions: 178,
      lastSession: '2025-10-03',
      avatar: 'MG',
      performance: 90,
      nextSession: '2025-10-04'
    },
    {
      id: 8,
      name: 'Chen Wei',
      sport: 'Boxing',
      level: 'Intermediate',
      age: 27,
      country: 'China',
      status: 'Injured',
      totalSessions: 134,
      lastSession: '2025-09-20',
      avatar: 'CW',
      performance: 72,
      nextSession: '2025-10-20'
    },
    {
      id: 9,
      name: 'Olivia Taylor',
      sport: 'Tennis',
      level: 'Advanced',
      age: 24,
      country: 'Australia',
      status: 'Active',
      totalSessions: 192,
      lastSession: '2025-10-03',
      avatar: 'OT',
      performance: 89,
      nextSession: '2025-10-05'
    },
    {
      id: 10,
      name: 'Diego Morales',
      sport: 'Football',
      level: 'Professional',
      age: 26,
      country: 'Argentina',
      status: 'Active',
      totalSessions: 215,
      lastSession: '2025-10-03',
      avatar: 'DM',
      performance: 93,
      nextSession: '2025-10-04'
    },
    {
      id: 11,
      name: 'Isabella Rossi',
      sport: 'Swimming',
      level: 'Elite',
      age: 22,
      country: 'Italy',
      status: 'Active',
      totalSessions: 198,
      lastSession: '2025-10-03',
      avatar: 'IR',
      performance: 94,
      nextSession: '2025-10-05'
    },
    {
      id: 12,
      name: 'Alex Petrov',
      sport: 'Athletics',
      level: 'Advanced',
      age: 25,
      country: 'Russia',
      status: 'Active',
      totalSessions: 173,
      lastSession: '2025-10-02',
      avatar: 'AP',
      performance: 87,
      nextSession: '2025-10-06'
    }
  ]);

  searchTerm = signal('');

  @Input()
  set items(v: AthleteCard[] | undefined) {
    if (v && v.length) this.athletes.set(v);
  }

  filteredAthletes = computed(() => {
    const q = this.searchTerm().trim().toLowerCase();
    if (!q) return this.athletes();
    const searchableFields = this.cardConfig().searchableFields || [];
    return this.athletes().filter(item => searchableFields.some(field => {
      const value = this.getFieldValue(item, field);
      return String(value).toLowerCase().includes(q);
    }));
  });

  // Events: keep legacy simple events and add a richer cardEvent
  @Output() edit = new EventEmitter<AthleteCard>();
  @Output() remove = new EventEmitter<AthleteCard>();
  @Output() cardEvent = new EventEmitter<{
    phase: 'before' | 'after';
    action: 'update' | 'remove';
    index: number | null;
    before?: AthleteCard;
    after?: AthleteCard;
    time: string;
  }>();

  // Editing state
  editing = signal<AthleteCard | null>(null);
  editCopy = signal<Partial<AthleteCard>>({});
  // Confirm modal state
  confirmModal = signal<{ open: boolean; target?: AthleteCard | null; message?: string }>({ open: false, target: null, message: '' });
  // Undo toast state
  undoToast = signal<{ open: boolean; message?: string }>({ open: false, message: '' });

  // Dynamic form config for editing
  formConfig = signal<FormConfig>({
    fields: [
      { key: 'name', label: 'Name', type: 'text', required: true },
      { key: 'country', label: 'Country', type: 'text' },
      { key: 'performance', label: 'Performance', type: 'number', min: 0, max: 100 },
      { key: 'status', label: 'Status', type: 'select', options: [
        { value: 'Active', label: 'Active' },
        { value: 'Inactive', label: 'Inactive' },
        { value: 'On Leave', label: 'On Leave' },
        { value: 'Injured', label: 'Injured' },
        { value: 'Recovery', label: 'Recovery' }
      ] }
    ]
  });

  // Card configuration
  cardConfig = signal<CardConfig<AthleteCard>>({
    avatarField: 'avatar',
    headerGradientField: 'sport',
    headerBadgeField: 'sport',
    titleField: 'name',
    gradientMap: {
      'Football': 'from-purple-500 to-indigo-600',
      'Basketball': 'from-orange-500 to-red-600',
      'Tennis': 'from-green-500 to-teal-600',
      'Swimming': 'from-blue-500 to-cyan-600',
      'Athletics': 'from-yellow-500 to-orange-600',
      'Cycling': 'from-lime-500 to-green-600',
      'Volleyball': 'from-pink-500 to-purple-600',
      'Boxing': 'from-red-500 to-pink-600'
    },
    subtitleFields: [
      {
        key: 'level',
        label: 'Level',
        type: 'badge',
        badgeConfig: {
          colorMap: {
            'Beginner': 'bg-green-100 text-green-700 dark:bg-green-900/50 dark:text-green-300',
            'Intermediate': 'bg-blue-100 text-blue-700 dark:bg-blue-900/50 dark:text-blue-300',
            'Advanced': 'bg-orange-100 text-orange-700 dark:bg-orange-900/50 dark:text-orange-300',
            'Professional': 'bg-purple-100 text-purple-700 dark:bg-purple-900/50 dark:text-purple-300',
            'Elite': 'bg-pink-100 text-pink-700 dark:bg-pink-900/50 dark:text-pink-300'
          }
        }
      },
      {
        key: 'status',
        label: 'Status',
        type: 'badge',
        badgeConfig: {
          colorMap: {
            'Active': 'bg-green-100 text-green-700 dark:bg-green-900/50 dark:text-green-300',
            'Inactive': 'bg-gray-100 text-gray-700 dark:bg-gray-700/50 dark:text-gray-300',
            'On Leave': 'bg-orange-100 text-orange-700 dark:bg-orange-900/50 dark:text-orange-300',
            'Injured': 'bg-red-100 text-red-700 dark:bg-red-900/50 dark:text-red-300',
            'Recovery': 'bg-cyan-100 text-cyan-700 dark:bg-cyan-900/50 dark:text-cyan-300'
          }
        }
      }
    ],
    infoFields: [
      {
        key: 'country',
        label: 'cards.country',
        type: 'icon-value',
        icon: 'M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z M15 11a3 3 0 11-6 0 3 3 0 016 0z'
      },
      {
        key: 'age',
        label: 'cards.age',
        type: 'number',
        suffix: 'cards.yearsOld',
        icon: 'M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z M15 11a3 3 0 11-6 0 3 3 0 016 0z'
      },
      {
        key: 'totalSessions',
        label: 'cards.sessions',
        type: 'number',
        icon: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z',
        colorConfig: {
          valueMap: {} // Will use default indigo color
        }
      },
      {
        key: 'performance',
        label: 'cards.performance',
        type: 'number',
        suffix: '%',
        icon: 'M13 7h8m0 0v8m0-8l-8 8-4-4-6 6',
        colorConfig: {
          rangeMap: [
            { min: 90, max: 100, color: 'text-green-600 dark:text-green-400' },
            { min: 80, max: 89, color: 'text-blue-600 dark:text-blue-400' },
            { min: 70, max: 79, color: 'text-orange-600 dark:text-orange-400' },
            { min: 0, max: 69, color: 'text-red-600 dark:text-red-400' }
          ]
        }
      },
      {
        key: 'nextSession',
        label: 'cards.nextSession',
        type: 'text',
        icon: 'M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z',
        visible: true
      }
    ],
    actions: [
      {
        icon: 'M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z',
        label: 'common.edit',
        color: 'bg-indigo-600 hover:bg-indigo-700 text-white',
        onClick: (athlete) => this.onEdit(athlete),
        primary: true
      },
      {
        icon: 'M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16',
        label: 'common.delete',
        color: 'bg-red-600 hover:bg-red-700 text-white',
        onClick: (athlete) => this.onDelete(athlete),
        primary: false
      }
    ],
    searchableFields: ['name', 'sport', 'country']
  });

  // Generic methods
  getFieldValue(item: any, field: string | keyof AthleteCard): any {
    return (item as any)[field];
  }

  getGradient(item: any): string {
    const config = this.cardConfig();
    if (!config.headerGradientField || !config.gradientMap) return 'from-gray-500 to-gray-600';
    const value = this.getFieldValue(item, config.headerGradientField);
    return config.gradientMap[value] || 'from-gray-500 to-gray-600';
  }

  getBadgeColor(value: any, field: any): string {
    if (!field.badgeConfig) return 'bg-gray-100 text-gray-700';
    return field.badgeConfig.colorMap[value] || 'bg-gray-100 text-gray-700 dark:bg-gray-700/50 dark:text-gray-300';
  }

  getColorForValue(value: any, field: any): string {
    if (!field.colorConfig) return 'text-indigo-600 dark:text-indigo-400';

    // Range-based color
    if (field.colorConfig.rangeMap && typeof value === 'number') {
      const range = field.colorConfig.rangeMap.find(
        (r: any) => value >= r.min && value <= r.max
      );
      return range?.color || 'text-gray-600 dark:text-gray-400';
    }

    // Value-based color
    if (field.colorConfig.valueMap) {
      return field.colorConfig.valueMap[value] || 'text-gray-600 dark:text-gray-400';
    }

    return 'text-indigo-600 dark:text-indigo-400';
  }

  getSafeHtml(html: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(html);
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    // Update search term; filteredAthletes is a computed signal and will react.
    this.searchTerm.set(input.value.toLowerCase());
  }

  onEdit(athlete: AthleteCard): void {
    // Open inline editor
    this.startEdit(athlete);
    this.edit.emit(athlete);
  }

  onDelete(athlete: AthleteCard): void {
    // Open custom Tailwind confirm modal instead of browser confirm()
    this.confirmModal.set({ open: true, target: athlete, message: `Eliminar ${athlete.name}?` });
    // emit before event
    const idx = this.athletes().findIndex(a => a.id === athlete.id);
    this.cardEvent.emit({ phase: 'before', action: 'remove', index: idx >= 0 ? idx : null, before: { ...athlete }, time: new Date().toISOString() });
  }

  confirmRemove() {
    const target = this.confirmModal().target;
    if (!target) return this.closeConfirm();
    const res = this.mock.remove(target.id);
    if (res.success) {
      // update local signal from mock
      this.athletes.set(this.mock.getAll());
      this.remove.emit(target);
      this.cardEvent.emit({ phase: 'after', action: 'remove', index: null, before: res.removed ? { ...res.removed } : undefined, time: new Date().toISOString() });
      // show undo toast (persistent until user closes or clicks Undo)
      this.undoToast.set({ open: true, message: `${target.name} eliminado` });
    }
    this.closeConfirm();
  }

  undoLast() {
    const res = this.mock.undoLast();
    if (res.success) {
      this.athletes.set(this.mock.getAll());
      this.undoToast.set({ open: true, message: 'Acción deshecha' });
      // If mock returned the action that was undone, emit a cardEvent so parent can sync
      const action = (res as any).action;
      if (action) {
        if (action.type === 'remove' && action.before) {
          // restore -> treat as update/after with the restored item as 'after'
          this.cardEvent.emit({ phase: 'after', action: 'update', index: null, after: { ...action.before }, time: new Date().toISOString() });
        } else if (action.type === 'update' && action.before) {
          // revert update -> emit after with before payload
          this.cardEvent.emit({ phase: 'after', action: 'update', index: null, after: { ...action.before }, time: new Date().toISOString() });
        }
      }
    }
  }

  closeConfirm() {
    this.confirmModal.set({ open: false, target: null, message: '' });
  }

  startEdit(athlete: AthleteCard) {
    this.editing.set(athlete);
    this.editCopy.set({ ...athlete });
  }

  cancelEdit() {
    this.editing.set(null);
    this.editCopy.set({});
  }

  saveEdit() {
    const copy = this.editCopy();
    const current = this.editing();
    if (!current) return;
    const updated = { ...current, ...copy } as AthleteCard;
    // emit before
    const idx = this.athletes().findIndex(a => a.id === updated.id);
    this.cardEvent.emit({ phase: 'before', action: 'update', index: idx >= 0 ? idx : null, before: { ...current }, time: new Date().toISOString() });

    const result = this.mock.update(updated);
    if (result.success) {
      this.athletes.set(this.mock.getAll());
      this.editing.set(null);
      this.editCopy.set({});
      this.edit.emit(updated);
      this.cardEvent.emit({ phase: 'after', action: 'update', index: null, before: result.before, after: result.after, time: new Date().toISOString() });
      // show undo toast (persistent)
      this.undoToast.set({ open: true, message: `Cambios guardados` });
    }
  }

  // Provide config object for the ConfirmationDialog component
  confirmConfig(): ConfirmationConfig {
    const msg = this.confirmModal().message || 'Are you sure?';
    return {
      title: 'Confirm',
      message: msg,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      confirmColor: 'danger'
    };
  }

  // Handler from DynamicForm submit
  onFormSubmit(payload: any) {
    // copy the submitted values to editCopy and save
    this.editCopy.set({ ...this.editCopy(), ...payload });
    this.saveEdit();
  }

  // General action caller used by template buttons to avoid inline complex expressions
  callAction(action: { onClick?: (item: AthleteCard) => void; label?: string }, item: AthleteCard) {
    // Prevent accidental event bubbling in template by handling actions centrally
    if (!action) return;
    if (action.label === 'common.edit') {
      this.startEdit(item);
      return;
    }
    if (action.label === 'common.delete') {
      this.onDelete(item);
      return;
    }
    // safe-call any custom handler
    try {
      action.onClick && action.onClick(item);
    } catch (e) {
      // swallow to avoid breaking UI; log for debugging
      // console.error('card action error', e);
    }
  }

  trackById(_: number, item: AthleteCard) {
    return item.id;
  }
}
