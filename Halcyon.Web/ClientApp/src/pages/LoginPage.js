import React, { useContext } from 'react';
import { Link } from 'react-router-dom';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, FormGroup } from 'reactstrap';
import {
    TextInput,
    CheckboxInput,
    Button,
    AuthContext,
    useFetch
} from '../components';

const initialValues = {
    emailAddress: '',
    password: '',
    rememberMe: true
};

const validationSchema = Yup.object().shape({
    emailAddress: Yup.string().label('Email Address').email().required(),
    password: Yup.string().label('Password').required()
});

export const LoginPage = ({ history }) => {
    const { setToken } = useContext(AuthContext);

    const { refetch: generateToken } = useFetch({
        method: 'POST',
        url: '/token',
        manual: true
    });

    const onSubmit = async data => {
        const result = await generateToken({
            grantType: 'PASSWORD',
            emailAddress: data.emailAddress,
            password: data.password
        });

        if (result.ok) {
            setToken(result.data.accessToken, data.rememberMe);
            history.push('/');
        }
    };

    return (
        <Container>
            <h1>Login</h1>
            <hr />

            <Formik
                initialValues={initialValues}
                validationSchema={validationSchema}
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

                        <FormGroup className="text-right">
                            <Button
                                type="submit"
                                color="primary"
                                loading={isSubmitting}
                            >
                                Submit
                            </Button>
                        </FormGroup>
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
