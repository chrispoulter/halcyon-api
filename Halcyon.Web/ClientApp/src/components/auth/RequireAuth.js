import React from 'react';
import { Navigate } from 'react-router';
import { useSelector } from 'react-redux';
import { selectCurrentUser } from '../../redux';
import { AccessDeniedPage } from '../../pages';
import { isAuthorized } from '../../utils/auth';

export const RequireAuth = ({ children, requiredRoles }) => {
    const currentUser = useSelector(selectCurrentUser);

    if (!isAuthorized(currentUser)) {
        return <Navigate to="/login" />;
    }

    if (!isAuthorized(currentUser, requiredRoles)) {
        return <AccessDeniedPage />;
    }

    return children;
};
