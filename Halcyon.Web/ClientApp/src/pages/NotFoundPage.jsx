import React from 'react';
import { Link } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import Button from 'react-bootstrap/Button';
import { Hero } from '../components';

export const NotFoundPage = () => (
    <>
        <Helmet>
            <title>Page Not Found</title>
        </Helmet>

        <Hero>
            <h1 className="display-3">Page Not Found</h1>
            <hr />
            <p className="lead">
                Sorry, the Page you were looking for could not be found.
            </p>
            <p className="text-end">
                <Button to="/" as={Link} variant="primary" size="lg">
                    Home
                </Button>
            </p>
        </Hero>
    </>
);
