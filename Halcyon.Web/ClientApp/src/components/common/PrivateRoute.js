import React from 'react';
import { Redirect, Route } from 'react-router';
import { useAuth } from '../providers';
import { AccessDeniedPage } from '../../pages';
import { isAuthorized } from '../../utils/auth';

export const PrivateRoute = ({
    component: PrivateComponent,
    requiredRoles,
    ...rest
}) => {
    const { currentUser } = useAuth();

    if (!isAuthorized(currentUser)) {
        return <Redirect to="/login" />;
    }

    if (!isAuthorized(currentUser, requiredRoles)) {
        return <Route component={AccessDeniedPage} />;
    }

    return <Route component={PrivateComponent} {...rest} />;
};
