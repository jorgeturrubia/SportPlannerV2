import { Component, signal, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { DynamicFormComponent, FormField } from '../../../../shared/components/dynamic-form/dynamic-form.component';
import { DataTableComponent, TableColumn, TableAction } from '../../../../shared/components/data-table/data-table.component';
import { ObjectivesService, ObjectiveDto, CreateObjectiveDto, UpdateObjectiveDto, Sport, ContentOwnership } from '../../services/objectives.service';
import { ObjectiveCategoriesService } from '../../services/objective-categories.service';
import { ObjectiveSubcategoriesService } from '../../services/objective-subcategories.service';
import { SubscriptionContextService } from '../../../../core/subscription/services/subscription-context.service';
import { NotificationService } from '../../../../shared/notifications/notification.service';

@Component({
  selector: 'app-objectives',
  standalone: true,
  imports: [CommonModule, TranslateModule, ConfirmationDialogComponent, DynamicFormComponent, DataTableComponent],
  templateUrl: './objectives.html',
  styleUrls: ['./objectives.css']
})
export class ObjectivesPage implements OnInit {
  private objectivesService = inject(ObjectivesService);
  private categoriesService = inject(ObjectiveCategoriesService);
  private subcategoriesService = inject(ObjectiveSubcategoriesService);
  private subscriptionContext = inject(SubscriptionContextService);
  private ns = inject(NotificationService);

  objectives = signal<ObjectiveDto[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  // Master data
  categories = signal<any[]>([]);
  subcategories = signal<any[]>([]);

  // Dialog and Form state
  isConfirmDialogOpen = signal(false);
  isFormOpen = signal(false);
  selectedObjective = signal<ObjectiveDto | null>(null);
  formTitle = 'Add New Objective';

  // Table configuration
  tableColumns = computed<TableColumn[]>(() => [
  { key: 'name', label: 'Nombre', sortable: true },
  { key: 'description', label: 'Descripción', sortable: false, truncate: true, truncateLength: 120 },
    {
      key: 'objectiveCategoryId',
      label: 'Categoría',
      sortable: true,
      formatter: (value: string) => this.getCategoryName(value)
    },
    {
      key: 'objectiveSubcategoryId',
      label: 'Subcategoría',
      sortable: false,
      formatter: (value?: string) => this.getSubcategoryName(value)
    },
    // 'ownership' column removed as requested
    { key: 'isActive', label: 'Estado', sortable: true, type: 'badge' }
  ]);

  tableActions = computed<TableAction[]>(() => [
    {
      icon: 'M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z',
      label: 'Editar',
      color: 'blue',
      handler: (row: ObjectiveDto) => {
        if (row.isEditable) this.openEditForm(row);
      }
    },
    {
      icon: 'M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z',
      label: 'Clonar',
      color: 'green',
      handler: (row: ObjectiveDto) => {
        if (!row.isEditable) this.handleClone(row);
      }
    },
    {
      icon: 'M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16',
      label: 'Eliminar',
      color: 'red',
      handler: (row: ObjectiveDto) => {
        if (row.isEditable) this.openDeleteConfirm(row);
      }
    }
  ]);

  objectiveFormConfig = computed<FormField[]>(() => [
    { key: 'name', label: 'Name', type: 'text', required: true },
    { key: 'description', label: 'Description', type: 'textarea', required: true },
    {
      key: 'objectiveCategoryId',
      label: 'Category',
      type: 'select',
      required: true,
      options: this.categories().map(c => ({ value: c.id, label: c.name }))
    },
    {
      key: 'objectiveSubcategoryId',
      label: 'Subcategory',
      type: 'select',
      required: false,
      options: this.subcategories().map(s => ({ value: s.id, label: s.name }))
    }
    ,
    {
      key: 'level',
      label: 'Level',
      type: 'number',
      required: false,
      min: 1,
      max: 5
    }
  ]);

  async ngOnInit(): Promise<void> {
    await this.subscriptionContext.loadSubscription();
    await Promise.all([
      this.loadObjectives(),
      this.loadMasterData()
    ]);
  }

  private async loadMasterData(): Promise<void> {
    try {
      const subscription = this.subscriptionContext.subscription();
      if (!subscription) return;

      const sport = this.parseSportToEnum(subscription.sport);
      const [categories, subcategories] = await Promise.all([
        this.categoriesService.getCategories(sport),
        this.subcategoriesService.getSubcategories()
      ]);

      this.categories.set(categories);
      this.subcategories.set(subcategories);
    } catch (err: any) {
      console.error('Failed to load master data:', err);
      this.ns.error(err?.message ?? 'Error al cargar datos maestros', 'Error');
    }
  }

  private parseSportToEnum(sportStr: string): Sport {
    switch (sportStr) {
      case 'Football': return Sport.Football;
      case 'Basketball': return Sport.Basketball;
      case 'Handball': return Sport.Handball;
      default: return Sport.Football;
    }
  }

  private async loadObjectives(): Promise<void> {
    try {
      this.isLoading.set(true);
      this.error.set(null);

      const objectives = await this.objectivesService.getObjectives();
      this.objectives.set(objectives || []);
    } catch (err: any) {
      console.error('Failed to load objectives:', err);
      this.error.set(err.message || 'Failed to load objectives');
      this.ns.error(err?.message ?? 'Error al cargar objetivos', 'Error');
      this.objectives.set([]);
    } finally {
      this.isLoading.set(false);
    }
  }

  getCategoryName(categoryId: string): string {
    const category = this.categories().find(c => c.id === categoryId);
    return category?.name || 'Unknown';
  }

  getSubcategoryName(subcategoryId?: string): string {
    if (!subcategoryId) return '-';
    const subcategory = this.subcategories().find(s => s.id === subcategoryId);
    return subcategory?.name || 'Unknown';
  }

  getOwnershipLabel(ownership: ContentOwnership): string {
    switch (ownership) {
      case ContentOwnership.System: return 'Sistema';
      case ContentOwnership.User: return 'Usuario';
      case ContentOwnership.MarketplaceUser: return 'Marketplace';
      default: return 'Unknown';
    }
  }

  // Delete Logic
  openDeleteConfirm(objective: ObjectiveDto): void {
    this.selectedObjective.set(objective);
    this.isConfirmDialogOpen.set(true);
  }

  async handleDeleteConfirm(): Promise<void> {
    const objectiveToDelete = this.selectedObjective();
    if (!objectiveToDelete) return;

    try {
      await this.objectivesService.deleteObjective(objectiveToDelete.id);
      this.ns.success('Objetivo eliminado', 'Objetivos');
      this.objectives.update(objs => objs.filter(o => o.id !== objectiveToDelete.id));
    } catch (err: any) {
      console.error('Failed to delete objective:', err);
      this.error.set(err.message || 'Failed to delete objective');
      this.ns.error(err?.message ?? 'No se pudo eliminar el objetivo', 'Error');
    } finally {
      this.closeConfirmDialog();
    }
  }

  closeConfirmDialog(): void {
    this.isConfirmDialogOpen.set(false);
    this.selectedObjective.set(null);
  }

  // Add/Edit Logic
  openAddForm(): void {
    this.selectedObjective.set(null);
    this.formTitle = 'Add New Objective';
    this.isFormOpen.set(true);
  }

  openEditForm(objective: ObjectiveDto): void {
    this.selectedObjective.set(objective);
    this.formTitle = `Edit ${objective.name}`;
    this.isFormOpen.set(true);
  }

  async handleFormSubmit(formData: any): Promise<void> {
    const selected = this.selectedObjective();

    try {
      const subscription = this.subscriptionContext.subscription();
      if (!subscription) return;

      const sport = this.parseSportToEnum(subscription.sport);

      if (selected) {
        // Update existing objective
        const updatePayload: UpdateObjectiveDto = {
          id: selected.id,
          name: formData.name,
          description: formData.description,
          objectiveCategoryId: formData.objectiveCategoryId,
            objectiveSubcategoryId: formData.objectiveSubcategoryId || undefined,
            level: formData.level ?? undefined,
          techniques: []
        };

        await this.objectivesService.updateObjective(selected.id, updatePayload);
        await this.loadObjectives();
        this.ns.success('Objetivo actualizado', 'Objetivos');
      } else {
        // Add new objective
        const createPayload: CreateObjectiveDto = {
          sport: sport,
          name: formData.name,
          description: formData.description,
          objectiveCategoryId: formData.objectiveCategoryId,
          objectiveSubcategoryId: formData.objectiveSubcategoryId || undefined,
            level: formData.level ?? 1,
            techniques: []
        };

        await this.objectivesService.createObjective(createPayload);
        await this.loadObjectives();
        this.ns.success('Objetivo creado', 'Objetivos');
      }

      this.closeForm();
    } catch (err: any) {
      console.error('Failed to save objective:', err);
      this.error.set(err.message || 'Failed to save objective');
      this.ns.error(err?.message ?? 'No se pudo guardar el objetivo', 'Error');
    }
  }

  async handleClone(objective: ObjectiveDto): Promise<void> {
    try {
      await this.objectivesService.cloneObjective(objective.id);
      await this.loadObjectives();
      this.ns.success('Objetivo clonado', 'Objetivos');
    } catch (err: any) {
      console.error('Failed to clone objective:', err);
      this.ns.error(err?.message ?? 'No se pudo clonar el objetivo', 'Error');
    }
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.selectedObjective.set(null);
  }
}
