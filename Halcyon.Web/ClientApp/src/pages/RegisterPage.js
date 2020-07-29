import React, { useContext } from 'react';
import { Link } from 'react-router-dom';
import { useMutation } from '@apollo/react-hooks';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, FormGroup } from 'reactstrap';
import { REGISTER, GENERATE_TOKEN } from '../graphql';
import { TextInput, DateInput, Button, AuthContext } from '../components';

const initialValues = {
    emailAddress: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
    dateOfBirth: ''
};

const validationSchema = Yup.object().shape({
    emailAddress: Yup.string()
        .label('Email Address')
        .max(254)
        .email()
        .required(),
    password: Yup.string().label('Password').min(8).max(50).required(),
    confirmPassword: Yup.string()
        .label('Confirm Password')
        .required()
        .oneOf(
            [Yup.ref('password')],
            d => `The "${d.label}" field does not match.`
        ),
    firstName: Yup.string().label('First Name').max(50).required(),
    lastName: Yup.string().label('Last Name').max(50).required(),
    dateOfBirth: Yup.string().label('Date of Birth').required()
});

export const RegisterPage = ({ history }) => {
    const { setToken } = useContext(AuthContext);

    const [register] = useMutation(REGISTER);

    const [generateToken] = useMutation(GENERATE_TOKEN);

    const onSubmit = async variables => {
        try {
            await register({ mutation: REGISTER, variables });

            const result = await generateToken({
                variables: { grantType: 'PASSWORD', ...variables }
            });

            setToken(result.data.generateToken.accessToken);
            history.push('/');
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <Container>
            <h1>Register</h1>
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
                            autoComplete="new-password"
                            component={TextInput}
                        />

                        <Field
                            name="confirmPassword"
                            type="password"
                            label="Confirm Password"
                            required
                            maxLength={50}
                            autoComplete="new-password"
                            component={TextInput}
                        />

                        <Field
                            name="firstName"
                            type="text"
                            label="First Name"
                            required
                            maxLength={50}
                            component={TextInput}
                        />

                        <Field
                            name="lastName"
                            type="text"
                            label="Last Name"
                            required
                            maxLength={50}
                            component={TextInput}
                        />

                        <Field
                            name="dateOfBirth"
                            type="date"
                            label="Date Of Birth"
                            required
                            component={DateInput}
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
                Already have an account? <Link to="/login">Log in now</Link>
            </p>
        </Container>
    );
};
