import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseResultList, RoleResponse } from '../../core/models/api.interfaces';

@Component({
  selector: 'app-roles',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="space-y-4">
      <div class="flex items-center justify-between">
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Perfis</h1>
        <button (click)="showDialog = true; editId = ''; formName = ''; formDesc = ''" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 text-sm">➕ Novo</button>
      </div>
      <div class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 overflow-x-auto">
        <table class="w-full text-sm">
          <thead><tr class="border-b border-gray-200 dark:border-gray-800 bg-gray-50 dark:bg-gray-800/50">
            <th class="text-left p-4 font-medium text-gray-500">Nome</th>
            <th class="text-left p-4 font-medium text-gray-500">Descrição</th>
            <th class="text-right p-4 font-medium text-gray-500">Ações</th>
          </tr></thead>
          <tbody>
            <tr *ngFor="let r of items" class="border-b border-gray-100 dark:border-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800/50">
              <td class="p-4 font-medium text-gray-900 dark:text-white">{{ r.name }}</td>
              <td class="p-4 text-gray-600 dark:text-gray-400">{{ r.description || '-' }}</td>
              <td class="p-4 text-right flex justify-end gap-2">
                <button (click)="edit(r)" class="p-2 text-blue-600 hover:bg-blue-50 rounded-lg">✏️</button>
                <button (click)="del(r)" class="p-2 text-red-600 hover:bg-red-50 rounded-lg">🗑️</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Dialog -->
    <div *ngIf="showDialog" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50" (click)="showDialog = false">
      <div class="bg-white dark:bg-gray-900 rounded-xl p-6 w-full max-w-md mx-4" (click)="$event.stopPropagation()">
        <h3 class="text-lg font-semibold mb-4 text-gray-900 dark:text-white">{{ editId ? 'Editar' : 'Novo' }} Perfil</h3>
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Nome *</label>
            <input type="text" [(ngModel)]="formName" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Descrição</label>
            <textarea [(ngModel)]="formDesc" rows="3" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white resize-none"></textarea>
          </div>
        </div>
        <div class="flex gap-3 justify-end mt-4">
          <button (click)="showDialog = false" class="px-4 py-2 border border-gray-300 dark:border-gray-700 rounded-lg">Cancelar</button>
          <button (click)="save()" [disabled]="!formName.trim() || saving" class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">{{ saving ? 'Salvando...' : 'Salvar' }}</button>
        </div>
      </div>
    </div>
  `,
})
export class RolesComponent implements OnInit {
  items: RoleResponse[] = [];
  showDialog = false;
  editId = '';
  formName = '';
  formDesc = '';
  saving = false;

  constructor(private http: HttpClient) {}

  async ngOnInit(): Promise<void> { await this.load(); }

  async load(): Promise<void> {
    const r = await this.http.get<BaseResultList<RoleResponse>>(`${environment.apiUrl}/v1/role`, { params: { PageSize: 100 } }).toPromise();
    if (r?.success) this.items = r.data;
  }

  edit(r: RoleResponse): void {
    this.editId = r.id;
    this.formName = r.name;
    this.formDesc = r.description || '';
    this.showDialog = true;
  }

  async save(): Promise<void> {
    if (!this.formName.trim()) return;
    this.saving = true;
    try {
      const payload = { name: this.formName, description: this.formDesc, permissions: [] };
      const r = this.editId
        ? await this.http.put<any>(`${environment.apiUrl}/v1/role/${this.editId}`, payload).toPromise()
        : await this.http.post<any>(`${environment.apiUrl}/v1/role`, payload).toPromise();
      if (r?.success) { this.showDialog = false; await this.load(); }
    } finally { this.saving = false; }
  }

  async del(r: RoleResponse): Promise<void> {
    if (!confirm(`Excluir "${r.name}"?`)) return;
    await this.http.delete(`${environment.apiUrl}/v1/role/${r.id}`).toPromise();
    await this.load();
  }
}
