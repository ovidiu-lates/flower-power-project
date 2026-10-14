import { apiRequest } from '../lib/api';

export type Rating = {
  id: number;
  userId: number;
  gameId: number;
  score: number;
  review: string | null;
  createdAt: string;
  updatedAt?: string;
};

export const getRatings = () => apiRequest<Rating[]>('/Rating');
export const getMyRatings = () => apiRequest<Rating[]>('/Rating/my');
export const getRatingsForGame = (gameId: number) => apiRequest<Rating[]>(`/Rating/game/${gameId}`);
export const createRating = (gameId: number, score: number, review: string) =>
  apiRequest<Rating>('/Rating', {
    method: 'POST',
    body: JSON.stringify({ gameId, score, review }),
  });
export const updateRating = (id: number, rating: Rating) =>
  apiRequest<Rating>(`/Rating/${id}`, { method: 'PUT', body: JSON.stringify(rating) });
export const deleteRating = (id: number) =>
  apiRequest<void>(`/Rating/${id}`, { method: 'DELETE' });
