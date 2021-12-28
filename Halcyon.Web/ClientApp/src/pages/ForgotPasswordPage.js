import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import Container from 'react-bootstrap/Container';
import { TextInput, Button } from '../components';
import { useToast } from '../contexts';
import { useForgotPassword } from '../services';

export const ForgotPasswordPage = () => {
    const navigate = useNavigate();

    const toast = useToast();

    const { refetch: forgotPassword } = useForgotPassword();

    const onSubmit = async variables => {
        const result = await forgotPassword(variables);

        if (result.ok) {
            toast.success(result.message);
            navigate('/login');
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
                validationSchema={Yup.object({
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
