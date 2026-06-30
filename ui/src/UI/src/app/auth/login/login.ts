import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="auth-container">
      <div class="auth-card">
          <div class="brand-logo-container">
          <img src="lf.png" alt="LifeFlow Logo" class="brand-logo-img" />
          </div>
        <h2 class="brand-title">AI Blood Forecast</h2>
        <p class="brand-subtitle">Sign in to your account</p>

        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
          <div class="form-group">
            <label>Email address</label>
            <input type="email" formControlName="email" placeholder="you@example.com" />
          </div>

          <div class="form-group">
            <label>Password</label>
            <input type="password" formControlName="password" placeholder="••••••••" />
          </div>

          @if (errorMessage()) {
            <div class="error-msg">{{ errorMessage() }}</div>
          }

          <!-- Normal State Button -->
          <button type="submit" class="btn-submit" [disabled]="loginForm.invalid || isLoading()">
            Sign In
          </button>

          <!-- Status Indicator Match -->
          @if (isLoading()) {
            <div class="status-indicator">
              <span class="spinner"></span> Signing in...
            </div>
          }
        </form>

        <!-- Divider Interface Component -->
        <div class="divider">
          <span>or</span>
        </div>

        <!-- Real Backend Guest Login Button -->
        <button type="button" class="btn-guest" [disabled]="isLoading()" (click)="onGuestLogin()">
          @if (isLoading() && loginForm.invalid) {
            <span class="spinner dark"></span> Connecting...
          } @else {
            Explore as Guest
          }
        </button>

        <div class="auth-footer">
          Don't have an account? <a routerLink="/register">Register</a>
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
    .form-group input:focus { outline: none; border-color: #e57373; }

    .btn-submit { 
      width: 100%; background-color: #e57373; color: white; border: none; padding: 0.75rem; 
      border-radius: 6px; font-weight: 600; cursor: pointer; font-size: 0.95rem; margin-top: 0.5rem;
    }
    .btn-submit:disabled { opacity: 0.7; cursor: not-allowed; }

    .status-indicator {
      display: flex; align-items: center; justify-content: center; gap: 0.5rem;
      margin-top: 0.75rem; border: 1px solid #d1d5db; padding: 0.65rem; border-radius: 6px; color: #6b7280; font-size: 0.9rem;
    }
    .spinner { border: 2px solid #f3f3f3; border-top: 2px solid #6b7280; border-radius: 50%; width: 14px; height: 14px; animation: spin 1s linear infinite; }
    @keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }

    .error-msg { color: #dc2626; font-size: 0.85rem; text-align: left; margin-bottom: 0.75rem; }
    .auth-footer { margin-top: 1.5rem; font-size: 0.85rem; color: #4b5563; }
    .auth-footer a { color: #dc2626; text-decoration: none; font-weight: 600; }
    .auth-footer a:hover { text-decoration: underline; }
    .divider {
      display: flex;
      align-items: center;
      text-align: center;
      margin: 1.25rem 0;
      color: #9ca3af;
      font-size: 0.85rem;
    }
    .divider::before, .divider::after {
      content: '';
      flex: 1;
      border-bottom: 1px solid #e5e7eb;
    }
    .divider:not(:empty)::before { margin-right: .5em; }
    .divider:not(:empty)::after { margin-left: .5em; }

    .btn-guest {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
      width: 100%;
      background-color: transparent;
      color: #4b5563;
      border: 1px solid #d1d5db;
      padding: 0.75rem;
      border-radius: 6px;
      font-weight: 600;
      cursor: pointer;
      font-size: 0.95rem;
      transition: all 0.2s ease;
    }
    .btn-guest:hover:not(:disabled) {
      background-color: #f9fafb;
      border-color: #9ca3af;
      color: #111827;
    }
    .btn-guest:disabled {
      opacity: 0.7;
      cursor: not-allowed;
    }
    .spinner.dark {
      border-top-color: #6b7280;
    }
  `]
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  onSubmit() {
    if (this.loginForm.invalid) return;
    
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.authService.login(this.loginForm.value as any).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/dashboard']); // Navigate to app landing area
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.message || 'Invalid email or password.');
      }
    });
  }
  onGuestLogin() {
  this.isLoading.set(true);
  this.errorMessage.set(null);

  this.authService.guestLogin().subscribe({
    next: () => {
      this.isLoading.set(false);
      this.router.navigate(['/dashboard']); // Redirects once backend issues the token
    },
    error: (err) => {
      this.isLoading.set(false);
      // Grabs the error message sent from your .NET API, or defaults
      this.errorMessage.set(err.error?.message || 'Guest login is currently unavailable.');
    }
  });
}
}