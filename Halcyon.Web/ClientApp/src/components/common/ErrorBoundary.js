import React from 'react';
import { Helmet } from 'react-helmet';
import { Container, Jumbotron } from 'reactstrap';
import { captureError } from '../../utils/logger';

export class ErrorBoundary extends React.Component {
    state = { hasError: false };

    static getDerivedStateFromError() {
        return { hasError: true };
    }

    componentDidCatch(error, errorInfo) {
        captureError(error, true, errorInfo);
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

                <Jumbotron>
                    <Container>
                        <h1 className="display-3">Error</h1>
                        <hr />
                        <p className="lead">
                            Sorry, something went wrong. Please try again later.
                        </p>
                        <p className="text-right">
                            <a href="/" className="btn btn-lg btn-primary">
                                Home
                            </a>
                        </p>
                    </Container>
                </Jumbotron>
            </>
        );
    }
}
