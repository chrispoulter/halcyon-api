import React from 'react';
import { Link } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import confirm from 'reactstrap-confirm';
import { toast } from 'react-toastify';
import {
    Spinner,
    TextInput,
    DateInput,
    CheckboxGroupInput,
    Button,
    useFetch
} from '../components';
import { ALL_ROLES } from '../utils/auth';
import { trackEvent } from '../utils/logger';

export const UpdateUserPage = ({ history, match }) => {
    const { refetch, loading, data } = useFetch({
        method: 'GET',
        url: `/user/${match.params.id}`
    });

    const { refetch: updateUser } = useFetch({
        method: 'PUT',
        url: `/user/${match.params.id}`,
        manual: true
    });

    const { refetch: lockUser, loading: isLocking } = useFetch({
        method: 'PUT',
        url: `/user/${match.params.id}/lock`,
        manual: true
    });

    const { refetch: unlockUser, loading: isUnlocking } = useFetch({
        method: 'PUT',
        url: `/user/${match.params.id}/unlock`,
        manual: true
    });

    const { refetch: deleteUser, loading: isDeleting } = useFetch({
        method: 'DELETE',
        url: `/user/${match.params.id}`,
        manual: true
    });

    if (loading) {
        return <Spinner />;
    }

    if (!data) {
        return (
            <div className="container alert alert-info p-3 mb-3">
                User could not be found.
            </div>
        );
    }

    const onSubmit = async variables => {
        const result = await updateUser({
            emailAddress: variables.emailAddress,
            firstName: variables.firstName,
            lastName: variables.lastName,
            dateOfBirth: variables.dateOfBirth,
            roles: variables.roles
        });

        if (result.ok) {
            toast.success(result.message);

            trackEvent('user_updated', {
                entityId: result.data.id
            });

            history.push('/user');
        }
    };

    const onLockUser = async () => {
        trackEvent('screen_view', {
            screen_name: 'lock-user-modal'
        });

        const confirmed = await confirm({
            title: 'Confirm',
            message: (
                <>
                    Are you sure you want to lock{' '}
                    <strong>
                        {data.firstName} {data.lastName}
                    </strong>
                    ?
                </>
            ),
            confirmText: 'Ok',
            cancelText: 'Cancel',
            cancelColor: 'secondary'
        });

        if (!confirmed) {
            return;
        }

        const result = await lockUser();
        if (result.ok) {
            await refetch();

            toast.success(result.message);

            trackEvent('user_locked', {
                entityId: result.data.id
            });
        }
    };

    const onUnlockUser = async () => {
        trackEvent('screen_view', {
            screen_name: 'unlock-user-modal'
        });

        const confirmed = await confirm({
            title: 'Confirm',
            message: (
                <>
                    Are you sure you want to unlock{' '}
                    <strong>
                        {data.firstName} {data.lastName}
                    </strong>
                    ?
                </>
            ),
            confirmText: 'Ok',
            cancelText: 'Cancel',
            cancelColor: 'secondary'
        });

        if (!confirmed) {
            return;
        }

        const result = await unlockUser();
        if (result.ok) {
            toast.success(result.message);

            trackEvent('user_unlocked', {
                entityId: result.data.id
            });
        }
    };

    const onDeleteUser = async () => {
        trackEvent('screen_view', {
            screen_name: 'delete-user-modal'
        });

        const confirmed = await confirm({
            title: 'Confirm',
            message: (
                <>
                    Are you sure you want to delete{' '}
                    <strong>
                        {data.firstName} {data.lastName}
                    </strong>
                    ?
                </>
            ),
            confirmText: 'Ok',
            cancelText: 'Cancel',
            cancelColor: 'secondary'
        });

        if (!confirmed) {
            return;
        }

        const result = await deleteUser();
        if (result.ok) {
            toast.success(result.message);

            trackEvent('user_deleted', {
                entityId: result.data.id
            });

            history.push('/user');
        }
    };

    return (
        <div className="container">
            <Helmet>
                <title>Update User</title>
            </Helmet>

            <h1>
                User
                <br />
                <small className="text-muted">Update</small>
            </h1>
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

                        <Field
                            name="roles"
                            label="Roles"
                            options={ALL_ROLES}
                            component={CheckboxGroupInput}
                        />

                        <div className="mb-3 text-end">
                            <Link to="/user" className="btn btn-secondary me-1">
                                Cancel
                            </Link>
                            {data.isLockedOut ? (
                                <Button
                                    className="btn btn-warning me-1"
                                    loading={isUnlocking}
                                    disabled={
                                        isLocking || isDeleting || isSubmitting
                                    }
                                    onClick={onUnlockUser}
                                >
                                    Unlock
                                </Button>
                            ) : (
                                <Button
                                    className="btn btn-warning me-1"
                                    loading={isLocking}
                                    disabled={
                                        isUnlocking ||
                                        isDeleting ||
                                        isSubmitting
                                    }
                                    onClick={onLockUser}
                                >
                                    Lock
                                </Button>
                            )}
                            <Button
                                className="btn btn-danger me-1"
                                loading={isDeleting}
                                disabled={
                                    isLocking || isUnlocking || isSubmitting
                                }
                                onClick={onDeleteUser}
                            >
                                Delete
                            </Button>
                            <Button
                                type="submit"
                                className="btn btn-primary"
                                loading={isSubmitting}
                                disabled={
                                    isLocking || isUnlocking || isDeleting
                                }
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
