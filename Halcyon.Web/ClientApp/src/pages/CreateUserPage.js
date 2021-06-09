import React from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Helmet } from 'react-helmet';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, FormGroup } from 'reactstrap';
import { toast } from 'react-toastify';
import {
    TextInput,
    DateInput,
    CheckboxGroupInput,
    Button,
    useFetch
} from '../components';
import { ALL_ROLES } from '../utils/auth';
import { trackEvent } from '../utils/logger';

export const CreateUserPage = ({ history }) => {
    const { t } = useTranslation();

    const { refetch: createUser } = useFetch({
        method: 'POST',
        url: '/user',
        manual: true
    });

    const onSubmit = async variables => {
        const result = await createUser({
            emailAddress: variables.emailAddress,
            password: variables.emailAddress,
            firstName: variables.firstName,
            lastName: variables.lastName,
            dateOfBirth: variables.dateOfBirth,
            roles: variables.roles
        });

        if (result.ok) {
            toast.success(t(`api.codes.${result.code}`));

            trackEvent('user_created', {
                entityId: result.data.id
            });

            history.push('/user');
        }
    };

    return (
        <Container>
            <Helmet>
                <title>{t('pages.createUser.meta.title')}</title>
            </Helmet>

            <h1>
                {t('pages.createUser.title')}
                <br />
                <small className="text-muted">
                    {t('pages.createUser.subtitle')}
                </small>
            </h1>
            <hr />

            <Formik
                initialValues={{
                    emailAddress: '',
                    password: '',
                    confirmPassword: '',
                    firstName: '',
                    lastName: '',
                    dateOfBirth: '',
                    roles: []
                }}
                validationSchema={Yup.object().shape({
                    emailAddress: Yup.string()
                        .label(t('pages.createUser.form.emailAddress'))
                        .max(254)
                        .email()
                        .required(),
                    password: Yup.string()
                        .label(t('pages.createUser.form.password'))
                        .min(8)
                        .max(50)
                        .required(),
                    confirmPassword: Yup.string()
                        .label(t('pages.createUser.form.confirmPassword'))
                        .required()
                        .oneOf([Yup.ref('password')]),
                    firstName: Yup.string()
                        .label(t('pages.createUser.form.firstName'))
                        .max(50)
                        .required(),
                    lastName: Yup.string()
                        .label(t('pages.createUser.form.lastName'))
                        .max(50)
                        .required(),
                    dateOfBirth: Yup.string()
                        .label(t('pages.createUser.form.dateOfBirth'))
                        .required()
                })}
                onSubmit={onSubmit}
            >
                {({ isSubmitting }) => (
                    <Form noValidate>
                        <Field
                            name="emailAddress"
                            type="email"
                            label={t('pages.createUser.form.emailAddress')}
                            required
                            maxLength={254}
                            autoComplete="username"
                            component={TextInput}
                        />

                        <Field
                            name="password"
                            type="password"
                            label={t('pages.createUser.form.password')}
                            required
                            maxLength={50}
                            autoComplete="new-password"
                            component={TextInput}
                        />

                        <Field
                            name="confirmPassword"
                            type="password"
                            label={t('pages.createUser.form.confirmPassword')}
                            required
                            maxLength={50}
                            autoComplete="new-password"
                            component={TextInput}
                        />

                        <Field
                            name="firstName"
                            type="text"
                            label={t('pages.createUser.form.firstName')}
                            required
                            maxLength={50}
                            component={TextInput}
                        />

                        <Field
                            name="lastName"
                            type="text"
                            label={t('pages.createUser.form.lastName')}
                            required
                            maxLength={50}
                            component={TextInput}
                        />

                        <Field
                            name="dateOfBirth"
                            type="date"
                            label={t('pages.createUser.form.dateOfBirth')}
                            required
                            component={DateInput}
                        />

                        <Field
                            name="roles"
                            label={t('pages.createUser.form.roles')}
                            options={ALL_ROLES.map(role => ({
                                value: role,
                                label: t(`api.userRoles.${role}`)
                            }))}
                            component={CheckboxGroupInput}
                        />

                        <FormGroup className="text-right">
                            <Button to="/user" className="mr-1" tag={Link}>
                                {t('pages.createUser.cancelButton')}
                            </Button>
                            <Button
                                type="submit"
                                color="primary"
                                loading={isSubmitting}
                            >
                                {t('pages.createUser.submitButton')}
                            </Button>
                        </FormGroup>
                    </Form>
                )}
            </Formik>
        </Container>
    );
};
