import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseResultList, CategoryResponse } from '../../core/models/api.interfaces';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="space-y-4">
      <div class="flex items-center justify-between">
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Categorias</h1>
        <a routerLink="/admin/categories/create" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 text-sm">➕ Nova</a>
      </div>
      <div class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800">
        <div class="p-4 border-b border-gray-200 dark:border-gray-800">
          <input type="text" placeholder="Buscar..." [(ngModel)]="search"
            class="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white" />
        </div>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead><tr class="border-b border-gray-200 dark:border-gray-800 bg-gray-50 dark:bg-gray-800/50">
              <th class="text-left p-4 font-medium text-gray-500">Nome</th>
              <th class="text-left p-4 font-medium text-gray-500">Descrição</th>
              <th class="text-left p-4 font-medium text-gray-500">Subcategorias</th>
              <th class="text-right p-4 font-medium text-gray-500">Ações</th>
            </tr></thead>
            <tbody>
              <tr *ngIf="loading"><td colspan="4" class="p-4 text-center text-gray-400">Carregando...</td></tr>
              <tr *ngFor="let c of filtered" class="border-b border-gray-100 dark:border-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800/50">
                <td class="p-4 font-medium text-gray-900 dark:text-white">{{ c.name }}</td>
                <td class="p-4 text-gray-600 dark:text-gray-400">{{ c.description || '-' }}</td>
                <td class="p-4 text-gray-600 dark:text-gray-400">{{ c.subCategories?.length || 0 }}</td>
                <td class="p-4 text-right flex justify-end gap-2">
                  <a [routerLink]="['/admin/categories', c.id, 'edit']" class="p-2 text-blue-600 hover:bg-blue-50 dark:hover:bg-blue-900/20 rounded-lg">✏️</a>
                  <button (click)="del(c)" class="p-2 text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20 rounded-lg">🗑️</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `,
})
export class CategoriesComponent implements OnInit {
  items: CategoryResponse[] = [];
  loading = true;
  search = '';

  get filtered() { return this.items.filter(c => c.name.toLowerCase().includes(this.search.toLowerCase())); }

  constructor(private http: HttpClient) {}
  async ngOnInit(): Promise<void> { await this.load(); }

  async load(): Promise<void> {
    this.loading = true;
    const res = await this.http.get<BaseResultList<CategoryResponse>>(`${environment.apiUrl}/v1/categories`, { params: { PageSize: 100 } }).toPromise();
    if (res?.success) this.items = res.data;
    this.loading = false;
  }

  async del(c: CategoryResponse): Promise<void> {
    if (!confirm(`Excluir "${c.name}"?`)) return;
    await this.http.delete(`${environment.apiUrl}/v1/categories/${c.id}`).toPromise();
    await this.load();
  }
}
