import apiClient from '@/lib/api-client';
import { BaseResult, NLToGridifyResult } from '@/types/api';

export const nlQueryService = {
  async convertToGridify(entityName: string, query: string, provider?: string) {
    const { data } = await apiClient.post<BaseResult<NLToGridifyResult>>('/v1/ai/nl-to-gridify', {
      entityName,
      query,
      provider,
    });
    return data;
  },

  async convertBatch(entityName: string, queries: string[], provider?: string) {
    const { data } = await apiClient.post<BaseResult>('/v1/ai/nl-to-gridify-batch', {
      entityName,
      queries,
      provider,
    });
    return data;
  },
};
