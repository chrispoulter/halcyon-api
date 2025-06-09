import { useEffect } from 'react';
import { Navigate } from 'react-router';
import { useAuth } from '@/components/auth-provider';

export function LogoutRedirect() {
    const { clearAuth } = useAuth();

    useEffect(() => {
        clearAuth();
    }, [clearAuth]);

    return <Navigate to="/account/login" />;
}
