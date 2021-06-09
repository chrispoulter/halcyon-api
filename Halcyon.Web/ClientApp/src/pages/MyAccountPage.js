import React, { useContext } from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Helmet } from 'react-helmet';
import { Container, Alert } from 'reactstrap';
import confirm from 'reactstrap-confirm';
import { toast } from 'react-toastify';
import { Button, Spinner, AuthContext, useFetch } from '../components';
import { trackEvent } from '../utils/logger';

export const MyAccountPage = ({ history }) => {
    const { t, i18n } = useTranslation();

    const { removeToken } = useContext(AuthContext);

    const { loading, data } = useFetch({
        method: 'GET',
        url: '/manage'
    });

    const { refetch: deleteAccount, loading: isDeleting } = useFetch({
        method: 'DELETE',
        url: '/manage',
        manual: true
    });

    if (loading) {
        return <Spinner />;
    }

    if (!data) {
        return (
            <Alert color="info" className="container p-3 mb-3">
                {t('pages.myAccount.profileNotFound')}
            </Alert>
        );
    }

    const onDeleteAccount = async () => {
        trackEvent('screen_view', {
            screen_name: 'delete-account-modal'
        });

        const confirmed = await confirm({
            title: t('pages.myAccount.deleteModal.title'),
            message: t('pages.myAccount.deleteModal.message'),
            confirmText: t('pages.myAccount.deleteModal.confirm'),
            cancelText: t('pages.myAccount.deleteModal.cancel'),
            cancelColor: 'secondary'
        });

        if (!confirmed) {
            return;
        }

        const result = await deleteAccount();

        if (result.ok) {
            toast.success(t(`api.codes.${result.code}`));

            trackEvent('account_deleted', {
                entityId: result.data.id
            });

            removeToken();
            history.push('/');
        }
    };

    return (
        <Container>
            <Helmet>
                <title>{t('pages.myAccount.meta.title')}</title>
            </Helmet>

            <h1>{t('pages.myAccount.title')}</h1>
            <hr />

            <div className="d-flex justify-content-between">
                <h3>{t('pages.myAccount.profileSection.title')}</h3>
                <Button
                    to="/update-profile"
                    color="primary"
                    className="align-self-start"
                    tag={Link}
                >
                    {t('pages.myAccount.profileSection.updateButton')}
                </Button>
            </div>
            <hr />

            <p>
                <span className="text-muted">
                    {t('pages.myAccount.profileSection.emailAddress')}
                </span>
                <br />
                {data.emailAddress}
            </p>

            <p>
                <span className="text-muted">
                    {t('pages.myAccount.profileSection.password')}
                </span>
                <br />
                ********
                <br />
                <Link to="/change-password">
                    {t('pages.myAccount.profileSection.changePasswordLink')}
                </Link>
            </p>

            <p>
                <span className="text-muted">
                    {t('pages.myAccount.profileSection.name')}
                </span>
                <br />
                {data.firstName} {data.lastName}
            </p>

            <p>
                <span className="text-muted">
                    {t('pages.myAccount.profileSection.dateOfBirth')}
                </span>
                <br />
                {new Date(data.dateOfBirth).toLocaleDateString(i18n.language, {
                    day: '2-digit',
                    month: 'long',
                    year: 'numeric'
                })}
            </p>

            <h3>{t('pages.myAccount.settingsSection.title')}</h3>
            <hr />
            <p>{t('pages.myAccount.settingsSection.deletePrompt')}</p>
            <p>
                <Button
                    color="danger"
                    loading={isDeleting}
                    onClick={onDeleteAccount}
                >
                    {t('pages.myAccount.settingsSection.deleteButton')}
                </Button>
            </p>
        </Container>
    );
};
