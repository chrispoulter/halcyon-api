import React from 'react';
import { Route } from 'react-router';

const siteName = 'Halcyon';
const seperator = ' // ';

export const PublicRoute = ({ component: PublicComponent, title, ...rest }) => {
    document.title = title ? `${title} ${seperator} ${siteName}` : siteName;
    return <Route component={PublicComponent} {...rest} />;
};
