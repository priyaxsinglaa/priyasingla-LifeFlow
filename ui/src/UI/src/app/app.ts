import { Component, signal } from '@angular/core';
import {  Router, NavigationEnd, RouterOutlet, RouterLink, RouterLinkActive} from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonModule } from '@angular/common'; // Needed for @if
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('UI');
  showSidebar = true;

  constructor(private router: Router) {
    // Listen for route changes
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      // Hide sidebar if the path is empty, /login, or /register
      const currentRoute = event.urlAfterRedirects;
      this.showSidebar = !(
        currentRoute === '/' || 
        currentRoute.includes('/login') || 
        currentRoute.includes('/register')
      );
    });
  }
}


