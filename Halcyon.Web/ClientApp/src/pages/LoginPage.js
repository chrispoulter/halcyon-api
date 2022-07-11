import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { Formik, Form } from 'formik';
import * as Yup from 'yup';
import Container from 'react-bootstrap/Container';
import { TextInput, CheckboxInput, Button } from '../components';
import { useAuth } from '../contexts';
import { useCreateToken } from '../services';

export const LoginPage = () => {
    const navigate = useNavigate();

    const { setToken } = useAuth();

    const { request: createToken } = useCreateToken();

    const onSubmit = async variables => {
        const result = await createToken(variables);

        if (result.ok) {
            setToken(result.data.accessToken, variables.rememberMe);
            navigate('/');
        }
    };

    return (
        <Container>
            <Helmet>
                <title>Login</title>
            </Helmet>

            <h1>Login</h1>
            <hr />

            <Formik
                initialValues={{
                    emailAddress: '',
                    password: '',
                    rememberMe: true
                }}
                validationSchema={Yup.object({
                    emailAddress: Yup.string()
                        .label('Email Address')
                        .email()
                        .required(),
                    password: Yup.string().label('Password').required()
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
                            name="password"
                            type="password"
                            label="Password"
                            required
                            maxLength={50}
                            autoComplete="current-password"
                        />

                        <CheckboxInput
                            name="rememberMe"
                            label="Remember my password on this computer"
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

            <p>
                Not already a member? <Link to="/register">Register now</Link>
            </p>
            <p>
                Forgotten your password?{' '}
                <Link to="/forgot-password">Request reset</Link>
            </p>
        </Container>
    );
};
