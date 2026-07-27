import { Component, OnInit } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './core/services/auth.service';
import { UserInfo } from './core/models/api.interfaces';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="min-h-screen bg-gray-50 dark:bg-gray-950" [class.dark]="isDark">
      <aside class="fixed top-0 left-0 z-50 h-full w-64 bg-white dark:bg-gray-900 border-r border-gray-200 dark:border-gray-800 transform transition-transform"
             [class.-translate-x-full]="!sidebarOpen" [class.lg\:translate-x-0]="true">
        <div class="flex items-center justify-between p-4 border-b border-gray-200 dark:border-gray-800">
          <div class="flex items-center gap-2">
            <div class="w-8 h-8 bg-blue-600 rounded-lg flex items-center justify-center text-white text-sm font-bold">E</div>
            <span class="text-lg font-bold text-gray-900 dark:text-white">EChamado</span>
          </div>
          <button (click)="sidebarOpen = false" class="lg:hidden p-1 rounded hover:bg-gray-100">✕</button>
        </div>
        <nav class="p-3 space-y-1">
          <a routerLink="/" routerLinkActive="active-link" [routerLinkActiveOptions]="{exact:true}" class="nav-item">🏠 Dashboard</a>
          <a routerLink="/orders" routerLinkActive="active-link" class="nav-item">📋 Chamados</a>
          <a routerLink="/orders/create" routerLinkActive="active-link" class="nav-item">➕ Novo Chamado</a>
          <div class="border-t border-gray-200 dark:border-gray-700 my-2"></div>
          <ng-container *ngIf="user?.roles?.includes('Admin')">
            <p class="px-3 py-1 text-xs font-semibold text-gray-400 uppercase">Administração</p>
            <a routerLink="/admin/categories" routerLinkActive="active-link" class="nav-item">📁 Categorias</a>
            <a routerLink="/admin/departments" routerLinkActive="active-link" class="nav-item">🏢 Departamentos</a>
            <a routerLink="/admin/statustypes" routerLinkActive="active-link" class="nav-item">📊 Status</a>
            <a routerLink="/admin/users" routerLinkActive="active-link" class="nav-item">👥 Usuários</a>
            <a routerLink="/admin/roles" routerLinkActive="active-link" class="nav-item">🛡️ Perfis</a>
          </ng-container>
        </nav>
      </aside>

      <div *ngIf="sidebarOpen" class="fixed inset-0 z-40 bg-black/50 lg:hidden" (click)="sidebarOpen = false"></div>

      <div class="lg:ml-64">
        <header class="sticky top-0 z-30 flex items-center justify-between h-16 px-4 bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-800">
          <button (click)="sidebarOpen = !sidebarOpen" class="lg:hidden p-2 rounded-lg hover:bg-gray-100">☰</button>
          <h1 class="text-lg font-semibold text-gray-900 dark:text-white hidden sm:block">Sistema de Gestão de Chamados</h1>
          <div class="flex items-center gap-2">
            <button (click)="toggleDark()" class="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 text-gray-600 dark:text-gray-400">
              {{ isDark ? '☀️' : '🌙' }}
            </button>
            <ng-container *ngIf="user; else loginBtn">
              <span class="hidden sm:block text-sm text-gray-700 dark:text-gray-300">{{ user.userName }}</span>
              <div class="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center text-white text-sm">👤</div>
              <button (click)="authService.logout()" class="p-2 rounded-lg hover:bg-gray-100 text-gray-600">🚪</button>
            </ng-container>
            <ng-template #loginBtn>
              <button (click)="authService.login()" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 text-sm">Entrar</button>
            </ng-template>
          </div>
        </header>
        <main class="p-4 md:p-6 lg:p-8"><router-outlet></router-outlet></main>
      </div>
    </div>
  `,
  styles: [`
    .nav-item {
      display: flex; align-items: center; gap: 0.75rem;
      padding: 0.5rem 0.75rem; border-radius: 0.5rem;
      font-size: 0.875rem; color: #374151;
      transition: background-color 0.2s;
    }
    .nav-item:hover { background-color: #f3f4f6; }
    :host-context(.dark) .nav-item { color: #d1d5db; }
    :host-context(.dark) .nav-item:hover { background-color: #1f2937; }
    .active-link { background-color: #eff6ff; color: #1d4ed8; font-weight: 500; }
    :host-context(.dark) .active-link { background-color: rgba(30,58,138,0.3); color: #60a5fa; }
  `],
})
export class AppComponent implements OnInit {
  sidebarOpen = false;
  isDark = false;
  user: UserInfo | null = null;

  constructor(public authService: AuthService) {}

  ngOnInit(): void {
    this.user = this.authService.getUser();
    this.isDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
  }

  toggleDark(): void {
    this.isDark = !this.isDark;
    document.documentElement.classList.toggle('dark', this.isDark);
  }
}
