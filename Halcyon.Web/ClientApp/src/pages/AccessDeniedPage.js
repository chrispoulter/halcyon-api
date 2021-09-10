import React from 'react';
import { Link } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import Button from 'react-bootstrap/Button';
import { Hero } from '../components';

export const AccessDeniedPage = () => (
    <>
        <Helmet>
            <title>Access Denied</title>
        </Helmet>

        <Hero>
            <h1 className="display-3">Access Denied</h1>
            <hr />
            <p className="lead">
                Sorry, you do not have access to this resource.
            </p>
            <p className="text-end">
                <Button to="/" as={Link} variant="primary" size="lg">
                    Home
                </Button>
            </p>
        </Hero>
    </>
);
