import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseResultList, DepartmentResponse } from '../../core/models/api.interfaces';

@Component({
  selector: 'app-departments',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="space-y-4">
      <div class="flex items-center justify-between">
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Departamentos</h1>
        <a routerLink="/admin/departments/create" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 text-sm">➕ Novo</a>
      </div>
      <div class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 overflow-x-auto">
        <table class="w-full text-sm">
          <thead><tr class="border-b border-gray-200 dark:border-gray-800 bg-gray-50 dark:bg-gray-800/50">
            <th class="text-left p-4 font-medium text-gray-500">Nome</th>
            <th class="text-left p-4 font-medium text-gray-500">Descrição</th>
            <th class="text-right p-4 font-medium text-gray-500">Ações</th>
          </tr></thead>
          <tbody>
            <tr *ngIf="loading"><td colspan="3" class="p-4 text-center text-gray-400">Carregando...</td></tr>
            <tr *ngFor="let d of items" class="border-b border-gray-100 dark:border-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800/50">
              <td class="p-4 font-medium text-gray-900 dark:text-white">{{ d.name }}</td>
              <td class="p-4 text-gray-600 dark:text-gray-400">{{ d.description || '-' }}</td>
              <td class="p-4 text-right flex justify-end gap-2">
                <a [routerLink]="['/admin/departments', d.id, 'edit']" class="p-2 text-blue-600 hover:bg-blue-50 rounded-lg">✏️</a>
                <button (click)="del(d)" class="p-2 text-red-600 hover:bg-red-50 rounded-lg">🗑️</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
})
export class DepartmentsComponent implements OnInit {
  items: DepartmentResponse[] = [];
  loading = true;
  constructor(private http: HttpClient) {}
  async ngOnInit(): Promise<void> { await this.load(); }
  async load(): Promise<void> {
    this.loading = true;
    const r = await this.http.get<BaseResultList<DepartmentResponse>>(`${environment.apiUrl}/v1/departments`, { params: { PageSize: 100 } }).toPromise();
    if (r?.success) this.items = r.data;
    this.loading = false;
  }
  async del(d: DepartmentResponse): Promise<void> {
    if (!confirm(`Excluir "${d.name}"?`)) return;
    await this.http.delete(`${environment.apiUrl}/v1/departments/${d.id}`).toPromise();
    await this.load();
  }
}
