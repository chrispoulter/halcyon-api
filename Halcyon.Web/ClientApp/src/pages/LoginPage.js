import React, { useContext } from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Helmet } from 'react-helmet';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, FormGroup } from 'reactstrap';
import {
    TextInput,
    CheckboxInput,
    Button,
    AuthContext,
    useFetch
} from '../components';
import { trackEvent } from '../utils/logger';

export const LoginPage = ({ history }) => {
    const { t } = useTranslation();

    const { setToken } = useContext(AuthContext);

    const { refetch: generateToken } = useFetch({
        method: 'POST',
        url: '/token',
        manual: true
    });

    const onSubmit = async variables => {
        const result = await generateToken({
            grantType: 'PASSWORD',
            emailAddress: variables.emailAddress,
            password: variables.password
        });

        if (result.ok) {
            setToken(result.data.accessToken, variables.rememberMe);

            trackEvent('login');
            history.push('/');
        }
    };

    return (
        <Container>
            <Helmet>
                <title>{t('pages.login.meta.title')}</title>
            </Helmet>

            <h1>{t('pages.login.title')}</h1>
            <hr />

            <Formik
                initialValues={{
                    emailAddress: '',
                    password: '',
                    rememberMe: true
                }}
                validationSchema={Yup.object().shape({
                    emailAddress: Yup.string()
                        .label(t('pages.login.form.emailAddress'))
                        .email()
                        .required(),
                    password: Yup.string()
                        .label(t('pages.login.form.password'))
                        .required()
                })}
                onSubmit={onSubmit}
            >
                {({ isSubmitting }) => (
                    <Form noValidate>
                        <Field
                            name="emailAddress"
                            type="email"
                            label={t('pages.login.form.emailAddress')}
                            required
                            maxLength={254}
                            autoComplete="username"
                            component={TextInput}
                        />

                        <Field
                            name="password"
                            type="password"
                            label={t('pages.login.form.password')}
                            required
                            maxLength={50}
                            autoComplete="current-password"
                            component={TextInput}
                        />

                        <Field
                            name="rememberMe"
                            label={t('pages.login.form.rememberMe')}
                            component={CheckboxInput}
                        />

                        <FormGroup className="text-right">
                            <Button
                                type="submit"
                                color="primary"
                                loading={isSubmitting}
                            >
                                {t('pages.login.submitButton')}
                            </Button>
                        </FormGroup>
                    </Form>
                )}
            </Formik>

            <p>
                {t('pages.login.registerPrompt')}{' '}
                <Link to="/register">{t('pages.login.registerLink')}</Link>
            </p>
            <p>
                {t('pages.login.forgotPasswordPrompt')}{' '}
                <Link to="/forgot-password">
                    {t('pages.login.forgotPasswordLink')}
                </Link>
            </p>
        </Container>
    );
};
