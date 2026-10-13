import { apiRequest } from '../lib/api';

export type ApiUser = {
  id: number;
  roleId: number;
  email: string;
  fullName: string;
  username: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
};
export const getUsers = () => apiRequest<ApiUser[]>('/Users');
export const getUserById = (id: number) => apiRequest<ApiUser>(`/Users/${id}`);
export const updateUser = (id: number, changes: { roleId?: number; isActive?: boolean }) =>
  apiRequest<ApiUser>(`/Users/${id}`, { method: 'PATCH', body: JSON.stringify(changes) });
export const deleteUser = (id: number) => apiRequest<void>(`/Users/${id}`, { method: 'DELETE' });
export const updateMyProfile = (fullName: string) =>
  apiRequest<ApiUser>('/Users/me', { method: 'PUT', body: JSON.stringify({ fullName }) });
export const changeMyPassword = (currentPassword: string, newPassword: string, confirmNewPassword: string) =>
  apiRequest<void>('/Users/me/password', {
    method: 'PUT',
    body: JSON.stringify({ currentPassword, newPassword, confirmNewPassword }),
  });
