import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="auth-container">
      <div class="auth-card">
        <div class="brand-logo">🩸🩸</div>
        <h2 class="brand-title">Create Account</h2>
        <p class="brand-subtitle">Join the AI Blood Forecast System</p>

        <form [formGroup]="registerForm" (ngSubmit)="onSubmit()">
          <div class="form-group">
            <label>Username</label>
            <input type="text" formControlName="username" placeholder="johndoe" />
          </div>

          <div class="form-group">
            <label>Email address</label>
            <input type="email" formControlName="email" placeholder="you@example.com" />
          </div>

          <div class="form-group">
            <label>Password</label>
            <input type="password" formControlName="password" placeholder="Min. 6 characters" />
          </div>

          @if (errorMessage()) {
            <div class="error-msg">{{ errorMessage() }}</div>
          }

          <button type="submit" class="btn-submit" [disabled]="registerForm.invalid || isLoading()">
            {{ isLoading() ? 'Creating Account...' : 'Create Account' }}
          </button>
        </form>

        <div class="auth-footer">
          Already have an account? <a routerLink="/login">Sign In</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .brand-logo { font-size: 2rem; margin-bottom: 0.5rem; }
    .brand-title { margin: 0; font-size: 1.5rem; color: #111827; font-weight: 700; }
    .brand-subtitle { margin: 0.25rem 0 1.5rem 0; color: #6b7280; font-size: 0.875rem; }
    
    .form-group { text-align: left; margin-bottom: 1.25rem; }
    .form-group label { display: block; font-size: 0.85rem; font-weight: 600; color: #374151; margin-bottom: 0.5rem; }
    .form-group input { 
      width: 100%; padding: 0.65rem 0.75rem; border: 1px solid #d1d5db; border-radius: 6px; 
      box-sizing: border-box; font-size: 0.95rem; 
    }
    
    .btn-submit { 
      width: 100%; background-color: #e57373; color: white; border: none; padding: 0.75rem; 
      border-radius: 6px; font-weight: 600; cursor: pointer; font-size: 0.95rem; margin-top: 1rem;
    }
    .btn-submit:disabled { opacity: 0.7; cursor: not-allowed; }

    .error-msg { color: #dc2626; font-size: 0.85rem; text-align: left; margin-bottom: 0.75rem; }
    .auth-footer { margin-top: 1.5rem; font-size: 0.85rem; color: #4b5563; border-top: 1px solid #e5e7eb; padding-top: 1rem;}
    .auth-footer a { color: #dc2626; text-decoration: none; font-weight: 600; }
  `]
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  registerForm = this.fb.group({
    username: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  onSubmit() {
    if (this.registerForm.invalid) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    // Map properties matching backend DTO constraints
    const formPayload = {
      ...this.registerForm.value,
      phoneNumber: "" // Auto-handles blank/missing parameters gracefully per service layer configuration
    };

    this.authService.register(formPayload as any).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error || 'Registration failed. Check details.');
      }
    });
  }
}