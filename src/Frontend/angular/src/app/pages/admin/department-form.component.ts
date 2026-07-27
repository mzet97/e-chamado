import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-department-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="max-w-2xl mx-auto">
      <div class="flex items-center gap-3 mb-6">
        <a routerLink="/admin/departments" class="text-gray-500 hover:text-gray-700">←</a>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">{{ isEdit ? 'Editar' : 'Novo' }} Departamento</h1>
      </div>
      <form (ngSubmit)="save()" class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-6 space-y-5">
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Nome *</label>
          <input type="text" [(ngModel)]="name" name="name" required maxLength="100" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Descrição</label>
          <textarea [(ngModel)]="description" name="description" rows="3" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white resize-none"></textarea>
        </div>
        <div class="flex gap-3 pt-4 border-t border-gray-200 dark:border-gray-700">
          <a routerLink="/admin/departments" class="px-4 py-2 border border-gray-300 dark:border-gray-700 rounded-lg">Cancelar</a>
          <button type="submit" [disabled]="saving" class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">{{ saving ? 'Salvando...' : 'Salvar' }}</button>
        </div>
      </form>
    </div>
  `,
})
export class DepartmentFormComponent implements OnInit {
  name = ''; description = ''; saving = false; isEdit = false; private editId = '';
  constructor(private http: HttpClient, private router: Router, private route: ActivatedRoute) {}
  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) { this.isEdit = true; this.editId = id;
      const r = await this.http.get<any>(`${environment.apiUrl}/v1/departments/${id}`).toPromise();
      if (r?.success) { this.name = r.data.name; this.description = r.data.description; }
    }
  }
  async save(): Promise<void> {
    if (!this.name.trim()) return; this.saving = true;
    try {
      const p = { name: this.name, description: this.description };
      const r = this.isEdit
        ? await this.http.put<any>(`${environment.apiUrl}/v1/departments/${this.editId}`, p).toPromise()
        : await this.http.post<any>(`${environment.apiUrl}/v1/departments`, p).toPromise();
      if (r?.success) this.router.navigate(['/admin/departments']);
    } finally { this.saving = false; }
  }
}
