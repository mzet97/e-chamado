import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseResultList, StatusTypeResponse } from '../../core/models/api.interfaces';

@Component({
  selector: 'app-statustypes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="space-y-4">
      <div class="flex items-center justify-between">
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Status</h1>
        <button (click)="showCreate()" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 text-sm">➕ Novo</button>
      </div>
      <div class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 overflow-x-auto">
        <table class="w-full text-sm">
          <thead><tr class="border-b border-gray-200 dark:border-gray-800 bg-gray-50 dark:bg-gray-800/50">
            <th class="text-left p-4 font-medium text-gray-500">Nome</th>
            <th class="text-left p-4 font-medium text-gray-500">Descrição</th>
            <th class="text-right p-4 font-medium text-gray-500">Ações</th>
          </tr></thead>
          <tbody>
            <tr *ngFor="let s of items" class="border-b border-gray-100 dark:border-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800/50">
              <td class="p-4 font-medium text-gray-900 dark:text-white">{{ s.name }}</td>
              <td class="p-4 text-gray-600 dark:text-gray-400">{{ s.description || '-' }}</td>
              <td class="p-4 text-right flex justify-end gap-2">
                <button (click)="del(s)" class="p-2 text-red-600 hover:bg-red-50 rounded-lg">🗑️</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
})
export class StatusTypesComponent implements OnInit {
  items: StatusTypeResponse[] = [];
  constructor(private http: HttpClient) {}
  async ngOnInit(): Promise<void> {
    const r = await this.http.get<any>(`${environment.apiUrl}/v1/statustypes`, { params: { PageSize: 100 } }).toPromise();
    if (r?.success) this.items = r.data;
  }
  showCreate(): void { /* TODO: dialog */ }
  async del(s: StatusTypeResponse): Promise<void> {
    if (!confirm(`Excluir "${s.name}"?`)) return;
    await this.http.delete(`${environment.apiUrl}/v1/statustypes/${s.id}`).toPromise();
    const r = await this.http.get<any>(`${environment.apiUrl}/v1/statustypes`, { params: { PageSize: 100 } }).toPromise();
    if (r?.success) this.items = r.data;
  }
}
