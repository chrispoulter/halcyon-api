import React from 'react';
import { useSelector } from 'react-redux';
import { selectCurrentUser } from '../../redux';
import { isAuthorized } from '../../utils/auth';

export const HasPermission = ({ requiredRoles, fallback, children }) => {
    const currentUser = useSelector(selectCurrentUser);

    const isAuthenticated = isAuthorized(currentUser, requiredRoles);
    if (!isAuthenticated) {
        return <>{fallback}</>;
    }

    return <>{children}</>;
};
