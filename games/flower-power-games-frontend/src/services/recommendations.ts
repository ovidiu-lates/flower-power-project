import { apiRequest } from '../lib/api';

export type RecommendedGame = { gameId: number; name: string; reason: string };
export type RecommendationResponse = { recommendations: RecommendedGame[] };

export const getRecommendations = (message: string) =>
  apiRequest<RecommendationResponse>('/Recommendations', {
    method: 'POST',
    body: JSON.stringify({ message }),
  });
