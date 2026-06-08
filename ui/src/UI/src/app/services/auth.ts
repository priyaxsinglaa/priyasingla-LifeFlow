import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  username: string;
  email: string;
  phoneNumber: string; // Required by your backend DTO
  password: string;
}

export interface AuthResponseDto {
  token: string;
  username: string;
  email: string;
  phoneNumber: string;
  role: string;
  expiresAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5187/api/auth'; // Replace with your exact .NET API port

  // Modern state management using Angular Signals
  currentUser = signal<AuthResponseDto | null>(this.getStoredUser());

  login(dto: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, dto).pipe(
      tap(response => this.handleAuthSuccess(response))
    );
  }

  register(dto: RegisterDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/register`, dto).pipe(
      tap(response => this.handleAuthSuccess(response))
    );
  }

  logout(): void {
    localStorage.removeItem('user_session');
    this.currentUser.set(null);
  }

  private handleAuthSuccess(response: AuthResponseDto): void {
    localStorage.setItem('user_session', JSON.stringify(response));
    this.currentUser.set(response);
  }

  private getStoredUser(): AuthResponseDto | null {
    const data = localStorage.getItem('user_session');
    return data ? JSON.parse(data) : null;
  }
}