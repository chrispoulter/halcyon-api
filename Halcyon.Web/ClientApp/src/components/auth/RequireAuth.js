import React from 'react';
import { Navigate } from 'react-router';
import { useAuth } from '../../contexts';
import { AccessDeniedPage } from '../../pages';
import { isAuthorized } from '../../utils/auth';

export const RequireAuth = ({ children, requiredRoles }) => {
    const { currentUser } = useAuth();

    if (!isAuthorized(currentUser)) {
        return <Navigate to="/login" />;
    }

    if (!isAuthorized(currentUser, requiredRoles)) {
        return <AccessDeniedPage />;
    }

    return children;
};
