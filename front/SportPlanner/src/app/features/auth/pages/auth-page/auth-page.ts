import { Component, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { AuthStateService } from '../../../../core/auth/services/auth-state.service';
import { LoginCredentials, RegisterPayload } from '../../../../core/auth/models/user.model';

@Component({
  selector: 'app-auth-page',
  imports: [CommonModule, FormsModule],
  templateUrl: './auth-page.html',
  styleUrl: './auth-page.css'
})
export class AuthPage {
  private authService = inject(AuthService);
  private authStateService = inject(AuthStateService);
  private router = inject(Router);

  // UI State
  activeTab = signal<'login' | 'register'>('login');
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  // Login form
  loginEmail = signal('');
  loginPassword = signal('');
  rememberMe = signal(false);

  // Register form
  registerName = signal('');
  registerEmail = signal('');
  registerPassword = signal('');
  registerConfirmPassword = signal('');
  acceptTerms = signal(false);

  async login(): Promise<void> {
    // Clear messages
    this.errorMessage.set(null);
    this.successMessage.set(null);

    // Validate
    if (!this.loginEmail() || !this.loginPassword()) {
      this.errorMessage.set('Por favor completa todos los campos');
      return;
    }

    this.isLoading.set(true);

    try {
      const credentials: LoginCredentials = {
        email: this.loginEmail(),
        password: this.loginPassword()
      };

      // AuthService now handles updating auth state internally
      const user = await this.authService.signIn(credentials);

      console.log('Login successful:', user.email);
      this.successMessage.set('¡Bienvenido!');

      // Navigate to dashboard
      setTimeout(() => {
        this.router.navigate(['/dashboard']);
      }, 500);

    } catch (error) {
      console.error('Login error:', error);
      this.errorMessage.set(
        error instanceof Error ? error.message : 'Error al iniciar sesión'
      );
    } finally {
      this.isLoading.set(false);
    }
  }

  async register(): Promise<void> {
    // Clear messages
    this.errorMessage.set(null);
    this.successMessage.set(null);

    // Validate
    if (!this.registerName() || !this.registerEmail() || !this.registerPassword()) {
      this.errorMessage.set('Por favor completa todos los campos');
      return;
    }

    if (this.registerPassword() !== this.registerConfirmPassword()) {
      this.errorMessage.set('Las contraseñas no coinciden');
      return;
    }

    if (this.registerPassword().length < 6) {
      this.errorMessage.set('La contraseña debe tener al menos 6 caracteres');
      return;
    }

    if (!this.acceptTerms()) {
      this.errorMessage.set('Debes aceptar los términos y condiciones');
      return;
    }

    this.isLoading.set(true);

    try {
      const payload: RegisterPayload = {
        email: this.registerEmail(),
        password: this.registerPassword(),
        displayName: this.registerName()
      };

      // AuthService now handles updating auth state internally
      const user = await this.authService.signUp(payload);

      console.log('Registration successful:', user.email);
      this.successMessage.set('¡Cuenta creada exitosamente! Redirigiendo...');

      // Navigate to dashboard after delay
      setTimeout(() => {
        this.router.navigate(['/dashboard']);
      }, 1500);

    } catch (error) {
      console.error('Registration error:', error);
      this.errorMessage.set(
        error instanceof Error ? error.message : 'Error al crear la cuenta'
      );
    } finally {
      this.isLoading.set(false);
    }
  }
}
