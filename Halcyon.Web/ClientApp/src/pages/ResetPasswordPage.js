import React from 'react';
import { Helmet } from 'react-helmet';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { toast } from 'react-toastify';
import Container from 'react-bootstrap/Container';
import { TextInput, Button, useFetch } from '../components';
import { trackEvent } from '../utils/logger';

export const ResetPasswordPage = ({ match, history }) => {
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
            toast.success(result.message);
            trackEvent('password_reset');
            history.push('/login');
        }
    };

    return (
        <Container>
            <Helmet>
                <title>Reset Password</title>
            </Helmet>

            <h1>Reset Password</h1>
            <hr />

            <Formik
                initialValues={{
                    emailAddress: '',
                    newPassword: '',
                    confirmNewPassword: ''
                }}
                validationSchema={Yup.object().shape({
                    emailAddress: Yup.string()
                        .label('Email Address')
                        .email()
                        .required(),
                    newPassword: Yup.string()
                        .label('New Password')
                        .min(8)
                        .max(50)
                        .required(),
                    confirmNewPassword: Yup.string()
                        .label('Confirm New Password')
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
                            label="Email Address"
                            required
                            maxLength={254}
                            autoComplete="username"
                            component={TextInput}
                        />
                        <Field
                            name="newPassword"
                            type="password"
                            label="New Password"
                            required
                            maxLength={50}
                            autoComplete="new-password"
                            component={TextInput}
                        />
                        <Field
                            name="confirmNewPassword"
                            type="password"
                            label="Confirm New Password"
                            required
                            maxLength={50}
                            autoComplete="new-password"
                            component={TextInput}
                        />

                        <div className="mb-3 text-end">
                            <Button
                                type="submit"
                                variant="primary"
                                loading={isSubmitting}
                            >
                                Submit
                            </Button>
                        </div>
                    </Form>
                )}
            </Formik>
        </Container>
    );
};
