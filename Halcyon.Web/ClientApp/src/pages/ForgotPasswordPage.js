import React from 'react';
import { useTranslation } from 'react-i18next';
import { Helmet } from 'react-helmet';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, FormGroup } from 'reactstrap';
import { toast } from 'react-toastify';
import { TextInput, Button, useFetch } from '../components';
import { trackEvent } from '../utils/logger';

export const ForgotPasswordPage = ({ history }) => {
    const { t } = useTranslation();

    const { refetch: forgotPassword } = useFetch({
        method: 'PUT',
        url: '/account/forgotpassword',
        manual: true
    });

    const onSubmit = async variables => {
        const result = await forgotPassword({
            emailAddress: variables.emailAddress
        });

        if (result.ok) {
            toast.success(t(`api.codes.${result.code}`));
            trackEvent('password_reminder');
            history.push('/login');
        }
    };

    return (
        <Container>
            <Helmet>
                <title>{t('pages.forgotPassword.meta.title')}</title>
            </Helmet>

            <h1>{t('pages.forgotPassword.title')}</h1>
            <hr />

            <Formik
                initialValues={{
                    emailAddress: ''
                }}
                validationSchema={Yup.object().shape({
                    emailAddress: Yup.string()
                        .label(t('pages.forgotPassword.form.emailAddress'))
                        .email()
                        .required()
                })}
                onSubmit={onSubmit}
            >
                {({ isSubmitting }) => (
                    <Form noValidate>
                        <Field
                            name="emailAddress"
                            type="email"
                            label={t('pages.forgotPassword.form.emailAddress')}
                            required
                            maxLength={254}
                            autoComplete="username"
                            component={TextInput}
                        />

                        <FormGroup className="text-right">
                            <Button
                                type="submit"
                                color="primary"
                                loading={isSubmitting}
                            >
                                {t('pages.forgotPassword.submitButton')}
                            </Button>
                        </FormGroup>
                    </Form>
                )}
            </Formik>
        </Container>
    );
};
