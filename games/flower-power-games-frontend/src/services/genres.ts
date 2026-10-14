import { apiRequest } from '../lib/api';

export type Genre = { id: number; name: string };

export const getGenres = () => apiRequest<Genre[]>('/Genres');

export const createGenre = (name: string) =>
  apiRequest<Genre>('/Genres', { method: 'POST', body: JSON.stringify({ name }) });

export const updateGenre = (id: number, name: string) =>
  apiRequest<Genre>(`/Genres/${id}`, { method: 'PUT', body: JSON.stringify({ id, name }) });

export const deleteGenre = (id: number) =>
  apiRequest<void>(`/Genres/${id}`, { method: 'DELETE' });
