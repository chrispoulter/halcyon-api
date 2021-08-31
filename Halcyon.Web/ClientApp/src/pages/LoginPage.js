import React from 'react';
import { Link } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import Container from 'react-bootstrap/Container';
import {
    TextInput,
    CheckboxInput,
    Button,
    useAuth,
    useFetch
} from '../components';
import { trackEvent } from '../utils/logger';

export const LoginPage = ({ history }) => {
    const { setToken } = useAuth();

    const { refetch: generateToken } = useFetch({
        method: 'POST',
        url: '/token',
        manual: true
    });

    const onSubmit = async variables => {
        const result = await generateToken({
            grantType: 'PASSWORD',
            emailAddress: variables.emailAddress,
            password: variables.password
        });

        if (result.ok) {
            setToken(result.data.accessToken, variables.rememberMe);

            trackEvent('login');
            history.push('/');
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
                validationSchema={Yup.object().shape({
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
                            name="password"
                            type="password"
                            label="Password"
                            required
                            maxLength={50}
                            autoComplete="current-password"
                            component={TextInput}
                        />

                        <Field
                            name="rememberMe"
                            label="Remember my password on this computer"
                            component={CheckboxInput}
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
