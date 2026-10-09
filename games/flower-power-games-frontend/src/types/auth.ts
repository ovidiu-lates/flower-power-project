export type AuthMode = 'login' | 'register';

export interface LoginRequest {
    emailOrUsername: string;
    password: string;
}
export interface RegisterRequest {
    email: string;
    username: string;
    fullName: string;
    password: string;
    confirmPassword: string;
}

export interface AuthResponse {
    userId: number;
    email: string;
    username: string;
    fullName: string;
    role: string;
    accessToken: string;
    refreshToken: string;
    accessTokenExpiresAtUtc: string;
}

export interface RegisterResponse {
    userId: number;
    email: string;
    username: string;
    fullName: string;
    role: string;
}

export interface ApiErrorResponse {
    title?: string;
    detail?: string;
    status?: number;
}