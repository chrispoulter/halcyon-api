import React from 'react';
import { Helmet } from 'react-helmet';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { toast } from 'react-toastify';
import Container from 'react-bootstrap/Container';
import { TextInput, Button, useFetch } from '../components';
import { trackEvent } from '../utils/logger';

export const ForgotPasswordPage = ({ history }) => {
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
            toast.success(result.message);
            trackEvent('password_reminder');
            history.push('/login');
        }
    };

    return (
        <Container>
            <Helmet>
                <title>Forgot Password</title>
            </Helmet>

            <h1>Forgot Password</h1>
            <hr />

            <Formik
                initialValues={{
                    emailAddress: ''
                }}
                validationSchema={Yup.object().shape({
                    emailAddress: Yup.string()
                        .label('Email Address')
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
                            label="Email Address"
                            required
                            maxLength={254}
                            autoComplete="username"
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
