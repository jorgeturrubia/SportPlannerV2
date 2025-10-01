import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-auth-page',
  imports: [],
  templateUrl: './auth-page.html'
})
export class AuthPage {
  activeTab = signal<'login' | 'register'>('login');

  constructor(private router: Router) {}

  login() {
    // TODO: Implement authentication logic
    this.router.navigate(['/dashboard']);
  }

  register() {
    // TODO: Implement registration logic
    this.router.navigate(['/dashboard']);
  }
}
