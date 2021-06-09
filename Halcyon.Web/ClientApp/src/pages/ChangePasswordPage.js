import React from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Helmet } from 'react-helmet';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, FormGroup } from 'reactstrap';
import { toast } from 'react-toastify';
import { TextInput, Button, useFetch } from '../components';
import { trackEvent } from '../utils/logger';

export const ChangePasswordPage = ({ history }) => {
    const { t } = useTranslation();

    const { refetch: changePassword } = useFetch({
        method: 'PUT',
        url: '/manage/changepassword',
        manual: true
    });

    const onSubmit = async variables => {
        const result = await changePassword({
            currentPassword: variables.currentPassword,
            newPassword: variables.newPassword
        });

        if (result.ok) {
            toast.success(t(`api.codes.${result.code}`));
            trackEvent('password_changed');
            history.push('/my-account');
        }
    };

    return (
        <Container>
            <Helmet>
                <title>{t('pages.changePassword.meta.title')}</title>
            </Helmet>

            <h1>{t('pages.changePassword.title')}</h1>
            <hr />

            <Formik
                initialValues={{
                    currentPassword: '',
                    newPassword: '',
                    confirmNewPassword: ''
                }}
                validationSchema={Yup.object().shape({
                    currentPassword: Yup.string()
                        .label(t('pages.changePassword.form.currentPassword'))
                        .required(),
                    newPassword: Yup.string()
                        .label(t('pages.changePassword.form.newPassword'))
                        .min(8)
                        .max(50)
                        .required(),
                    confirmNewPassword: Yup.string()
                        .label(
                            t('pages.changePassword.form.confirmNewPassword')
                        )
                        .required()
                        .oneOf([Yup.ref('newPassword')])
                })}
                onSubmit={onSubmit}
            >
                {({ isSubmitting }) => (
                    <Form noValidate>
                        <Field
                            name="currentPassword"
                            type="password"
                            label={t(
                                'pages.changePassword.form.currentPassword'
                            )}
                            required
                            maxLength={50}
                            autoComplete="current-password"
                            component={TextInput}
                        />
                        <Field
                            name="newPassword"
                            type="password"
                            label={t('pages.changePassword.form.newPassword')}
                            required
                            maxLength={50}
                            autoComplete="new-password"
                            component={TextInput}
                        />
                        <Field
                            name="confirmNewPassword"
                            type="password"
                            label={t(
                                'pages.changePassword.form.confirmNewPassword'
                            )}
                            required
                            maxLength={50}
                            autoComplete="new-password"
                            component={TextInput}
                        />

                        <FormGroup className="text-right">
                            <Button
                                to="/my-account"
                                className="mr-1"
                                tag={Link}
                            >
                                {t('pages.changePassword.cancelButton')}
                            </Button>
                            <Button
                                type="submit"
                                color="primary"
                                loading={isSubmitting}
                            >
                                {t('pages.changePassword.submitButton')}
                            </Button>
                        </FormGroup>
                    </Form>
                )}
            </Formik>

            <p>
                {t('pages.changePassword.forgotPasswordPrompt')}{' '}
                <Link to="/forgot-password">
                    {t('pages.changePassword.forgotPasswordLink')}
                </Link>
            </p>
        </Container>
    );
};
