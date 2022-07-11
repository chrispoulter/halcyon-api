import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { Formik, Form } from 'formik';
import * as Yup from 'yup';
import Container from 'react-bootstrap/Container';
import {
    TextInput,
    DateInput,
    CheckboxGroupInput,
    Button
} from '../components';
import { useToast } from '../contexts';
import { useCreateUser } from '../services';
import { ALL_ROLES } from '../utils/auth';

export const CreateUserPage = () => {
    const navigate = useNavigate();

    const toast = useToast();

    const { request: createUser } = useCreateUser();

    const onSubmit = async variables => {
        const result = await createUser(variables);

        if (result.ok) {
            toast.success(result.message);
            navigate('/user');
        }
    };

    return (
        <Container>
            <Helmet>
                <title>Create User</title>
            </Helmet>

            <h1>
                User
                <br />
                <small className="text-muted">Create</small>
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
                validationSchema={Yup.object({
                    emailAddress: Yup.string()
                        .label('Email Address')
                        .max(254)
                        .email()
                        .required(),
                    password: Yup.string()
                        .label('Password')
                        .min(8)
                        .max(50)
                        .required(),
                    confirmPassword: Yup.string()
                        .label('Confirm Password')
                        .required()
                        .oneOf([Yup.ref('password')]),
                    firstName: Yup.string()
                        .label('First Name')
                        .max(50)
                        .required(),
                    lastName: Yup.string()
                        .label('Last Name')
                        .max(50)
                        .required(),
                    dateOfBirth: Yup.string().label('Date Of Birth').required()
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
                            autoComplete="new-password"
                        />

                        <TextInput
                            name="confirmPassword"
                            type="password"
                            label="Confirm Password"
                            required
                            maxLength={50}
                            autoComplete="new-password"
                        />

                        <TextInput
                            name="firstName"
                            type="text"
                            label="First Name"
                            required
                            maxLength={50}
                        />

                        <TextInput
                            name="lastName"
                            type="text"
                            label="Last Name"
                            required
                            maxLength={50}
                        />

                        <DateInput
                            name="dateOfBirth"
                            type="date"
                            label="Date Of Birth"
                            required
                        />

                        <CheckboxGroupInput
                            name="roles"
                            label="Roles"
                            options={ALL_ROLES}
                        />

                        <div className="mb-3 text-end">
                            <Button
                                to="/user"
                                as={Link}
                                variant="secondary"
                                className="me-1"
                            >
                                Cancel
                            </Button>
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
