export const API_BASE_URL =
    import.meta.env.VITE_API_BASE_URL ??
    'http://localhost:5101/api';

const ACCESS_TOKEN_KEY = 'access_token';

export class ApiError extends Error {
    public readonly status: number;

    constructor(
        status: number,
        body: unknown,
    ) {
        super(getErrorMessage(body));

        this.status = status;
        this.name = 'ApiError';
    }
}

function getErrorMessage(body: unknown): string {
    if (!body || typeof body !== 'object') {
        return 'The request failed.';
    }

    if (Array.isArray(body)) {
        const messages = body.flatMap((item) => {
            if (!item || typeof item !== 'object') return [];
            const entry = item as Record<string, unknown>;
            return typeof entry.errorMessage === 'string'
                ? [entry.errorMessage]
                : typeof entry.message === 'string' ? [entry.message] : [];
        });
        return messages.join(' ') || 'The request failed.';
    }

    const response = body as Record<string, unknown>;
    if (typeof response.detail === 'string') return response.detail;
    if (typeof response.title === 'string') return response.title;
    if (response.errors && typeof response.errors === 'object') {
        const messages = Object.values(response.errors)
            .flatMap((value) => Array.isArray(value) ? value : [])
            .filter((value): value is string => typeof value === 'string');
        if (messages.length) return messages.join(' ');
    }
    return 'The request failed.';
}

export function getAccessToken(): string | null {
    return localStorage.getItem(
        ACCESS_TOKEN_KEY,
    );
}

export function setAccessToken(
    token: string,
): void {
    localStorage.setItem(
        ACCESS_TOKEN_KEY,
        token,
    );
}

export function clearAccessToken(): void {
    localStorage.removeItem(
        ACCESS_TOKEN_KEY,
    );

    localStorage.removeItem(
        'current_user',
    );
}

export async function apiRequest<T>(
    path: string,
    options: RequestInit = {},
): Promise<T> {
    let response: Response;

    try {
        const token = getAccessToken();

        response = await fetch(
            `${API_BASE_URL}${path}`,
            {
                ...options,
                headers: {
                    'Content-Type':
                        'application/json',

                    ...(token
                        ? {
                            Authorization:
                                `Bearer ${token}`,
                        }
                        : {}),

                    ...(options.headers ?? {}),
                },
            },
        );
    } catch {
        throw new Error(
            'Unable to connect to the server.',
        );
    }

    const body = await response
        .json()
        .catch(() => undefined);

    if (!response.ok) {
        throw new ApiError(
            response.status,
            body,
        );
    }

    return body as T;
}