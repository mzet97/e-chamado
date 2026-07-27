import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/services/auth.service';
import { BaseResult, BaseResultList, DashboardStatsResponse, OrderListViewModel } from '../../core/models/api.interfaces';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="space-y-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Dashboard</h1>
          <p class="text-gray-500 dark:text-gray-400">Bem-vindo, {{ user?.userName || 'Usuário' }}</p>
        </div>
        <a routerLink="/orders/create" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 text-sm">➕ Novo Chamado</a>
      </div>

      <!-- KPI Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div *ngFor="let card of kpiCards" class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5 flex items-center gap-4">
          <div [class]="card.color + ' p-3 rounded-lg text-white text-xl'">{{ card.icon }}</div>
          <div>
            <p class="text-sm text-gray-500 dark:text-gray-400">{{ card.label }}</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ loading ? '...' : card.value }}</p>
          </div>
        </div>
      </div>

      <!-- Recent Orders -->
      <div class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800">
        <div class="p-5 border-b border-gray-200 dark:border-gray-800 flex items-center justify-between">
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white">Últimos Chamados</h2>
          <a routerLink="/orders" class="text-sm text-blue-600 hover:underline">Ver todos →</a>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead><tr class="border-b border-gray-200 dark:border-gray-800">
              <th class="text-left p-4 text-gray-500 font-medium">Título</th>
              <th class="text-left p-4 text-gray-500 font-medium">Status</th>
              <th class="text-left p-4 text-gray-500 font-medium">Tipo</th>
              <th class="text-left p-4 text-gray-500 font-medium">Data</th>
            </tr></thead>
            <tbody>
              <tr *ngIf="loading"><td colspan="4" class="p-4 text-center text-gray-400">Carregando...</td></tr>
              <tr *ngIf="!loading && recentOrders.length === 0"><td colspan="4" class="p-8 text-center text-gray-500">✅ Nenhum chamado encontrado</td></tr>
              <tr *ngFor="let o of recentOrders" class="border-b border-gray-100 dark:border-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800/50">
                <td class="p-4"><a [routerLink]="['/orders', o.id]" class="text-blue-600 hover:underline font-medium">{{ o.title }}</a></td>
                <td class="p-4"><span class="px-2 py-1 rounded-full text-xs" [class]="statusClass(o.statusName)">{{ o.statusName }}</span></td>
                <td class="p-4 text-gray-600 dark:text-gray-400">{{ o.typeName }}</td>
                <td class="p-4 text-gray-500 text-xs">{{ o.openingDate | date:'dd/MM/yyyy' }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <a routerLink="/orders/create" class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5 hover:border-blue-300 transition-colors block">
          <div class="text-2xl mb-2">➕</div>
          <h3 class="font-semibold text-gray-900 dark:text-white">Novo Chamado</h3>
          <p class="text-sm text-gray-500">Criar uma nova solicitação</p>
        </a>
        <a routerLink="/orders" class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5 hover:border-blue-300 transition-colors block">
          <div class="text-2xl mb-2">📋</div>
          <h3 class="font-semibold text-gray-900 dark:text-white">Listar Todos</h3>
          <p class="text-sm text-gray-500">Ver todos os chamados</p>
        </a>
        <a routerLink="/orders" [queryParams]="{assigned: true}" class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5 hover:border-blue-300 transition-colors block">
          <div class="text-2xl mb-2">⏰</div>
          <h3 class="font-semibold text-gray-900 dark:text-white">Meus Atribuídos</h3>
          <p class="text-sm text-gray-500">Chamados atribuídos a mim</p>
        </a>
      </div>
    </div>
  `,
})
export class DashboardComponent implements OnInit {
  user: any;
  stats: DashboardStatsResponse | null = null;
  recentOrders: OrderListViewModel[] = [];
  loading = true;

  kpiCards = [
    { label: 'Total de Chamados', value: 0, icon: '📋', color: 'bg-blue-500' },
    { label: 'Meus Chamados', value: 0, icon: '📝', color: 'bg-indigo-500' },
    { label: 'Atribuídos a Mim', value: 0, icon: '⏰', color: 'bg-amber-500' },
    { label: 'Em Atraso', value: 0, icon: '⚠️', color: 'bg-red-500' },
  ];

  constructor(private http: HttpClient, private authService: AuthService) {}

  async ngOnInit(): Promise<void> {
    this.user = this.authService.getUser();
    try {
      const [statsRes, ordersRes] = await Promise.all([
        this.http.get<BaseResult<DashboardStatsResponse>>(`${environment.apiUrl}/v1/dashboard/stats`).toPromise(),
        this.http.get<BaseResultList<OrderListViewModel>>(`${environment.apiUrl}/v1/orders`, { params: { PageIndex: 1, PageSize: 10 } }).toPromise(),
      ]);
      if (statsRes?.success && statsRes.data) {
        this.stats = statsRes.data;
        this.kpiCards[0].value = this.stats.totalTickets;
        this.kpiCards[1].value = this.stats.myTickets;
        this.kpiCards[2].value = this.stats.assignedToMe;
        this.kpiCards[3].value = this.stats.overdueTickets;
      }
      if (ordersRes?.success) this.recentOrders = ordersRes.data;
    } catch (e) { console.error(e); }
    this.loading = false;
  }

  statusClass(status: string): string {
    const s = status?.toLowerCase() || '';
    if (s.includes('aberto') || s.includes('open')) return 'bg-blue-100 text-blue-700';
    if (s.includes('andamento') || s.includes('progress')) return 'bg-amber-100 text-amber-700';
    if (s.includes('fechado') || s.includes('closed') || s.includes('resolvido')) return 'bg-green-100 text-green-700';
    return 'bg-gray-100 text-gray-700';
  }
}
