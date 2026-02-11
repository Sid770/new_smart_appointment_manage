import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { 
    path: 'dashboard', 
    loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent) 
  },
  { 
    path: 'available-slots', 
    loadComponent: () => import('./pages/available-slots/available-slots.component').then(m => m.AvailableSlotsComponent) 
  },
  { 
    path: 'book-appointment/:slotId', 
    loadComponent: () => import('./pages/book-appointment/book-appointment.component').then(m => m.BookAppointmentComponent) 
  },
  { 
    path: 'my-appointments', 
    loadComponent: () => import('./pages/my-appointments/my-appointments.component').then(m => m.MyAppointmentsComponent) 
  },
  { 
    path: 'admin', 
    loadComponent: () => import('./pages/admin/admin.component').then(m => m.AdminComponent) 
  },
  { path: '**', redirectTo: '/dashboard' }
];
