import apiClient from '@/lib/api-client';
import { BaseResult, BaseResultList, DepartmentResponse, CreateDepartmentRequest, UpdateDepartmentRequest } from '@/types/api';

export const departmentService = {
  async getAll(pageSize = 100) {
    const { data } = await apiClient.get<BaseResultList<DepartmentResponse>>('/v1/departments', { params: { PageSize: pageSize } });
    return data;
  },
  async getById(id: string) {
    const { data } = await apiClient.get<BaseResult<DepartmentResponse>>(`/v1/departments/${id}`);
    return data;
  },
  async create(req: CreateDepartmentRequest) {
    const { data } = await apiClient.post<BaseResult<string>>('/v1/departments', req);
    return data;
  },
  async update(id: string, req: UpdateDepartmentRequest) {
    const { data } = await apiClient.put<BaseResult>(`/v1/departments/${id}`, req);
    return data;
  },
  async delete(id: string) {
    const { data } = await apiClient.delete<BaseResult>(`/v1/departments/${id}`);
    return data;
  },
};
