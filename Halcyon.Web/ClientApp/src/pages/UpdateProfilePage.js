import React from 'react';
import { Link } from 'react-router-dom';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, Alert, FormGroup } from 'reactstrap';
import { toast } from 'react-toastify';
import { Spinner, TextInput, DateInput, Button, useFetch } from '../components';

const validationSchema = Yup.object().shape({
    emailAddress: Yup.string()
        .label('Email Address')
        .max(254)
        .email()
        .required(),
    firstName: Yup.string().label('First Name').max(50).required(),
    lastName: Yup.string().label('Last Name').max(50).required(),
    dateOfBirth: Yup.string().label('Date of Birth').required()
});

export const UpdateProfilePage = ({ history }) => {
    const { loading, data } = useFetch({
        method: 'GET',
        url: '/manage'
    });

    const { refetch: updateProfile } = useFetch({
        method: 'PUT',
        url: '/manage',
        manual: true
    });

    if (loading) {
        return <Spinner />;
    }

    if (!data) {
        return (
            <Alert color="info" className="container p-3 mb-3">
                Profile could not be found.
            </Alert>
        );
    }

    console.log('data', data);

    const onSubmit = async data => {
        try {
            const result = await updateProfile(data);
            toast.success(result.messages);
            history.push('/my-account');
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <Container>
            <h1>Update Profile</h1>
            <hr />

            <Formik
                enableReinitialize={true}
                initialValues={data}
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
                                to="/my-account"
                                className="mr-1"
                                tag={Link}
                            >
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
