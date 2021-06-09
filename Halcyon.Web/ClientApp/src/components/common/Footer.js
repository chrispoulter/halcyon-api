import React from 'react';
import { useTranslation } from 'react-i18next';
import { Container, Button } from 'reactstrap';

const currentYear = new Date().getFullYear();

export const Footer = () => {
    const { t, i18n } = useTranslation();

    const languages = t('languages', { returnObjects: true });

    const changeLanguage = lng => {
        i18n.changeLanguage(lng);
    };

    return (
        <footer>
            <Container className="pt-3">
                <hr />
                <div className="d-flex justify-content-between">
                    <p>
                        &copy;{' '}
                        <a href="https://www.chrispoulter.com">Chris Poulter</a>{' '}
                        {currentYear}
                    </p>
                    <p>
                        {Object.entries(languages).map(([key, value]) => (
                            <Button
                                key={key}
                                color="link"
                                className="p-0 ml-2"
                                onClick={() => changeLanguage(key)}
                            >
                                {value}
                            </Button>
                        ))}
                    </p>
                </div>
            </Container>
        </footer>
    );
};
