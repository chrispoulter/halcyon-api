import React from 'react';
import { Link } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { Container, Jumbotron, Button } from 'reactstrap';

export const AccessDeniedPage = () => (
    <>
        <Helmet>
            <title>Access Denied</title>
        </Helmet>

        <Jumbotron>
            <Container>
                <h1 className="display-3">Access Denied</h1>
                <hr />
                <p className="lead">
                    Sorry, you do not have access to this resource.
                </p>
                <p className="text-right">
                    <Button to="/" color="primary" size="lg" tag={Link}>
                        Home
                    </Button>
                </p>
            </Container>
        </Jumbotron>
    </>
);
