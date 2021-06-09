import React, { useContext } from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Helmet } from 'react-helmet';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, FormGroup } from 'reactstrap';
import {
    TextInput,
    DateInput,
    Button,
    AuthContext,
    useFetch
} from '../components';
import { trackEvent } from '../utils/logger';

export const RegisterPage = ({ history }) => {
    const { t } = useTranslation();

    const { setToken } = useContext(AuthContext);

    const { refetch: register } = useFetch({
        method: 'POST',
        url: '/account/register',
        manual: true
    });

    const { refetch: generateToken } = useFetch({
        method: 'POST',
        url: '/token',
        manual: true
    });

    const onSubmit = async variables => {
        let result = await register({
            emailAddress: variables.emailAddress,
            password: variables.password,
            firstName: variables.firstName,
            lastName: variables.lastName,
            dateOfBirth: variables.dateOfBirth
        });

        if (result.ok) {
            trackEvent('sign_up', {
                entityId: result.data.id
            });

            result = await generateToken({
                grantType: 'PASSWORD',
                emailAddress: variables.emailAddress,
                password: variables.password
            });

            if (result.ok) {
                setToken(result.data.accessToken);
                history.push('/');
            }
        }
    };

    return (
        <Container>
            <Helmet>
                <title>{t('pages.register.meta.title')}</title>
            </Helmet>

            <h1>{t('pages.register.title')}</h1>
            <hr />

            <Formik
                initialValues={{
                    emailAddress: '',
                    password: '',
                    confirmPassword: '',
                    firstName: '',
                    lastName: '',
                    dateOfBirth: ''
                }}
                validationSchema={Yup.object().shape({
                    emailAddress: Yup.string()
                        .label(t('pages.register.form.emailAddress'))
                        .max(254)
                        .email()
                        .required(),
                    password: Yup.string()
                        .label(t('pages.register.form.password'))
                        .min(8)
                        .max(50)
                        .required(),
                    confirmPassword: Yup.string()
                        .label(t('pages.register.form.confirmPassword'))
                        .required()
                        .oneOf([Yup.ref('password')]),
                    firstName: Yup.string()
                        .label(t('pages.register.form.firstName'))
                        .max(50)
                        .required(),
                    lastName: Yup.string()
                        .label(t('pages.register.form.lastName'))
                        .max(50)
                        .required(),
                    dateOfBirth: Yup.string()
                        .label(t('pages.register.form.dateOfBirth'))
                        .required()
                })}
                onSubmit={onSubmit}
            >
                {({ isSubmitting }) => (
                    <Form noValidate>
                        <Field
                            name="emailAddress"
                            type="email"
                            label={t('pages.register.form.emailAddress')}
                            required
                            maxLength={254}
                            autoComplete="username"
                            component={TextInput}
                        />

                        <Field
                            name="password"
                            type="password"
                            label={t('pages.register.form.password')}
                            required
                            maxLength={50}
                            autoComplete="new-password"
                            component={TextInput}
                        />

                        <Field
                            name="confirmPassword"
                            type="password"
                            label={t('pages.register.form.confirmPassword')}
                            required
                            maxLength={50}
                            autoComplete="new-password"
                            component={TextInput}
                        />

                        <Field
                            name="firstName"
                            type="text"
                            label={t('pages.register.form.firstName')}
                            required
                            maxLength={50}
                            component={TextInput}
                        />

                        <Field
                            name="lastName"
                            type="text"
                            label={t('pages.register.form.lastName')}
                            required
                            maxLength={50}
                            component={TextInput}
                        />

                        <Field
                            name="dateOfBirth"
                            type="date"
                            label={t('pages.register.form.dateOfBirth')}
                            required
                            component={DateInput}
                        />

                        <FormGroup className="text-right">
                            <Button
                                type="submit"
                                color="primary"
                                loading={isSubmitting}
                            >
                                {t('pages.register.submitButton')}
                            </Button>
                        </FormGroup>
                    </Form>
                )}
            </Formik>

            <p>
                {t('pages.register.loginPrompt')}{' '}
                <Link to="/login">{t('pages.register.loginLink')}</Link>
            </p>
        </Container>
    );
};
