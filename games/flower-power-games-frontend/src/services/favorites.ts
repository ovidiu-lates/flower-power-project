import { apiRequest } from '../lib/api';

export type Favorite = { id: number; userId: number; gameId: number };

export const getMyFavorites = () => apiRequest<Favorite[]>('/Favorite/my');
export const getFavorites = () => apiRequest<Favorite[]>('/Favorite');
export const getFavorite = (id: number) => apiRequest<Favorite>(`/Favorite/${id}`);
export const getFavoritesForUser = (userId: number) => apiRequest<Favorite[]>(`/Favorite/user/${userId}`);
export const createFavorite = (gameId: number) =>
  apiRequest<Favorite>('/Favorite', { method: 'POST', body: JSON.stringify({ gameId }) });
export const deleteFavorite = (id: number) =>
  apiRequest<void>(`/Favorite/${id}`, { method: 'DELETE' });
