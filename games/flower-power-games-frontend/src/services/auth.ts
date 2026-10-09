import {
    apiRequest,
    clearAccessToken,
    setAccessToken,
} from '../lib/api';

import type {
    AuthResponse,
    RegisterRequest,
    LoginRequest
} from '../types/auth';

import type { User } from '../types/user';
function mapAuthResponseToUser(response: AuthResponse): User {
    return {
        id: response.userId,
        email: response.email,
        username: response.username,
        fullName: response.fullName,
        role: response.role,
    };
}

export function login(
    request: LoginRequest,
) {
    return apiRequest<AuthResponse>(
        '/auth/login',
        {
            method: 'POST',
            body: JSON.stringify(request),
        },
    );
}

export function register(request: RegisterRequest,) {
    return apiRequest(
        '/auth/register',
        {
            method: 'POST',
            body: JSON.stringify(request),
        },
    );
}

export function saveSession(
    response: AuthResponse,
): void {
    setAccessToken(response.accessToken);
    saveCurrentUser(mapAuthResponseToUser(response));
}

export function saveCurrentUser(
    user: User,
): void {
    localStorage.setItem(
        'current_user',
        JSON.stringify(user),
    );
}

export function getStoredCurrentUser(): User | null {
    const value =
        localStorage.getItem('current_user');

    if (!value) {
        return null;
    }

    return JSON.parse(value) as User;
}

export async function refresh(
    refreshToken: string,
) {
    return apiRequest<AuthResponse>(
        '/auth/refresh',
        {
            method: 'POST',
            body: JSON.stringify({
                refreshToken,
            }),
        },
    );
}

export async function logout(
    refreshToken: string,
): Promise<void> {
    await apiRequest<void>(
        '/auth/logout',
        {
            method: 'POST',
            body: JSON.stringify({
                refreshToken,
            }),
        },
    );

    clearAccessToken();
}