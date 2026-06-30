import { Routes } from '@angular/router';
import { authGuard } from './guard/auth-guard-guard';

export const routes: Routes = [
    {
        path: '', 
        loadComponent: () => import('./auth/login/login').then(m => m.LoginComponent),
        pathMatch: 'full'
    },
    {
        path: 'login', 
        loadComponent: () => import('./auth/login/login').then(m => m.LoginComponent)
    },
    {   
        path: 'register', 
        loadComponent: () => import('./auth/register/register').then(m => m.RegisterComponent)
    },
    {
        path: '',
        canActivate: [authGuard],
        children: 
        [
            { 
                path: 'dashboard',
                loadComponent: () => import('./components/dashboard/dashboard').then(m => m.DashboardComponent) 
            },
            { 
                path: 'donations', 
                loadComponent: () => import('./components/donation/donation').then(m => m.DonationsComponent) 
            },
            { 
                path: 'forecast', 
                loadComponent: () => import('./components/forecast/forecast').then(m => m.ForecastComponent) },
            { 
                path: 'alerts', 
                loadComponent: () => import('./components/alerts/alerts').then(m => m.AlertsComponent) },
            { 
                path: 'reports', 
                loadComponent: () => import('./components/reports/reports').then(m => m.ReportsComponent) 
            }
        ]
    },

    { 
        path: '**', 
        redirectTo: 'dashboard' 
    }

];
