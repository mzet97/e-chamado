import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/services/auth.service';
import { BaseResultList, OrderListViewModel, StatusTypeResponse, DepartmentResponse, OrderTypeResponse } from '../../core/models/api.interfaces';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="space-y-4">
      <div class="flex items-center justify-between">
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">{{ assignedOnly ? 'Meus Atribuídos' : 'Chamados' }}</h1>
        <a routerLink="/orders/create" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 text-sm">➕ Novo Chamado</a>
      </div>

      <div class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-4">
        <div class="flex gap-3">
          <input type="text" placeholder="Buscar por título..." [(ngModel)]="searchText" (keyup.enter)="loadOrders()"
                 class="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white" />
          <button (click)="showFilters = !showFilters" class="px-4 py-2 border border-gray-300 dark:border-gray-700 rounded-lg hover:bg-gray-50">🔍 Filtros</button>
        </div>
        <div *ngIf="showFilters" class="grid grid-cols-1 sm:grid-cols-3 gap-3 mt-3 pt-3 border-t border-gray-200 dark:border-gray-700">
          <select [(ngModel)]="selectedStatus" (change)="loadOrders()" class="px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white">
            <option value="">Todos os Status</option>
            <option *ngFor="let s of statuses" [value]="s.id">{{ s.name }}</option>
          </select>
          <select [(ngModel)]="selectedDept" (change)="loadOrders()" class="px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white">
            <option value="">Todos os Departamentos</option>
            <option *ngFor="let d of departments" [value]="d.id">{{ d.name }}</option>
          </select>
          <select [(ngModel)]="selectedType" (change)="loadOrders()" class="px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white">
            <option value="">Todos os Tipos</option>
            <option *ngFor="let t of types" [value]="t.id">{{ t.name }}</option>
          </select>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 overflow-hidden">
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead><tr class="border-b border-gray-200 dark:border-gray-800 bg-gray-50 dark:bg-gray-800/50">
              <th class="text-left p-4 font-medium text-gray-500">Título</th>
              <th class="text-left p-4 font-medium text-gray-500">Status</th>
              <th class="text-left p-4 font-medium text-gray-500">Tipo</th>
              <th class="text-left p-4 font-medium text-gray-500">Departamento</th>
              <th class="text-left p-4 font-medium text-gray-500">Responsável</th>
              <th class="text-left p-4 font-medium text-gray-500">Data</th>
              <th class="text-left p-4 font-medium text-gray-500">Ações</th>
            </tr></thead>
            <tbody>
              <tr *ngIf="loading"><td colspan="7" class="p-4 text-center text-gray-400">Carregando...</td></tr>
              <tr *ngIf="!loading && orders.length === 0"><td colspan="7" class="p-8 text-center text-gray-500">Nenhum chamado encontrado</td></tr>
              <tr *ngFor="let o of orders" class="border-b border-gray-100 dark:border-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800/50">
                <td class="p-4"><a [routerLink]="['/orders', o.id]" class="text-blue-600 hover:underline font-medium">{{ o.title }}</a></td>
                <td class="p-4"><span class="px-2 py-1 rounded-full text-xs" [class]="statusClass(o.statusName)">{{ o.statusName }}</span></td>
                <td class="p-4 text-gray-600 dark:text-gray-400">{{ o.typeName }}</td>
                <td class="p-4 text-gray-600 dark:text-gray-400">{{ o.departmentName || '-' }}</td>
                <td class="p-4 text-gray-600 dark:text-gray-400 text-xs">{{ o.responsibleUserEmail || '-' }}</td>
                <td class="p-4 text-gray-500 text-xs">{{ o.openingDate | date:'dd/MM/yyyy' }}</td>
                <td class="p-4 flex gap-2">
                  <a [routerLink]="['/orders', o.id]" class="text-blue-600 hover:underline text-xs">Ver</a>
                  <a [routerLink]="['/orders', o.id, 'edit']" class="text-gray-600 hover:underline text-xs">Editar</a>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div *ngIf="totalPages > 1" class="flex items-center justify-between p-4 border-t border-gray-200 dark:border-gray-800">
          <p class="text-sm text-gray-500">Mostrando {{ (page-1)*pageSize+1 }}-{{ min(page*pageSize, totalCount) }} de {{ totalCount }}</p>
          <div class="flex gap-2">
            <button (click)="page = page - 1; loadOrders()" [disabled]="page <= 1" class="px-3 py-1 border border-gray-300 rounded-lg disabled:opacity-50">←</button>
            <span class="px-3 py-1 text-sm">{{ page }} / {{ totalPages }}</span>
            <button (click)="page = page + 1; loadOrders()" [disabled]="page >= totalPages" class="px-3 py-1 border border-gray-300 rounded-lg disabled:opacity-50">→</button>
          </div>
        </div>
      </div>
    </div>
  `,
})
export class OrderListComponent implements OnInit {
  orders: OrderListViewModel[] = [];
  totalCount = 0;
  loading = true;
  page = 1;
  pageSize = 10;
  searchText = '';
  selectedStatus = '';
  selectedDept = '';
  selectedType = '';
  showFilters = false;
  assignedOnly = false;

  statuses: StatusTypeResponse[] = [];
  departments: DepartmentResponse[] = [];
  types: OrderTypeResponse[] = [];

  get totalPages() { return Math.ceil(this.totalCount / this.pageSize); }
  min = Math.min;

  constructor(private http: HttpClient, private route: ActivatedRoute, private authService: AuthService) {}

  async ngOnInit(): Promise<void> {
    this.assignedOnly = this.route.snapshot.queryParamMap.get('assigned') === 'true';
    try {
      const [s, d, t] = await Promise.all([
        this.http.get<any>(`${environment.apiUrl}/v1/statustypes`, { params: { PageSize: 100 } }).toPromise(),
        this.http.get<any>(`${environment.apiUrl}/v1/departments`, { params: { PageSize: 100 } }).toPromise(),
        this.http.get<any>(`${environment.apiUrl}/v1/ordertypes`, { params: { PageSize: 100 } }).toPromise(),
      ]);
      if (s?.success) this.statuses = s.data;
      if (d?.success) this.departments = d.data;
      if (t?.success) this.types = t.data;
    } catch {}
    await this.loadOrders();
  }

  async loadOrders(): Promise<void> {
    this.loading = true;
    const params: any = { PageIndex: this.page, PageSize: this.pageSize };
    if (this.searchText) params.Title = this.searchText;
    if (this.selectedStatus) params.StatusTypeId = this.selectedStatus;
    if (this.selectedDept) params.DepartmentId = this.selectedDept;
    if (this.selectedType) params.TypeId = this.selectedType;
    if (this.assignedOnly) params.AssignedToUserId = this.authService.getUser()?.userId;
    try {
      const res = await this.http.get<BaseResultList<OrderListViewModel>>(`${environment.apiUrl}/v1/orders`, { params }).toPromise();
      if (res?.success) { this.orders = res.data; this.totalCount = res.pagedResult.rowCount; }
    } catch {}
    this.loading = false;
  }

  statusClass(s: string): string {
    s = s?.toLowerCase() || '';
    if (s.includes('aberto') || s.includes('open')) return 'bg-blue-100 text-blue-700';
    if (s.includes('andamento') || s.includes('progress')) return 'bg-amber-100 text-amber-700';
    if (s.includes('fechado') || s.includes('closed') || s.includes('resolvido')) return 'bg-green-100 text-green-700';
    return 'bg-gray-100 text-gray-700';
  }
}
