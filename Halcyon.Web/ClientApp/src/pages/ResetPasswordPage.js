import React from 'react';
import { useTranslation } from 'react-i18next';
import { Helmet } from 'react-helmet';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, FormGroup } from 'reactstrap';
import { toast } from 'react-toastify';
import { TextInput, Button, useFetch } from '../components';
import { trackEvent } from '../utils/logger';

export const ResetPasswordPage = ({ match, history }) => {
    const { t } = useTranslation();

    const { refetch: resetPassword } = useFetch({
        method: 'PUT',
        url: '/account/resetpassword',
        manual: true
    });

    const onSubmit = async variables => {
        const result = await resetPassword({
            token: match.params.token,
            emailAddress: variables.emailAddress,
            newPassword: variables.newPassword
        });

        if (result.ok) {
            toast.success(t(`api.codes.${result.code}`));
            trackEvent('password_reset');
            history.push('/login');
        }
    };

    return (
        <Container>
            <Helmet>
                <title>{t('pages.resetPassword.meta.title')}</title>
            </Helmet>

            <h1>{t('pages.resetPassword.title')}</h1>
            <hr />

            <Formik
                initialValues={{
                    emailAddress: '',
                    newPassword: '',
                    confirmNewPassword: ''
                }}
                validationSchema={Yup.object().shape({
                    emailAddress: Yup.string()
                        .label(t('pages.resetPassword.form.emailAddress'))
                        .email()
                        .required(),
                    newPassword: Yup.string()
                        .label(t('pages.resetPassword.form.newPassword'))
                        .min(8)
                        .max(50)
                        .required(),
                    confirmNewPassword: Yup.string()
                        .label(t('pages.resetPassword.form.confirmNewPassword'))
                        .required()
                        .oneOf([Yup.ref('newPassword')])
                })}
                onSubmit={onSubmit}
            >
                {({ isSubmitting }) => (
                    <Form noValidate>
                        <Field
                            name="emailAddress"
                            type="email"
                            label={t('pages.resetPassword.form.emailAddress')}
                            required
                            maxLength={254}
                            autoComplete="username"
                            component={TextInput}
                        />
                        <Field
                            name="newPassword"
                            type="password"
                            label={t('pages.resetPassword.form.newPassword')}
                            required
                            maxLength={50}
                            autoComplete="new-password"
                            component={TextInput}
                        />
                        <Field
                            name="confirmNewPassword"
                            type="password"
                            label={t(
                                'pages.resetPassword.form.confirmNewPassword'
                            )}
                            required
                            maxLength={50}
                            autoComplete="new-password"
                            component={TextInput}
                        />

                        <FormGroup className="text-right">
                            <Button
                                type="submit"
                                color="primary"
                                loading={isSubmitting}
                            >
                                {t('pages.resetPassword.submitButton')}
                            </Button>
                        </FormGroup>
                    </Form>
                )}
            </Formik>
        </Container>
    );
};
