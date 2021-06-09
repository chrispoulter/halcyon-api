import React from 'react';
import { useTranslation } from 'react-i18next';
import { Helmet } from 'react-helmet';

export const Meta = () => {
    const { t, i18n } = useTranslation();

    return (
        <Helmet
            defaultTitle={t('meta.title')}
            titleTemplate={t('meta.template')}
        >
            <html lang={i18n.language} />
            <meta name="description" content={t('meta.description')} />
            <meta name="keywords" content={t('meta.keywords')} />
        </Helmet>
    );
};
