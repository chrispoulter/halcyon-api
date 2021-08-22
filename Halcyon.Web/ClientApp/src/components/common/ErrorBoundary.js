import React from 'react';
import { Helmet } from 'react-helmet';
import { Link } from 'react-router-dom';
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

                <section className="bg-light pt-5 pb-5 mb-3">
                    <div className="container pt-5 pb-5">
                        <h1 className="display-3">Error</h1>
                        <hr />
                        <p className="lead">
                            Sorry, something went wrong. Please try again later.
                        </p>
                        <p className="text-end">
                            <Link to="/" className="btn btn-primary btn-lg">
                                Home
                            </Link>
                        </p>
                    </div>
                </section>
            </>
        );
    }
}
