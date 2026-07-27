import apiClient from '@/lib/api-client';
import { BaseResult, BaseResultList, RoleResponse, CreateRoleRequest, UpdateRoleRequest } from '@/types/api';

export const roleService = {
  async getAll(pageSize = 100) {
    const { data } = await apiClient.get<BaseResultList<RoleResponse>>('/v1/role', { params: { PageSize: pageSize } });
    return data;
  },
  async getById(id: string) {
    const { data } = await apiClient.get<BaseResult<RoleResponse>>(`/v1/role/${id}`);
    return data;
  },
  async getByName(name: string) {
    const { data } = await apiClient.get<BaseResult<RoleResponse>>(`/v1/role/name/${name}`);
    return data;
  },
  async create(req: CreateRoleRequest) {
    const { data } = await apiClient.post<BaseResult<string>>('/v1/role', req);
    return data;
  },
  async update(id: string, req: UpdateRoleRequest) {
    const { data } = await apiClient.put<BaseResult>(`/v1/role/${id}`, req);
    return data;
  },
  async delete(id: string) {
    const { data } = await apiClient.delete<BaseResult>(`/v1/role/${id}`);
    return data;
  },
};
