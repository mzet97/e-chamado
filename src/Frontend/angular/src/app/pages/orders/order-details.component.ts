import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/services/auth.service';
import { BaseResult, BaseResultList, OrderViewModel, CommentResponse, StatusTypeResponse } from '../../core/models/api.interfaces';

@Component({
  selector: 'app-order-details',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="space-y-6" *ngIf="order">
      <div class="flex items-center justify-between">
        <div>
          <div class="flex items-center gap-3 mb-1">
            <a routerLink="/orders" class="text-gray-500 hover:text-gray-700">←</a>
            <h1 class="text-2xl font-bold text-gray-900 dark:text-white">{{ order.title }}</h1>
          </div>
          <p class="text-sm text-gray-500">Criado em {{ order.openingDate | date:'dd/MM/yyyy HH:mm' }}</p>
        </div>
        <a [routerLink]="['/orders', order.id, 'edit']" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 text-sm">✏️ Editar</a>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div class="lg:col-span-2 space-y-6">
          <div class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5">
            <h2 class="text-lg font-semibold mb-3 text-gray-900 dark:text-white">Descrição</h2>
            <p class="text-gray-700 dark:text-gray-300 whitespace-pre-wrap">{{ order.description }}</p>
          </div>
          <div class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5">
            <h2 class="text-lg font-semibold mb-4 text-gray-900 dark:text-white">Comentários</h2>
            <div *ngIf="comments.length === 0" class="text-gray-500 text-center py-4">Nenhum comentário ainda</div>
            <div *ngFor="let c of comments" class="flex gap-3 mb-3">
              <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900/30 rounded-full flex items-center justify-center text-blue-600 text-xs font-bold">{{ c.userEmail?.charAt(0)?.toUpperCase() }}</div>
              <div class="flex-1 bg-gray-50 dark:bg-gray-800 rounded-lg p-3">
                <div class="flex items-center gap-2 mb-1">
                  <span class="text-sm font-medium text-gray-900 dark:text-white">{{ c.userEmail }}</span>
                  <span *ngIf="c.isInternal" class="px-1.5 py-0.5 bg-amber-100 dark:bg-amber-900/30 text-amber-700 dark:text-amber-400 rounded text-xs">🔒 Interno</span>
                  <span class="text-xs text-gray-500">{{ c.createdAt | date:'dd/MM HH:mm' }}</span>
                </div>
                <p class="text-sm text-gray-700 dark:text-gray-300 whitespace-pre-wrap">{{ c.text }}</p>
              </div>
            </div>
            <div class="border-t border-gray-200 dark:border-gray-700 pt-4">
              <div class="flex items-center gap-2 mb-2">
                <input type="checkbox" id="internal" [(ngModel)]="isInternal" class="rounded" />
                <label for="internal" class="text-sm text-gray-600 dark:text-gray-400">🔒 Comentário interno</label>
              </div>
              <div class="flex gap-2">
                <textarea [(ngModel)]="newComment" placeholder="Adicionar comentário..." rows="3"
                  class="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white resize-none"></textarea>
                <button (click)="addComment()" [disabled]="!newComment.trim()" class="self-end px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">Enviar</button>
              </div>
            </div>
          </div>
        </div>

        <div class="space-y-6">
          <div class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5">
            <h2 class="text-lg font-semibold mb-3 text-gray-900 dark:text-white">Detalhes</h2>
            <dl class="space-y-3 text-sm">
              <div class="flex justify-between"><dt class="text-gray-500">Status</dt><dd><span class="px-2 py-1 rounded-full text-xs" [class]="statusClass(order.statusName)">{{ order.statusName }}</span></dd></div>
              <div class="flex justify-between"><dt class="text-gray-500">Tipo</dt><dd class="text-gray-900 dark:text-white">{{ order.typeName }}</dd></div>
              <div class="flex justify-between"><dt class="text-gray-500">Departamento</dt><dd class="text-gray-900 dark:text-white">{{ order.departmentName || '-' }}</dd></div>
              <div class="flex justify-between"><dt class="text-gray-500">Solicitante</dt><dd class="text-gray-900 dark:text-white">{{ order.requestingUserEmail }}</dd></div>
              <div class="flex justify-between"><dt class="text-gray-500">Responsável</dt><dd class="text-gray-900 dark:text-white">{{ order.responsibleUserEmail || 'Não atribuído' }}</dd></div>
            </dl>
          </div>
          <div class="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5">
            <h2 class="text-lg font-semibold mb-3 text-gray-900 dark:text-white">Ações</h2>
            <div class="space-y-3">
              <div>
                <label class="text-sm text-gray-600 dark:text-gray-400 mb-1 block">Alterar Status</label>
                <div class="flex gap-2">
                  <select [(ngModel)]="selectedStatus" class="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white">
                    <option *ngFor="let s of statuses" [value]="s.id">{{ s.name }}</option>
                  </select>
                  <button (click)="changeStatus()" [disabled]="selectedStatus === order.statusId" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">OK</button>
                </div>
              </div>
              <button *ngIf="!order.responsibleUserEmail" (click)="assignToMe()"
                class="w-full px-4 py-2 border border-blue-300 text-blue-600 rounded-lg hover:bg-blue-50 dark:hover:bg-blue-900/20">👤 Assumir Chamado</button>
              <button *ngIf="!order.closingDate" (click)="showCloseDialog = true"
                class="w-full px-4 py-2 border border-green-300 text-green-600 rounded-lg hover:bg-green-50 dark:hover:bg-green-900/20">✅ Fechar Chamado</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Close Dialog -->
    <div *ngIf="showCloseDialog" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50" (click)="showCloseDialog = false">
      <div class="bg-white dark:bg-gray-900 rounded-xl p-6 w-full max-w-md mx-4" (click)="$event.stopPropagation()">
        <h3 class="text-lg font-semibold mb-4 text-gray-900 dark:text-white">Fechar Chamado</h3>
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-4">Avalie o atendimento (opcional):</p>
        <div class="flex justify-center gap-2 mb-6">
          <button *ngFor="let n of [1,2,3,4,5]" (click)="evaluation = n"
            [class]="evaluation >= n ? 'text-yellow-500' : 'text-gray-300 dark:text-gray-600'" class="p-2 text-2xl">⭐</button>
        </div>
        <div class="flex gap-3 justify-end">
          <button (click)="showCloseDialog = false" class="px-4 py-2 border border-gray-300 dark:border-gray-700 rounded-lg">Cancelar</button>
          <button (click)="closeOrder()" class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700">Confirmar</button>
        </div>
      </div>
    </div>
  `,
})
export class OrderDetailsComponent implements OnInit {
  order: OrderViewModel | null = null;
  comments: CommentResponse[] = [];
  statuses: StatusTypeResponse[] = [];
  selectedStatus = '';
  newComment = '';
  isInternal = false;
  evaluation = 0;
  showCloseDialog = false;

  constructor(private http: HttpClient, private route: ActivatedRoute, private authService: AuthService) {}

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.paramMap.get('id')!;
    try {
      const [orderRes, statusRes, commentsRes] = await Promise.all([
        this.http.get<BaseResult<OrderViewModel>>(`${environment.apiUrl}/v1/orders/${id}`).toPromise(),
        this.http.get<any>(`${environment.apiUrl}/v1/statustypes`, { params: { PageSize: 100 } }).toPromise(),
        this.http.get<BaseResultList<CommentResponse>>(`${environment.apiUrl}/v1/comments/${id}/comments`).toPromise(),
      ]);
      if (orderRes?.success) { this.order = orderRes.data; this.selectedStatus = this.order.statusId; }
      if (statusRes?.success) this.statuses = statusRes.data;
      if (commentsRes?.success) this.comments = commentsRes.data;
    } catch {}
  }

  async addComment(): Promise<void> {
    if (!this.newComment.trim() || !this.order) return;
    const user = this.authService.getUser()!;
    await this.http.post(`${environment.apiUrl}/v1/comments`, {
      orderId: this.order.id, description: this.newComment,
      userId: user.userId, userEmail: user.email, isInternal: this.isInternal
    }).toPromise();
    this.newComment = '';
    this.isInternal = false;
    await this.reload();
  }

  async changeStatus(): Promise<void> {
    if (!this.order || this.selectedStatus === this.order.statusId) return;
    await this.http.post(`${environment.apiUrl}/v1/orders/status`, { orderId: this.order.id, statusTypeId: this.selectedStatus }).toPromise();
    await this.reload();
  }

  async assignToMe(): Promise<void> {
    if (!this.order) return;
    const userId = this.authService.getUser()?.userId;
    await this.http.post(`${environment.apiUrl}/v1/orders/assign`, { orderId: this.order.id, assignedToUserId: userId }).toPromise();
    await this.reload();
  }

  async closeOrder(): Promise<void> {
    if (!this.order) return;
    await this.http.post(`${environment.apiUrl}/v1/orders/close`, { orderId: this.order.id, evaluation: this.evaluation || null }).toPromise();
    this.showCloseDialog = false;
    await this.reload();
  }

  private async reload(): Promise<void> {
    const id = this.route.snapshot.paramMap.get('id')!;
    const [orderRes, commentsRes] = await Promise.all([
      this.http.get<BaseResult<OrderViewModel>>(`${environment.apiUrl}/v1/orders/${id}`).toPromise(),
      this.http.get<BaseResultList<CommentResponse>>(`${environment.apiUrl}/v1/comments/${id}/comments`).toPromise(),
    ]);
    if (orderRes?.success) this.order = orderRes.data;
    if (commentsRes?.success) this.comments = commentsRes.data;
  }

  statusClass(s: string): string {
    s = s?.toLowerCase() || '';
    if (s.includes('aberto') || s.includes('open')) return 'bg-blue-100 text-blue-700';
    if (s.includes('andamento') || s.includes('progress')) return 'bg-amber-100 text-amber-700';
    if (s.includes('fechado') || s.includes('closed') || s.includes('resolvido')) return 'bg-green-100 text-green-700';
    return 'bg-gray-100 text-gray-700';
  }
}
