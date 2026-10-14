import { ApiError, apiRequest } from '../lib/api';

export type AiUsage = {
  id: number;
  userId: number;
  totalRequests: number;
  totalPromptUsed: number;
  totalAvailablePrompt: number;
};

export async function getAiUsageForUser(userId: number): Promise<AiUsage | null> {
  try {
    return await apiRequest<AiUsage>(`/AiUsage/user/${userId}`);
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) return null;
    throw error;
  }
}

export const getAiUsageById = (id: number) =>
  apiRequest<AiUsage>(`/AiUsage/${id}`);

export const updateAiUsageLimit = (id: number, totalAvailablePrompt: number) =>
  apiRequest<AiUsage>(`/AiUsage/admin/${id}`, {
    method: 'PUT',
    body: JSON.stringify({ totalAvailablePrompt }),
  });
