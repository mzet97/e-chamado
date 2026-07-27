import apiClient from '@/lib/api-client';
import { BaseResult, BaseResultList, OrderTypeResponse, StatusTypeResponse, DepartmentResponse, CategoryResponse, CreateOrderTypeRequest, UpdateOrderTypeRequest, CreateStatusTypeRequest, UpdateStatusTypeRequest } from '@/types/api';

// In-memory cache (same strategy as Blazor client)
let cachedOrderTypes: OrderTypeResponse[] | null = null;
let cachedStatusTypes: StatusTypeResponse[] | null = null;
let cachedDepartments: DepartmentResponse[] | null = null;
let cachedCategories: CategoryResponse[] | null = null;

export const lookupService = {
  clearCache() {
    cachedOrderTypes = null;
    cachedStatusTypes = null;
    cachedDepartments = null;
    cachedCategories = null;
  },

  async getOrderTypes(forceRefresh = false) {
    if (!forceRefresh && cachedOrderTypes) return cachedOrderTypes;
    const { data } = await apiClient.get<BaseResultList<OrderTypeResponse>>('/v1/ordertypes', { params: { PageSize: 100 } });
    cachedOrderTypes = data.data || [];
    return cachedOrderTypes;
  },

  async getStatusTypes(forceRefresh = false) {
    if (!forceRefresh && cachedStatusTypes) return cachedStatusTypes;
    const { data } = await apiClient.get<BaseResultList<StatusTypeResponse>>('/v1/statustypes', { params: { PageSize: 100 } });
    cachedStatusTypes = data.data || [];
    return cachedStatusTypes;
  },

  async getDepartments(forceRefresh = false) {
    if (!forceRefresh && cachedDepartments) return cachedDepartments;
    const { data } = await apiClient.get<BaseResultList<DepartmentResponse>>('/v1/departments', { params: { PageSize: 100 } });
    cachedDepartments = data.data || [];
    return cachedDepartments;
  },

  async getCategories(forceRefresh = false) {
    if (!forceRefresh && cachedCategories) return cachedCategories;
    const { data } = await apiClient.get<BaseResultList<CategoryResponse>>('/v1/categories', { params: { PageSize: 100 } });
    cachedCategories = data.data || [];
    return cachedCategories;
  },

  async getSubCategories(categoryId: string) {
    const cats = await this.getCategories();
    const cat = cats.find(c => c.id === categoryId);
    return cat?.subCategories || [];
  },

  // CRUD for OrderTypes
  async createOrderType(req: CreateOrderTypeRequest) {
    const { data } = await apiClient.post<BaseResult<string>>('/v1/ordertypes', req);
    this.clearCache();
    return data;
  },
  async updateOrderType(id: string, req: UpdateOrderTypeRequest) {
    const { data } = await apiClient.put<BaseResult>(`/v1/ordertypes/${id}`, req);
    this.clearCache();
    return data;
  },
  async deleteOrderType(id: string) {
    const { data } = await apiClient.delete<BaseResult>(`/v1/ordertypes/${id}`);
    this.clearCache();
    return data;
  },

  // CRUD for StatusTypes
  async createStatusType(req: CreateStatusTypeRequest) {
    const { data } = await apiClient.post<BaseResult<string>>('/v1/statustypes', req);
    this.clearCache();
    return data;
  },
  async updateStatusType(id: string, req: UpdateStatusTypeRequest) {
    const { data } = await apiClient.put<BaseResult>(`/v1/statustypes/${id}`, req);
    this.clearCache();
    return data;
  },
  async deleteStatusType(id: string) {
    const { data } = await apiClient.delete<BaseResult>(`/v1/statustypes/${id}`);
    this.clearCache();
    return data;
  },
};
