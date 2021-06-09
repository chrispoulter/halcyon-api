import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import Backend from 'i18next-http-backend';
import LanguageDetector from 'i18next-browser-languagedetector';
import { setLocale } from 'yup';
import { setContext } from './utils/logger';

const initializeValidation = lang => {
    if (!lang) {
        return;
    }

    setContext({ page_locale: lang });

    setLocale({
        mixed: {
            required: i18n.t('validation.required'),
            oneOf: i18n.t('validation.oneOf')
        },
        string: {
            email: i18n.t('validation.email'),
            min: i18n.t('validation.min'),
            max: i18n.t('validation.max')
        }
    });
};

i18n.use(Backend)
    .use(LanguageDetector)
    .use(initReactI18next)
    .init(
        {
            fallbackLng: 'en',
            interpolation: {
                escapeValue: false
            }
        },
        initializeValidation
    );

i18n.on('languageChanged', initializeValidation);
