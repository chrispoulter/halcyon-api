import { config } from '@/lib/config';

export class ApiClientError extends Error {
    constructor(
        message: string,
        public status?: number
    ) {
        super(message);
        this.name = 'ApiClientError';
    }
}

class ApiClient {
    private baseUrl: string;

    constructor(baseUrl: string) {
        this.baseUrl = baseUrl;
    }

    private async fetch<Data>(
        path: string,
        method: string,
        params?: Record<string, string | number | boolean>,
        body?: Record<string, unknown>,
        headers: Record<string, string> = {}
    ): Promise<Data> {
        const url = new URL(path, this.baseUrl);

        if (params) {
            for (const [key, value] of Object.entries(params)) {
                url.searchParams.append(key, String(value));
            }
        }

        const response = await fetch(url, {
            method,
            headers: {
                'Content-Type': 'application/json',
                ...headers,
            },
            body: body ? JSON.stringify(body) : undefined,
        });

        // wait for 3 seconds
        // await new Promise((resolve) => setTimeout(resolve, 3000));

        const contentType = response.headers.get('content-type') || '';

        if (!response.ok) {
            if (contentType.includes('application/problem+json')) {
                const problem = await response.json();
                throw new ApiClientError(problem.title, problem.status);
            }

            throw new ApiClientError(
                `HTTP ${response.status} ${response.statusText}`,
                response.status
            );
        }

        if (contentType.includes('application/json')) {
            return await response.json();
        }

        return (await response.text()) as Data;
    }

    get<Data>(
        path: string,
        params: Record<string, string | number | boolean> = {},
        headers: Record<string, string> = {}
    ): Promise<Data> {
        return this.fetch<Data>(path, 'GET', params, undefined, headers);
    }

    post<Data>(
        path: string,
        body: Record<string, unknown>,
        headers: Record<string, string> = {}
    ): Promise<Data> {
        return this.fetch<Data>(path, 'POST', undefined, body, headers);
    }

    put<Data>(
        path: string,
        body: Record<string, unknown>,
        headers: Record<string, string> = {}
    ): Promise<Data> {
        return this.fetch<Data>(path, 'PUT', undefined, body, headers);
    }

    delete<Data>(
        path: string,
        body: Record<string, unknown>,
        headers: Record<string, string> = {}
    ): Promise<Data> {
        return this.fetch<Data>(path, 'DELETE', undefined, body, headers);
    }
}

export const apiClient = new ApiClient(config.VITE_API_URL);
