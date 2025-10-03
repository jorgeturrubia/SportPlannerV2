import { Component, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SubscriptionService, SubscriptionType, Sport } from '../../../../core/subscription/services/subscription.service';

interface SubscriptionPlan {
  type: SubscriptionType;
  name: string;
  maxUsers: number;
  maxTeams: number;
  features: string[];
  recommended?: boolean;
}

@Component({
  selector: 'app-subscription-selection-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './subscription-selection-page.html',
  styleUrl: './subscription-selection-page.css'
})
export class SubscriptionSelectionPage {
  private subscriptionService = inject(SubscriptionService);
  private router = inject(Router);

  selectedSport = signal<Sport>('Football');
  isCreating = signal(false);
  error = signal<string | null>(null);

  // Available sports
  sports: Sport[] = ['Football', 'Basketball', 'Handball'];

  // Subscription plans
  plans: SubscriptionPlan[] = [
    {
      type: 'Free',
      name: 'Plan Gratuito',
      maxUsers: 1,
      maxTeams: 1,
      features: [
        '1 equipo',
        '1 usuario',
        'Funcionalidades básicas',
        'Ideal para empezar'
      ]
    },
    {
      type: 'Club',
      name: 'Plan Club',
      maxUsers: 1,
      maxTeams: 1,
      features: [
        '1 equipo',
        '1 usuario',
        'Gestión de club deportivo',
        'Reportes básicos'
      ]
    },
    {
      type: 'Team',
      name: 'Plan Equipo',
      maxUsers: 15,
      maxTeams: 15,
      recommended: true,
      features: [
        'Hasta 15 equipos',
        'Hasta 15 usuarios',
        'Gestión completa de equipos',
        'Calendario compartido',
        'Reportes avanzados'
      ]
    },
    {
      type: 'Coach',
      name: 'Plan Entrenador',
      maxUsers: 15,
      maxTeams: 15,
      features: [
        'Hasta 15 equipos',
        'Hasta 15 usuarios',
        'Herramientas de entrenador',
        'Planificación de entrenamientos',
        'Seguimiento de atletas'
      ]
    }
  ];

  getSportLabel(sport: Sport): string {
    const labels: Record<Sport, string> = {
      'Football': 'Fútbol',
      'Basketball': 'Baloncesto',
      'Handball': 'Balonmano'
    };
    return labels[sport];
  }

  async selectPlan(plan: SubscriptionPlan): Promise<void> {
    this.error.set(null);
    this.isCreating.set(true);

    try {
      const subscriptionId = await this.subscriptionService.createSubscription({
        type: plan.type,
        sport: this.selectedSport()
      });

      console.log('Subscription created:', subscriptionId);

      // Navigate to dashboard
      await this.router.navigate(['/dashboard']);

    } catch (err) {
      console.error('Failed to create subscription:', err);
      this.error.set(
        err instanceof Error ? err.message : 'No se pudo crear la suscripción'
      );
    } finally {
      this.isCreating.set(false);
    }
  }
}
