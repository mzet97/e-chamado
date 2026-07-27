import apiClient from '@/lib/api-client';
import { BaseResult, BaseResultList, UserResponse } from '@/types/api';

export const userService = {
  async getAll(pageIndex = 1, pageSize = 100) {
    const { data } = await apiClient.get<BaseResultList<UserResponse>>('/v1/users', { params: { PageIndex: pageIndex, PageSize: pageSize } });
    return data;
  },
  async getById(id: string) {
    const { data } = await apiClient.get<BaseResult<UserResponse>>(`/v1/users/${id}`);
    return data;
  },
  async getByEmail(email: string) {
    const { data } = await apiClient.get<BaseResult<UserResponse>>(`/v1/users/email/${email}`);
    return data;
  },
};
