import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseResultList, UserResponse } from '../../core/models/api.interfaces';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="space-y-4">
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Usuários</h1>
      <div class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 overflow-x-auto">
        <table class="w-full text-sm">
          <thead><tr class="border-b border-gray-200 dark:border-gray-800 bg-gray-50 dark:bg-gray-800/50">
            <th class="text-left p-4 font-medium text-gray-500">Email</th>
            <th class="text-left p-4 font-medium text-gray-500">Usuário</th>
            <th class="text-left p-4 font-medium text-gray-500">Email OK</th>
            <th class="text-left p-4 font-medium text-gray-500">2FA</th>
            <th class="text-left p-4 font-medium text-gray-500">Status</th>
          </tr></thead>
          <tbody>
            <tr *ngIf="loading"><td colspan="5" class="p-4 text-center text-gray-400">Carregando...</td></tr>
            <tr *ngFor="let u of users" class="border-b border-gray-100 dark:border-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800/50">
              <td class="p-4 text-gray-900 dark:text-white">{{ u.email }}</td>
              <td class="p-4 text-gray-600 dark:text-gray-400">{{ u.userName }}</td>
              <td class="p-4">{{ u.emailConfirmed ? '✅' : '❌' }}</td>
              <td class="p-4">{{ u.twoFactorEnabled ? '✅' : '-' }}</td>
              <td class="p-4"><span [class]="u.lockoutEnd ? 'text-red-600' : 'text-green-600'">{{ u.lockoutEnd ? '🔒 Bloqueado' : '✅ Ativo' }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
})
export class UsersComponent implements OnInit {
  users: UserResponse[] = [];
  loading = true;
  constructor(private http: HttpClient) {}
  async ngOnInit(): Promise<void> {
    const r = await this.http.get<BaseResultList<UserResponse>>(`${environment.apiUrl}/v1/users`, { params: { PageIndex: 1, PageSize: 100 } }).toPromise();
    if (r?.success) this.users = r.data;
    this.loading = false;
  }
}
