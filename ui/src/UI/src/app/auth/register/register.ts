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
        <div class="brand-logo-container">
          <img src="lf.png" alt="LifeFlow Logo" class="brand-logo-img" />
          </div>
        <h2 class="brand-title">Create Account</h2>
        <p class="brand-subtitle">Join the AI Blood Forecast System</p>

        <form [formGroup]="registerForm" (ngSubmit)="onSubmit()">
          <div class="form-group">
            <label>Username</label>
            <input type="text" formControlName="username" placeholder="username" />
          </div>

          <div class="form-group">
            <label>Email address</label>
            <input type="email" formControlName="email" placeholder="you@example.com" />
          </div>

          <!-- Added Phone Number Field -->
          <div class="form-group">
            <label>Phone Number</label>
            <input type="tel" formControlName="phoneNumber" placeholder="e.g.,+91 1234567890" />
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
    .brand-logo-container {
      display: flex;
      justify-content: center;
    }
    .brand-logo-img {
      width: 140px;        
      height: 80px;       
      object-fit: contain;
    }
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

  // 1. Added phoneNumber control to the form group
  registerForm = this.fb.group({
    username: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.required]], // Remove Validators.required if phone is optional
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  onSubmit() {
    if (this.registerForm.invalid) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    // 2. Pass the dynamic form data directly to the service payload
    const formPayload = this.registerForm.value;

    this.authService.register(formPayload as any).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.isLoading.set(false);
        const errorMsg = err.error?.message || err.error || 'Registration failed. Check details.';
        this.errorMessage.set(Array.isArray(errorMsg) ? errorMsg.join(', ') : errorMsg);
      }
    });
  }
}