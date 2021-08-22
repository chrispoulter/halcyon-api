import React from 'react';
import { Link } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { toast } from 'react-toastify';
import { Spinner, TextInput, DateInput, Button, useFetch } from '../components';
import { trackEvent } from '../utils/logger';

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
            <div className="container alert alert-info p-3 mb-3">
                Profile could not be found.
            </div>
        );
    }

    const onSubmit = async variables => {
        const result = await updateProfile({
            emailAddress: variables.emailAddress,
            firstName: variables.firstName,
            lastName: variables.lastName,
            dateOfBirth: variables.dateOfBirth
        });

        if (result.ok) {
            toast.success(result.message);
            trackEvent('profile_updated');
            history.push('/my-account');
        }
    };

    return (
        <div className="container">
            <Helmet>
                <title>Update Profile</title>
            </Helmet>

            <h1>Update Profile</h1>
            <hr />

            <Formik
                enableReinitialize={true}
                initialValues={data}
                validationSchema={Yup.object().shape({
                    emailAddress: Yup.string()
                        .label('Email Address')
                        .max(254)
                        .email()
                        .required(),
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

                        <div className="mb-3 text-end">
                            <Link
                                to="/my-account"
                                className="btn btn-secondary me-1"
                            >
                                Cancel
                            </Link>
                            <Button
                                type="submit"
                                className="btn btn-primary"
                                loading={isSubmitting}
                            >
                                Submit
                            </Button>
                        </div>
                    </Form>
                )}
            </Formik>
        </div>
    );
};
