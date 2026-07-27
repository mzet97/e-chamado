import apiClient from '@/lib/api-client';
import { BaseResult, BaseResultList, CategoryResponse, SubCategoryResponse, CreateCategoryRequest, UpdateCategoryRequest, CreateSubCategoryRequest, UpdateSubCategoryRequest } from '@/types/api';

export const categoryService = {
  async getAll(pageSize = 100) {
    const { data } = await apiClient.get<BaseResultList<CategoryResponse>>('/v1/categories', { params: { PageSize: pageSize } });
    return data;
  },
  async getById(id: string) {
    const { data } = await apiClient.get<BaseResult<CategoryResponse>>(`/v1/categories/${id}`);
    return data;
  },
  async create(req: CreateCategoryRequest) {
    const { data } = await apiClient.post<BaseResult<string>>('/v1/categories', req);
    return data;
  },
  async update(id: string, req: UpdateCategoryRequest) {
    const { data } = await apiClient.put<BaseResult>(`/v1/categories/${id}`, req);
    return data;
  },
  async delete(id: string) {
    const { data } = await apiClient.delete<BaseResult>(`/v1/categories/${id}`);
    return data;
  },
  // SubCategories
  async getSubCategories(categoryId?: string, pageSize = 100) {
    const { data } = await apiClient.get<BaseResultList<SubCategoryResponse>>('/v1/subcategories', { params: { CategoryId: categoryId, PageSize: pageSize } });
    return data;
  },
  async createSubCategory(req: CreateSubCategoryRequest) {
    const { data } = await apiClient.post<BaseResult<string>>('/v1/subcategories', req);
    return data;
  },
  async updateSubCategory(id: string, req: UpdateSubCategoryRequest) {
    const { data } = await apiClient.put<BaseResult>(`/v1/subcategories/${id}`, req);
    return data;
  },
  async deleteSubCategory(id: string) {
    const { data } = await apiClient.delete<BaseResult>(`/v1/subcategories/${id}`);
    return data;
  },
};
