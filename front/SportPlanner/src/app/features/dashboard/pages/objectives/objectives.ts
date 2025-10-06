import { Component, signal, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { EntityPageLayoutComponent } from '../../../../shared/components/entity-page-layout/entity-page-layout.component';
import { CardComponent } from '../../../../shared/components/card/card.component';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { CardCarouselComponent } from '../../../../shared/components/card-carousel/card-carousel.component';
import { DynamicFormComponent, FormField } from '../../../../shared/components/dynamic-form/dynamic-form.component';
import { ObjectivesService, ObjectiveDto, CreateObjectiveDto, UpdateObjectiveDto, Sport, ContentOwnership } from '../../services/objectives.service';
import { ObjectiveCategoriesService } from '../../services/objective-categories.service';
import { ObjectiveSubcategoriesService } from '../../services/objective-subcategories.service';
import { SubscriptionContextService } from '../../../../core/subscription/services/subscription-context.service';
import { NotificationService } from '../../../../shared/notifications/notification.service';

@Component({
  selector: 'app-objectives',
  standalone: true,
  imports: [CommonModule, TranslateModule, EntityPageLayoutComponent, CardComponent, ConfirmationDialogComponent, DynamicFormComponent, CardCarouselComponent],
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
