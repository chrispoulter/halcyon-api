import React from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Container, Jumbotron, Row, Col, Button } from 'reactstrap';

export const HomePage = () => {
    const { t } = useTranslation();

    return (
        <>
            <Jumbotron>
                <Container>
                    <h1 className="display-3">
                        {t('pages.home.jumbotron.title')}
                    </h1>
                    <hr />
                    <p className="lead">{t('pages.home.jumbotron.lead')}</p>
                    <p className="text-right">
                        <Button
                            to="/register"
                            color="primary"
                            size="lg"
                            tag={Link}
                        >
                            {t('pages.home.jumbotron.registerButton')}
                        </Button>
                    </p>
                </Container>
            </Jumbotron>

            <Container>
                <Row className="justify-content-md-center">
                    <Col lg={4}>
                        <h2>{t('pages.home.panel.title')}</h2>
                        <hr />
                        <p>{t('pages.home.panel.text')}</p>
                    </Col>
                    <Col lg={4}>
                        <h2>{t('pages.home.panel.title')}</h2>
                        <hr />
                        <p>{t('pages.home.panel.text')}</p>
                    </Col>
                    <Col lg={4}>
                        <h2>{t('pages.home.panel.title')}</h2>
                        <hr />
                        <p>{t('pages.home.panel.text')}</p>
                    </Col>
                </Row>
            </Container>
        </>
    );
};
