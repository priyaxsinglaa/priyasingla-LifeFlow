import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  
  // Replace with actual login credentials
  if (authService.isLoggedIn()) {
    return true;
  } else {
    // Redirect unauthenticated users back to the login screen
    router.navigate(['/login']);
    return false;
  }
};
