import { createContext, useContext, useState } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { decodeJwt } from 'jose';
import { SessionPayload } from '@/lib/session-types';

type AuthProviderProps = {
    children: React.ReactNode;
};

type AuthProviderState = {
    accessToken: string | null;
    user?: SessionPayload;
    setAuth: (accessToken: string) => void;
    clearAuth: () => void;
};

const STORAGE_KEY = 'accessToken';

const initialState: AuthProviderState = {
    accessToken: null,
    user: undefined,
    setAuth: () => {},
    clearAuth: () => {},
};

const AuthProviderContext = createContext<AuthProviderState>(initialState);

export const AuthProvider = ({ children }: AuthProviderProps) => {
    const [accessToken, setAccessToken] = useState<string | null>(
        localStorage.getItem(STORAGE_KEY)
    );

    const queryClient = useQueryClient();

    function setAuth(accessToken: string) {
        localStorage.setItem(STORAGE_KEY, accessToken);
        setAccessToken(accessToken);
    }

    function clearAuth() {
        localStorage.removeItem(STORAGE_KEY);
        setAccessToken(null);
        queryClient.clear();
    }

    const payload = accessToken
        ? decodeJwt<SessionPayload>(accessToken)
        : undefined;

    const user = payload
        ? {
              ...payload,
              roles:
                  typeof payload.roles === 'string'
                      ? [payload.roles]
                      : payload.roles || [],
          }
        : undefined;

    const value = {
        accessToken,
        user,
        setAuth,
        clearAuth,
    };

    return <AuthProviderContext value={value}>{children}</AuthProviderContext>;
};

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
    const context = useContext(AuthProviderContext);

    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }

    return context;
}
