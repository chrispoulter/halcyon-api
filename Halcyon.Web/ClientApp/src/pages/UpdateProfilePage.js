import React from 'react';
import { Link } from 'react-router-dom';
import { useQuery, useMutation } from '@apollo/react-hooks';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, Alert, FormGroup } from 'reactstrap';
import { toast } from 'react-toastify';
import { GET_PROFILE, UPDATE_PROFILE } from '../graphql';
import { Spinner, TextInput, DateInput, Button } from '../components';

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
    const { loading, data } = useQuery(GET_PROFILE);

    const [updateProfile] = useMutation(UPDATE_PROFILE);

    if (loading) {
        return <Spinner />;
    }

    if (!data?.getProfile) {
        return (
            <Alert color="info" className="container p-3 mb-3">
                Profile could not be found.
            </Alert>
        );
    }

    const onSubmit = async variables => {
        try {
            const result = await updateProfile({ variables });
            toast.success(result.data.updateProfile.message);
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
                initialValues={data.getProfile}
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
