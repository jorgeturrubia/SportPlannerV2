import { Component, signal } from '@angular/core';
import { CommonModule, NgIf } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'sp-sport-demo-forms',
  standalone: true,
  imports: [CommonModule, NgIf, ReactiveFormsModule],
  templateUrl: './sport-demo-forms.component.html',
})
export class SportDemoFormsComponent {
  view = signal<'team' | 'contact'>('team');

  teamForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(3)]),
    city: new FormControl('', Validators.required),
    founded: new FormControl('', Validators.required),
    members: new FormControl('', Validators.required),
  });

  contactForm = new FormGroup({
    name: new FormControl('', Validators.required),
    email: new FormControl('', [Validators.required, Validators.email]),
    message: new FormControl('', [Validators.required, Validators.minLength(10)]),
  });

  submittedTeam: any = null;
  submittedContact: any = null;

  submitTeam() {
    if (this.teamForm.valid) {
      this.submittedTeam = { ...this.teamForm.value };
      // fake processing
      this.teamForm.reset();
    } else {
      this.teamForm.markAllAsTouched();
    }
  }

  submitContact() {
    if (this.contactForm.valid) {
      this.submittedContact = { ...this.contactForm.value };
      this.contactForm.reset();
    } else {
      this.contactForm.markAllAsTouched();
    }
  }
}
