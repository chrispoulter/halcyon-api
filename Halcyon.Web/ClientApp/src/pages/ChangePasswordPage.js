import React from 'react';
import { Link } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { toast } from 'react-toastify';
import { TextInput, Button, useFetch } from '../components';
import { trackEvent } from '../utils/logger';

export const ChangePasswordPage = ({ history }) => {
    const { refetch: changePassword } = useFetch({
        method: 'PUT',
        url: '/manage/changepassword',
        manual: true
    });

    const onSubmit = async variables => {
        const result = await changePassword({
            currentPassword: variables.currentPassword,
            newPassword: variables.newPassword
        });

        if (result.ok) {
            toast.success(result.message);
            trackEvent('password_changed');
            history.push('/my-account');
        }
    };

    return (
        <div className="container">
            <Helmet>
                <title>Change Password</title>
            </Helmet>

            <h1>Change Password</h1>
            <hr />

            <Formik
                initialValues={{
                    currentPassword: '',
                    newPassword: '',
                    confirmNewPassword: ''
                }}
                validationSchema={Yup.object().shape({
                    currentPassword: Yup.string()
                        .label('Current Password')
                        .required(),
                    newPassword: Yup.string()
                        .label('New Password')
                        .min(8)
                        .max(50)
                        .required(),
                    confirmNewPassword: Yup.string()
                        .label('Confirm New Password')
                        .required()
                        .oneOf([Yup.ref('newPassword')])
                })}
                onSubmit={onSubmit}
            >
                {({ isSubmitting }) => (
                    <Form noValidate>
                        <Field
                            name="currentPassword"
                            type="password"
                            label="Current Password"
                            required
                            maxLength={50}
                            autoComplete="current-password"
                            component={TextInput}
                        />
                        <Field
                            name="newPassword"
                            type="password"
                            label="New Password"
                            required
                            maxLength={50}
                            autoComplete="new-password"
                            component={TextInput}
                        />
                        <Field
                            name="confirmNewPassword"
                            type="password"
                            label="Confirm New Password"
                            required
                            maxLength={50}
                            autoComplete="new-password"
                            component={TextInput}
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

            <p>
                Forgotten your password?{' '}
                <Link to="/forgot-password">Request reset</Link>
            </p>
        </div>
    );
};
