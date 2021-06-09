import React from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Helmet } from 'react-helmet';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, Alert, FormGroup } from 'reactstrap';
import { toast } from 'react-toastify';
import { Spinner, TextInput, DateInput, Button, useFetch } from '../components';
import { trackEvent } from '../utils/logger';

export const UpdateProfilePage = ({ history }) => {
    const { t } = useTranslation();

    const { loading, data } = useFetch({
        method: 'GET',
        url: '/manage'
    });

    const { refetch: updateProfile } = useFetch({
        method: 'PUT',
        url: '/manage',
        manual: true
    });

    if (loading) {
        return <Spinner />;
    }

    if (!data) {
        return (
            <Alert color="info" className="container p-3 mb-3">
                {t('pages.updateProfile.profileNotFound')}
            </Alert>
        );
    }

    const onSubmit = async variables => {
        const result = await updateProfile({
            emailAddress: variables.emailAddress,
            firstName: variables.firstName,
            lastName: variables.lastName,
            dateOfBirth: variables.dateOfBirth
        });

        if (result.ok) {
            toast.success(t(`api.codes.${result.code}`));
            trackEvent('profile_updated');
            history.push('/my-account');
        }
    };

    return (
        <Container>
            <Helmet>
                <title>{t('pages.updateProfile.meta.title')}</title>
            </Helmet>

            <h1>{t('pages.updateProfile.title')}</h1>
            <hr />

            <Formik
                enableReinitialize={true}
                initialValues={data}
                validationSchema={Yup.object().shape({
                    emailAddress: Yup.string()
                        .label(t('pages.updateProfile.form.emailAddress'))
                        .max(254)
                        .email()
                        .required(),
                    firstName: Yup.string()
                        .label(t('pages.updateProfile.form.firstName'))
                        .max(50)
                        .required(),
                    lastName: Yup.string()
                        .label(t('pages.updateProfile.form.lastName'))
                        .max(50)
                        .required(),
                    dateOfBirth: Yup.string()
                        .label(t('pages.updateProfile.form.dateOfBirth'))
                        .required()
                })}
                onSubmit={onSubmit}
            >
                {({ isSubmitting }) => (
                    <Form noValidate>
                        <Field
                            name="emailAddress"
                            type="email"
                            label={t('pages.updateProfile.form.emailAddress')}
                            required
                            maxLength={254}
                            autoComplete="username"
                            component={TextInput}
                        />

                        <Field
                            name="firstName"
                            type="text"
                            label={t('pages.updateProfile.form.firstName')}
                            required
                            maxLength={50}
                            component={TextInput}
                        />

                        <Field
                            name="lastName"
                            type="text"
                            label={t('pages.updateProfile.form.lastName')}
                            required
                            maxLength={50}
                            component={TextInput}
                        />

                        <Field
                            name="dateOfBirth"
                            type="date"
                            label={t('pages.updateProfile.form.dateOfBirth')}
                            required
                            component={DateInput}
                        />

                        <FormGroup className="text-right">
                            <Button
                                to="/my-account"
                                className="mr-1"
                                tag={Link}
                            >
                                {t('pages.updateProfile.cancelButton')}
                            </Button>
                            <Button
                                type="submit"
                                color="primary"
                                loading={isSubmitting}
                            >
                                {t('pages.updateProfile.submitButton')}
                            </Button>
                        </FormGroup>
                    </Form>
                )}
            </Formik>
        </Container>
    );
};
