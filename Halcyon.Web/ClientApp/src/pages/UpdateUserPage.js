import React from 'react';
import { Link } from 'react-router-dom';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, Alert, FormGroup } from 'reactstrap';
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
import { AVAILABLE_ROLES } from '../utils/auth';

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

export const UpdateUserPage = ({ history, match }) => {
    const { loading, data } = useFetch({
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
            <Alert color="info" className="container p-3 mb-3">
                User could not be found.
            </Alert>
        );
    }

    const onSubmit = async data => {
        try {
            const result = await updateUser({ id: match.params.id, ...data });
            toast.success(result.messages);
            history.push('/user');
        } catch (error) {
            console.error(error);
        }
    };

    const onLockUser = async () => {
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
            cancelColor: 'secondary'
        });

        if (!confirmed) {
            return;
        }

        try {
            const result = await lockUser();
            toast.success(result.messages);
        } catch (error) {
            console.error(error);
        }
    };

    const onUnlockUser = async () => {
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
            cancelColor: 'secondary'
        });

        if (!confirmed) {
            return;
        }

        try {
            const result = await unlockUser();
            toast.success(result.messages);
        } catch (error) {
            console.error(error);
        }
    };

    const onDeleteUser = async () => {
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
            cancelColor: 'secondary'
        });

        if (!confirmed) {
            return;
        }

        try {
            const result = await deleteUser();
            toast.success(result.messages);
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
                <small className="text-muted">Update</small>
            </h1>
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
                            {data.isLockedOut ? (
                                <Button
                                    color="warning"
                                    className="mr-1"
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
                                    color="warning"
                                    className="mr-1"
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
                                color="danger"
                                className="mr-1"
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
                                color="primary"
                                loading={isSubmitting}
                                disabled={
                                    isLocking || isUnlocking || isDeleting
                                }
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
