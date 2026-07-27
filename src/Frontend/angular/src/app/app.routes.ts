import { Routes } from '@angular/router';
import { authGuard, roleGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // Auth
  { path: 'auth/login', loadComponent: () => import('./pages/auth/login.component').then(m => m.LoginComponent) },
  { path: 'auth/callback', loadComponent: () => import('./pages/auth/callback.component').then(m => m.CallbackComponent) },
  { path: 'auth/logout', loadComponent: () => import('./pages/auth/logout.component').then(m => m.LogoutComponent) },

  // Dashboard
  { path: '', loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent), canActivate: [authGuard] },

  // Orders
  { path: 'orders', loadComponent: () => import('./pages/orders/order-list.component').then(m => m.OrderListComponent), canActivate: [authGuard] },
  { path: 'orders/create', loadComponent: () => import('./pages/orders/order-form.component').then(m => m.OrderFormComponent), canActivate: [authGuard] },
  { path: 'orders/:id', loadComponent: () => import('./pages/orders/order-details.component').then(m => m.OrderDetailsComponent), canActivate: [authGuard] },
  { path: 'orders/:id/edit', loadComponent: () => import('./pages/orders/order-form.component').then(m => m.OrderFormComponent), canActivate: [authGuard] },

  // Admin
  { path: 'admin/categories', loadComponent: () => import('./pages/admin/categories.component').then(m => m.CategoriesComponent), canActivate: [authGuard, roleGuard], data: { role: 'Admin' } },
  { path: 'admin/categories/create', loadComponent: () => import('./pages/admin/category-form.component').then(m => m.CategoryFormComponent), canActivate: [authGuard, roleGuard], data: { role: 'Admin' } },
  { path: 'admin/categories/:id/edit', loadComponent: () => import('./pages/admin/category-form.component').then(m => m.CategoryFormComponent), canActivate: [authGuard, roleGuard], data: { role: 'Admin' } },
  { path: 'admin/departments', loadComponent: () => import('./pages/admin/departments.component').then(m => m.DepartmentsComponent), canActivate: [authGuard, roleGuard], data: { role: 'Admin' } },
  { path: 'admin/departments/create', loadComponent: () => import('./pages/admin/department-form.component').then(m => m.DepartmentFormComponent), canActivate: [authGuard, roleGuard], data: { role: 'Admin' } },
  { path: 'admin/departments/:id/edit', loadComponent: () => import('./pages/admin/department-form.component').then(m => m.DepartmentFormComponent), canActivate: [authGuard, roleGuard], data: { role: 'Admin' } },
  { path: 'admin/statustypes', loadComponent: () => import('./pages/admin/statustypes.component').then(m => m.StatusTypesComponent), canActivate: [authGuard, roleGuard], data: { role: 'Admin' } },
  { path: 'admin/users', loadComponent: () => import('./pages/admin/users.component').then(m => m.UsersComponent), canActivate: [authGuard, roleGuard], data: { role: 'Admin' } },
  { path: 'admin/roles', loadComponent: () => import('./pages/admin/roles.component').then(m => m.RolesComponent), canActivate: [authGuard, roleGuard], data: { role: 'Admin' } },

  { path: '**', redirectTo: '' },
];
