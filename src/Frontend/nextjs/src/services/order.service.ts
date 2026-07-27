import apiClient from '@/lib/api-client';
import {
  BaseResult, BaseResultList, OrderViewModel, OrderListViewModel,
  CreateOrderRequest, UpdateOrderRequest, CloseOrderRequest,
  ChangeStatusRequest, AssignOrderRequest, SearchOrdersParameters,
  DashboardStatsResponse, CommentResponse, CreateCommentRequest,
} from '@/types/api';

export const orderService = {
  // Orders
  async search(params: SearchOrdersParameters) {
    const { data } = await apiClient.get<BaseResultList<OrderListViewModel>>('/v1/orders', { params });
    return data;
  },

  async getById(id: string) {
    const { data } = await apiClient.get<BaseResult<OrderViewModel>>(`/v1/orders/${id}`);
    return data;
  },

  async create(req: CreateOrderRequest) {
    const { data } = await apiClient.post<BaseResult<string>>('/v1/orders', req);
    return data;
  },

  async update(req: UpdateOrderRequest) {
    const { data } = await apiClient.put<BaseResult>('/v1/orders', req);
    return data;
  },

  async close(req: CloseOrderRequest) {
    const { data } = await apiClient.post<BaseResult>('/v1/orders/close', req);
    return data;
  },

  async assign(req: AssignOrderRequest) {
    const { data } = await apiClient.post<BaseResult>('/v1/orders/assign', req);
    return data;
  },

  async changeStatus(req: ChangeStatusRequest) {
    const { data } = await apiClient.post<BaseResult>('/v1/orders/status', req);
    return data;
  },

  // Gridify search
  async searchGridify(filter: string, orderBy?: string, page = 1, pageSize = 10) {
    const { data } = await apiClient.get<BaseResultList<OrderListViewModel>>('/v1/orders/gridify', {
      params: { Filter: filter, OrderBy: orderBy, Page: page, PageSize: pageSize },
    });
    return data;
  },

  // Comments
  async getComments(orderId: string) {
    const { data } = await apiClient.get<BaseResultList<CommentResponse>>(`/v1/comments/${orderId}/comments`);
    return data;
  },

  async addComment(req: CreateCommentRequest) {
    const { data } = await apiClient.post<BaseResult<string>>('/v1/comments', req);
    return data;
  },

  async deleteComment(id: string) {
    const { data } = await apiClient.delete<BaseResult>(`/v1/comments/${id}`);
    return data;
  },

  // Dashboard
  async getDashboardStats(userId?: string) {
    const { data } = await apiClient.get<BaseResult<DashboardStatsResponse>>('/v1/dashboard/stats', {
      params: userId ? { userId } : {},
    });
    return data;
  },

  // SLA
  async getSlaStats() {
    const { data } = await apiClient.get<BaseResult>('/v1/dashboard/sla/stats');
    return data;
  },

  // Team
  async getTeamStats() {
    const { data } = await apiClient.get<BaseResult>('/v1/dashboard/team/stats');
    return data;
  },

  // Full-text search
  async searchFullText(q: string, page = 1, pageSize = 10) {
    const { data } = await apiClient.get<BaseResultList<OrderListViewModel>>('/v1/orders/search', {
      params: { q, page, pageSize },
    });
    return data;
  },
};
