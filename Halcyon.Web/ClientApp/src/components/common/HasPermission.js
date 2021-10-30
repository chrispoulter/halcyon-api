import React from 'react';
import { useAuth } from '../providers';
import { isAuthorized } from '../../utils/auth';

export const HasPermission = ({ requiredRoles, fallback, children }) => {
    const { currentUser } = useAuth();

    const isAuthenticated = isAuthorized(currentUser, requiredRoles);
    if (!isAuthenticated) {
        return <>{fallback}</>;
    }

    return <>{children}</>;
};
