import React from 'react';
import { Link } from 'react-router-dom';
import { useMutation } from '@apollo/react-hooks';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, FormGroup } from 'reactstrap';
import { toast } from 'react-toastify';
import { CREATE_USER } from '../graphql';
import {
    TextInput,
    DateInput,
    CheckboxGroupInput,
    Button
} from '../components';
import { AVAILABLE_ROLES } from '../utils/auth';

const initialValues = {
    emailAddress: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
    dateOfBirth: '',
    roles: []
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

export const CreateUserPage = ({ history }) => {
    const [createUser] = useMutation(CREATE_USER);

    const onSubmit = async variables => {
        try {
            const result = await createUser({ variables });
            toast.success(result.data.createUser.message);
            history.push('/user');
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <Container>
            <h1>
                User
                <br />
                <small className="text-muted">Create</small>
            </h1>
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

                        <Field
                            name="roles"
                            label="Roles"
                            options={AVAILABLE_ROLES}
                            component={CheckboxGroupInput}
                        />

                        <FormGroup className="text-right">
                            <Button to="/user" className="mr-1" tag={Link}>
                                Cancel
                            </Button>
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
        </Container>
    );
};
