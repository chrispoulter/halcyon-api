import React from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { Formik, Form } from 'formik';
import * as Yup from 'yup';
import Container from 'react-bootstrap/Container';
import Alert from 'react-bootstrap/Alert';
import {
    Spinner,
    TextInput,
    DateInput,
    CheckboxGroupInput,
    Button
} from '../components';
import { useModal, useToast } from '../contexts';
import {
    useGetUser,
    useUpdateUser,
    useLockUser,
    useUnlockUser,
    useDeleteUser
} from '../services';
import { ALL_ROLES } from '../utils/auth';

export const UpdateUserPage = () => {
    const navigate = useNavigate();

    const { id } = useParams();

    const { showModal } = useModal();

    const toast = useToast();

    const { request, loading, data } = useGetUser(id);

    const { request: updateUser } = useUpdateUser(id);

    const { request: lockUser, loading: isLocking } = useLockUser(id);

    const { request: unlockUser, loading: isUnlocking } = useUnlockUser(id);

    const { request: deleteUser, loading: isDeleting } = useDeleteUser(id);

    if (loading) {
        return <Spinner />;
    }

    if (!data) {
        return (
            <Container>
                <Alert variant="info">User could not be found.</Alert>
            </Container>
        );
    }

    const onSubmit = async variables => {
        const result = await updateUser(variables);

        if (result.ok) {
            toast.success(result.message);
            navigate('/user');
        }
    };

    const onLockUser = () =>
        showModal({
            title: 'Confirm',
            body: (
                <>
                    Are you sure you want to lock{' '}
                    <strong>
                        {data.firstName} {data.lastName}
                    </strong>
                    ?
                </>
            ),
            onOk: async () => {
                const result = await lockUser();
                if (result.ok) {
                    await request();
                    toast.success(result.message);
                }
            }
        });

    const onUnlockUser = () =>
        showModal({
            title: 'Confirm',
            body: (
                <>
                    Are you sure you want to unlock{' '}
                    <strong>
                        {data.firstName} {data.lastName}
                    </strong>
                    ?
                </>
            ),
            onOk: async () => {
                const result = await unlockUser();
                if (result.ok) {
                    await request();
                    toast.success(result.message);
                }
            }
        });

    const onDeleteUser = () =>
        showModal({
            title: 'Confirm',
            body: (
                <>
                    Are you sure you want to delete{' '}
                    <strong>
                        {data.firstName} {data.lastName}
                    </strong>
                    ?
                </>
            ),
            onOk: async () => {
                const result = await deleteUser();
                if (result.ok) {
                    toast.success(result.message);
                    navigate('/user');
                }
            }
        });

    return (
        <Container>
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
                validationSchema={Yup.object({
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
                        <TextInput
                            name="emailAddress"
                            type="email"
                            label="Email Address"
                            required
                            maxLength={254}
                            autoComplete="username"
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
                            {data.isLockedOut ? (
                                <Button
                                    variant="warning"
                                    className="me-1"
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
                                    variant="warning"
                                    className="me-1"
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
                                variant="danger"
                                className="me-1"
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
                                variant="primary"
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
        </Container>
    );
};
