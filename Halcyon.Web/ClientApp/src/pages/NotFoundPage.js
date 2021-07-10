import React from 'react';
import { Link } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { Container, Jumbotron, Button } from 'reactstrap';

export const NotFoundPage = () => (
    <>
        <Helmet>
            <title>Page Not Found</title>
        </Helmet>

        <Jumbotron>
            <Container>
                <h1 className="display-3">Page Not Found</h1>
                <hr />
                <p className="lead">
                    Sorry, the Page you were looking for could not be found.
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
