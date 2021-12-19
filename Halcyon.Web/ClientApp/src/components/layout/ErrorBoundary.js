import React from 'react';
import { Helmet } from 'react-helmet';
import { Link } from 'react-router-dom';
import Button from 'react-bootstrap/Button';
import { Hero } from '../common/Hero';

export class ErrorBoundary extends React.Component {
    state = { hasError: false };

    static getDerivedStateFromError() {
        return { hasError: true };
    }

    componentDidCatch(error, errorInfo) {
        console.error(error, errorInfo);
    }

    render() {
        if (!this.state.hasError) {
            return this.props.children;
        }

        return (
            <>
                <Helmet>
                    <title>Error</title>
                </Helmet>

                <Hero>
                    <h1 className="display-3">Error</h1>
                    <hr />
                    <p className="lead">
                        Sorry, something went wrong. Please try again later.
                    </p>
                    <p className="text-end">
                        <Button to="/" as={Link} variant="primary" size="lg">
                            Home
                        </Button>
                    </p>
                </Hero>
            </>
        );
    }
}
