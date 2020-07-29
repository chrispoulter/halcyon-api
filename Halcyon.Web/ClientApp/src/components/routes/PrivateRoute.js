import React, { useContext } from 'react';
import { Redirect } from 'react-router';
import { AuthContext } from '../providers/AuthProvider';
import { PublicRoute } from './PublicRoute';
import { AccessDenied } from './AccessDenied';
import { isAuthorized } from '../../utils/auth';

export const PrivateRoute = ({
    component: PrivateComponent,
    requiredRoles,
    ...rest
}) => {
    const { currentUser } = useContext(AuthContext);

    if (!isAuthorized(currentUser)) {
        return <Redirect to="/login" />;
    }

    if (!isAuthorized(currentUser, requiredRoles)) {
        return <PublicRoute title="Access Denied" component={AccessDenied} />;
    }

    return <PublicRoute component={PrivateComponent} {...rest} />;
};
