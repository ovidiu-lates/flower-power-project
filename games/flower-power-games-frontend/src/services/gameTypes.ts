import { apiRequest } from '../lib/api';

export type GameType = { id: number; name: string };

export const getGameTypes = () => apiRequest<GameType[]>('/GameTypes');

export const createGameType = (name: string) =>
  apiRequest<GameType>('/GameTypes', { method: 'POST', body: JSON.stringify({ name }) });

export const updateGameType = (id: number, name: string) =>
  apiRequest<GameType>(`/GameTypes/${id}`, { method: 'PUT', body: JSON.stringify({ id, name }) });

export const deleteGameType = (id: number) =>
  apiRequest<void>(`/GameTypes/${id}`, { method: 'DELETE' });
