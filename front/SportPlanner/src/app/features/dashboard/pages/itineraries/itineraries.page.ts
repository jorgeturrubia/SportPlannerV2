import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { DataTableComponent, TableColumn, TableAction } from '../../../../shared/components/data-table/data-table.component';
import { DynamicFormComponent, FormField } from '../../../../shared/components/dynamic-form/dynamic-form.component';
import { ItinerariesService, ItineraryDto } from '../../services/itineraries.service';
import { NotificationService } from '../../../../shared/notifications/notification.service';

@Component({
  selector: 'app-itineraries-page',
  standalone: true,
  imports: [CommonModule, TranslateModule, DataTableComponent, DynamicFormComponent],
  templateUrl: './itineraries.page.html'
})
export class ItinerariesPage implements OnInit {
  private itinerariesService = inject(ItinerariesService);
  private ns = inject(NotificationService);

  itineraries = signal<ItineraryDto[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  // Form state
  isFormOpen = signal(false);
  selectedItinerary = signal<ItineraryDto | null>(null);
  formTitle = 'Añadir Itinerario';

  columns: TableColumn[] = [
    { key: 'name', label: 'Nombre', sortable: true },
    { key: 'description', label: 'Descripción', sortable: false },
    {
      key: 'sport',
      label: 'Deporte',
      sortable: true,
      formatter: (value: number) => this.getSportLabel(value)
    },
    {
      key: 'level',
      label: 'Nivel',
      sortable: true,
      formatter: (value: number) => this.getLevelLabel(value)
    },
    {
      key: 'items',
      label: 'Objetivos',
      sortable: false,
      formatter: (value: any[]) => (value?.length || 0).toString()
    },
    { key: 'isActive', label: 'Estado', sortable: true, type: 'badge' }
  ];

  actions: TableAction[] = [
    {
      icon: 'M15 12a3 3 0 11-6 0 3 3 0 016 0z M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z',
      label: 'Ver Detalles',
      color: 'blue',
      handler: (row) => this.viewItinerary(row)
    }
  ];

  formConfig: FormField[] = [
    { key: 'name', label: 'Nombre del Itinerario', type: 'text', required: true },
    { key: 'description', label: 'Descripción', type: 'textarea', required: true },
    {
      key: 'sport',
      label: 'Deporte',
      type: 'select',
      required: true,
      options: [
        { value: 0, label: 'Fútbol' },
        { value: 1, label: 'Baloncesto' },
        { value: 2, label: 'Tenis' },
        { value: 3, label: 'Natación' },
        { value: 4, label: 'Atletismo' }
      ]
    },
    {
      key: 'level',
      label: 'Nivel',
      type: 'select',
      required: true,
      options: [
        { value: 0, label: 'Principiante' },
        { value: 1, label: 'Intermedio' },
        { value: 2, label: 'Avanzado' },
        { value: 3, label: 'Experto' }
      ]
    }
  ];

  async ngOnInit(): Promise<void> {
    await this.loadItineraries();
  }

  private async loadItineraries(): Promise<void> {
    try {
      this.isLoading.set(true);
      this.error.set(null);

      const data = await this.itinerariesService.getItineraries();
      this.itineraries.set(data || []);
    } catch (err: any) {
      console.error('Failed to load itineraries:', err);
      this.error.set(err.message || 'Failed to load itineraries');
      this.ns.error(err?.message ?? 'Error al cargar itinerarios', 'Error');
      this.itineraries.set([]);
    } finally {
      this.isLoading.set(false);
    }
  }

  openAddForm(): void {
    this.selectedItinerary.set(null);
    this.formTitle = 'Añadir Itinerario';
    this.isFormOpen.set(true);
  }

  viewItinerary(itinerary: ItineraryDto): void {
    // TODO: Navigate to itinerary details or show modal with objectives
    this.ns.info(
      `Itinerario: ${itinerary.name} - ${itinerary.items.length} objetivos`,
      'Detalles del Itinerario'
    );
  }

  async handleFormSubmit(formData: any): Promise<void> {
    try {
      const createDto = {
        name: formData.name,
        description: formData.description,
        sport: Number(formData.sport),
        level: Number(formData.level),
        items: [] // Initially empty, items can be added in detail view
      };

      await this.itinerariesService.createItinerary(createDto);
      this.ns.success('Itinerario creado correctamente', 'Itinerarios');

      await this.loadItineraries();
      this.closeForm();
    } catch (err: any) {
      console.error('Failed to create itinerary:', err);
      this.error.set(err.message || 'Failed to create itinerary');
      this.ns.error(err?.message ?? 'No se pudo crear el itinerario', 'Error');
    }
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedItinerary.set(null);
  }

  private getSportLabel(sport: number): string {
    const sports: { [key: number]: string } = {
      0: 'Fútbol',
      1: 'Baloncesto',
      2: 'Tenis',
      3: 'Natación',
      4: 'Atletismo'
    };
    return sports[sport] || 'Desconocido';
  }

  private getLevelLabel(level: number): string {
    const levels: { [key: number]: string } = {
      0: 'Principiante',
      1: 'Intermedio',
      2: 'Avanzado',
      3: 'Experto'
    };
    return levels[level] || 'Desconocido';
  }
}
