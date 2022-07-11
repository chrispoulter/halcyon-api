import React from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { Formik, Form } from 'formik';
import * as Yup from 'yup';
import Container from 'react-bootstrap/Container';
import { TextInput, Button } from '../components';
import { useToast } from '../contexts';
import { useResetPassword } from '../services';

export const ResetPasswordPage = () => {
    const navigate = useNavigate();

    const { token } = useParams();

    const toast = useToast();

    const { request: resetPassword } = useResetPassword();

    const onSubmit = async variables => {
        const result = await resetPassword({
            token,
            ...variables
        });

        if (result.ok) {
            toast.success(result.message);
            navigate('/login');
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
                validationSchema={Yup.object({
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
                        <TextInput
                            name="emailAddress"
                            type="email"
                            label="Email Address"
                            required
                            maxLength={254}
                            autoComplete="username"
                        />
                        <TextInput
                            name="newPassword"
                            type="password"
                            label="New Password"
                            required
                            maxLength={50}
                            autoComplete="new-password"
                        />
                        <TextInput
                            name="confirmNewPassword"
                            type="password"
                            label="Confirm New Password"
                            required
                            maxLength={50}
                            autoComplete="new-password"
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
