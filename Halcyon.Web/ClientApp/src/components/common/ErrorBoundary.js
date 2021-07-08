import React from 'react';
import { withTranslation } from 'react-i18next';
import { Helmet } from 'react-helmet';
import { Container, Jumbotron } from 'reactstrap';
import { captureError } from '../../utils/logger';

export class ErrorBoundaryComponent extends React.Component {
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

        const { t } = this.props;

        return (
            <>
                <Helmet>
                    <title>{t('components.errorBoundary.meta.title')}</title>
                </Helmet>

                <Jumbotron>
                    <Container>
                        <h1 className="display-3">
                            {t('components.errorBoundary.jumbotron.title')}
                        </h1>
                        <hr />
                        <p className="lead">
                            {t('components.errorBoundary.jumbotron.lead')}yarn
                        </p>
                        <p className="text-right">
                            <a href="/" className="btn btn-lg btn-primary">
                                {t(
                                    'components.errorBoundary.jumbotron.homeButton'
                                )}
                            </a>
                        </p>
                    </Container>
                </Jumbotron>
            </>
        );
    }
}

export const ErrorBoundary = withTranslation()(ErrorBoundaryComponent);
