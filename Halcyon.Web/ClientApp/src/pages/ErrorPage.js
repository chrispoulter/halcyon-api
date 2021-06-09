import React from 'react';
import { useTranslation } from 'react-i18next';
import { Helmet } from 'react-helmet';
import { Container, Jumbotron } from 'reactstrap';

export const ErrorPage = () => {
    const { t } = useTranslation();

    return (
        <>
            <Helmet>
                <title>{t('pages.error.meta.title')}</title>
            </Helmet>

            <Jumbotron>
                <Container>
                    <h1 className="display-3">
                        {t('pages.error.jumbotron.title')}
                    </h1>
                    <hr />
                    <p className="lead">{t('pages.error.jumbotron.lead')}</p>
                    <p className="text-right">
                        <a href="/" className="btn btn-lg btn-primary">
                            {t('pages.error.jumbotron.homeButton')}
                        </a>
                    </p>
                </Container>
            </Jumbotron>
        </>
    );
};
