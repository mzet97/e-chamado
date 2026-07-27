import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/services/auth.service';
import { BaseResult, OrderTypeResponse, DepartmentResponse, CategoryResponse, SubCategoryResponse } from '../../core/models/api.interfaces';

@Component({
  selector: 'app-order-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="max-w-3xl mx-auto">
      <div class="flex items-center gap-3 mb-6">
        <a routerLink="/orders" class="text-gray-500 hover:text-gray-700">←</a>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">{{ isEdit ? 'Editar' : 'Novo' }} Chamado</h1>
      </div>
      <form (ngSubmit)="save()" class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-6 space-y-5">
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Título *</label>
          <input type="text" [(ngModel)]="form.title" name="title" maxLength="200" required
            class="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white" />
          <p class="text-xs text-gray-400 text-right mt-1">{{ form.title?.length || 0 }}/200</p>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Descrição *</label>
          <textarea [(ngModel)]="form.description" name="description" rows="5" maxLength="2000" required
            class="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white resize-none"></textarea>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Tipo *</label>
          <select [(ngModel)]="form.typeId" name="typeId" required class="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white">
            <option value="">Selecione...</option>
            <option *ngFor="let t of types" [value]="t.id">{{ t.name }}</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Departamento *</label>
          <select [(ngModel)]="form.departmentId" name="departmentId" required class="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white">
            <option value="">Selecione...</option>
            <option *ngFor="let d of departments" [value]="d.id">{{ d.name }}</option>
          </select>
        </div>
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Categoria *</label>
            <select [(ngModel)]="form.categoryId" name="categoryId" (ngModelChange)="onCategoryChange()" required class="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white">
              <option value="">Selecione...</option>
              <option *ngFor="let c of categories" [value]="c.id">{{ c.name }}</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Subcategoria</label>
            <select [(ngModel)]="form.subCategoryId" name="subCategoryId" [disabled]="!form.categoryId" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white disabled:opacity-50">
              <option value="">Selecione...</option>
              <option *ngFor="let s of subCategories" [value]="s.id">{{ s.name }}</option>
            </select>
          </div>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Prazo</label>
          <input type="date" [(ngModel)]="form.dueDate" name="dueDate" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white" />
        </div>
        <div class="flex gap-3 pt-4 border-t border-gray-200 dark:border-gray-700">
          <a routerLink="/orders" class="px-4 py-2 border border-gray-300 dark:border-gray-700 rounded-lg">Cancelar</a>
          <button type="submit" [disabled]="saving" class="flex items-center gap-2 px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">
            {{ saving ? 'Salvando...' : (isEdit ? 'Salvar' : 'Criar Chamado') }}
          </button>
        </div>
      </form>
    </div>
  `,
})
export class OrderFormComponent implements OnInit {
  form: any = { title: '', description: '', typeId: '', departmentId: '', categoryId: '', subCategoryId: '', dueDate: '' };
  types: OrderTypeResponse[] = [];
  departments: DepartmentResponse[] = [];
  categories: CategoryResponse[] = [];
  subCategories: SubCategoryResponse[] = [];
  saving = false;
  isEdit = false;
  private editId = '';

  constructor(private http: HttpClient, private router: Router, private route: ActivatedRoute, private authService: AuthService) {}

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) { this.isEdit = true; this.editId = id; }

    const [t, d, c] = await Promise.all([
      this.http.get<any>(`${environment.apiUrl}/v1/ordertypes`, { params: { PageSize: 100 } }).toPromise(),
      this.http.get<any>(`${environment.apiUrl}/v1/departments`, { params: { PageSize: 100 } }).toPromise(),
      this.http.get<any>(`${environment.apiUrl}/v1/categories`, { params: { PageSize: 100 } }).toPromise(),
    ]);
    if (t?.success) this.types = t.data;
    if (d?.success) this.departments = d.data;
    if (c?.success) this.categories = c.data;

    if (this.isEdit) {
      const res = await this.http.get<any>(`${environment.apiUrl}/v1/orders/${id}`).toPromise();
      if (res?.success) {
        const o = res.data;
        this.form = { title: o.title, description: o.description, typeId: o.typeId, departmentId: o.departmentId, categoryId: o.categoryId, subCategoryId: o.subCategoryId || '', dueDate: o.dueDate ? o.dueDate.split('T')[0] : '' };
      }
    } else {
      const user = this.authService.getUser();
      this.form.requestingUserId = user?.userId;
      this.form.requestingUserEmail = user?.email;
    }
  }

  onCategoryChange(): void {
    const cat = this.categories.find(c => c.id === this.form.categoryId);
    this.subCategories = cat?.subCategories || [];
    this.form.subCategoryId = '';
  }

  async save(): Promise<void> {
    this.saving = true;
    try {
      const payload = { ...this.form, requestingUserId: this.authService.getUser()?.userId, requestingUserEmail: this.authService.getUser()?.email };
      const res = await this.http.post<any>(`${environment.apiUrl}/v1/orders`, payload).toPromise();
      if (res?.success) this.router.navigate(['/orders', res.data]);
    } finally { this.saving = false; }
  }
}
