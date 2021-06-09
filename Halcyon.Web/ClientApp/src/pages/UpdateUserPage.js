import React from 'react';
import { Link } from 'react-router-dom';
import { useTranslation, Trans } from 'react-i18next';
import { Helmet } from 'react-helmet';
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
import { ALL_ROLES } from '../utils/auth';
import { trackEvent } from '../utils/logger';

export const UpdateUserPage = ({ history, match }) => {
    const { t } = useTranslation();

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
            <Alert color="info" className="container p-3 mb-3">
                {t('pages.updateUser.userNotFound')}
            </Alert>
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
            toast.success(t(`api.codes.${result.code}`));

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
            title: t('pages.updateUser.lockModal.title'),
            message: (
                <Trans
                    i18nKey="pages.updateUser.lockModal.message"
                    values={data}
                />
            ),
            confirmText: t('pages.updateUser.lockModal.confirm'),
            cancelText: t('pages.updateUser.lockModal.cancel'),
            cancelColor: 'secondary'
        });

        if (!confirmed) {
            return;
        }

        const result = await lockUser();
        if (result.ok) {
            await refetch();

            toast.success(t(`api.codes.${result.code}`));

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
            title: t('pages.updateUser.unlockModal.title'),
            message: (
                <Trans
                    i18nKey="pages.updateUser.unlockModal.message"
                    values={data}
                />
            ),
            confirmText: t('pages.updateUser.unlockModal.confirm'),
            cancelText: t('pages.updateUser.unlockModal.cancel'),
            cancelColor: 'secondary'
        });

        if (!confirmed) {
            return;
        }

        const result = await unlockUser();
        if (result.ok) {
            toast.success(t(`api.codes.${result.code}`));

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
            title: t('pages.updateUser.deleteModal.title'),
            message: (
                <Trans
                    i18nKey="pages.updateUser.deleteModal.message"
                    values={data}
                />
            ),
            confirmText: t('pages.updateUser.deleteModal.confirm'),
            cancelText: t('pages.updateUser.deleteModal.cancel'),
            cancelColor: 'secondary'
        });

        if (!confirmed) {
            return;
        }

        const result = await deleteUser();
        if (result.ok) {
            toast.success(t(`api.codes.${result.code}`));

            trackEvent('user_deleted', {
                entityId: result.data.id
            });

            history.push('/user');
        }
    };

    return (
        <Container>
            <Helmet>
                <title>{t('pages.updateUser.meta.title')}</title>
            </Helmet>

            <h1>
                {t('pages.updateUser.title')}
                <br />
                <small className="text-muted">
                    {t('pages.updateUser.subtitle')}
                </small>
            </h1>
            <hr />

            <Formik
                enableReinitialize={true}
                initialValues={data}
                validationSchema={Yup.object().shape({
                    emailAddress: Yup.string()
                        .label(t('pages.updateUser.form.emailAddress'))
                        .max(254)
                        .email()
                        .required(),
                    firstName: Yup.string()
                        .label(t('pages.updateUser.form.firstName'))
                        .max(50)
                        .required(),
                    lastName: Yup.string()
                        .label(t('pages.updateUser.form.lastName'))
                        .max(50)
                        .required(),
                    dateOfBirth: Yup.string()
                        .label(t('pages.updateUser.form.dateOfBirth'))
                        .required()
                })}
                onSubmit={onSubmit}
            >
                {({ isSubmitting }) => (
                    <Form noValidate>
                        <Field
                            name="emailAddress"
                            type="email"
                            label={t('pages.updateUser.form.emailAddress')}
                            required
                            maxLength={254}
                            autoComplete="username"
                            component={TextInput}
                        />

                        <Field
                            name="firstName"
                            type="text"
                            label={t('pages.updateUser.form.firstName')}
                            required
                            maxLength={50}
                            component={TextInput}
                        />

                        <Field
                            name="lastName"
                            type="text"
                            label={t('pages.updateUser.form.lastName')}
                            required
                            maxLength={50}
                            component={TextInput}
                        />

                        <Field
                            name="dateOfBirth"
                            type="date"
                            label={t('pages.updateUser.form.dateOfBirth')}
                            required
                            component={DateInput}
                        />

                        <Field
                            name="roles"
                            label={t('pages.updateUser.form.roles')}
                            options={ALL_ROLES.map(role => ({
                                value: role,
                                label: t(`api.userRoles.${role}`)
                            }))}
                            component={CheckboxGroupInput}
                        />

                        <FormGroup className="text-right">
                            <Button to="/user" className="mr-1" tag={Link}>
                                {t('pages.updateUser.cancelButton')}
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
                                    {t('pages.updateUser.unlockButton')}
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
                                    {t('pages.updateUser.lockButton')}
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
                                {t('pages.updateUser.deleteButton')}
                            </Button>
                            <Button
                                type="submit"
                                color="primary"
                                loading={isSubmitting}
                                disabled={
                                    isLocking || isUnlocking || isDeleting
                                }
                            >
                                {t('pages.updateUser.submitButton')}
                            </Button>
                        </FormGroup>
                    </Form>
                )}
            </Formik>
        </Container>
    );
};
