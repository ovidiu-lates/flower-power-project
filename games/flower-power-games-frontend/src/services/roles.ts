import { apiRequest } from '../lib/api';

export type Role = { roleId: number; name: string };

export const getRoles = () => apiRequest<Role[]>('/Roles');
export const getRoleById = (id: number) => apiRequest<Role>(`/Roles/${id}`);
export const createRole = (name: string) =>
  apiRequest<Role>('/Roles', { method: 'POST', body: JSON.stringify({ name }) });
export const updateRole = (id: number, name: string) =>
  apiRequest<Role>(`/Roles/${id}`, { method: 'PUT', body: JSON.stringify({ roleId: id, name }) });
export const deleteRole = (id: number) => apiRequest<void>(`/Roles/${id}`, { method: 'DELETE' });
