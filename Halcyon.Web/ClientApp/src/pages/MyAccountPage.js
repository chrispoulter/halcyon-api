import React, { useContext } from 'react';
import { Link } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { toast } from 'react-toastify';
import Container from 'react-bootstrap/Container';
import Alert from 'react-bootstrap/Alert';
import {
    Button,
    Spinner,
    AuthContext,
    useFetch,
    useModal
} from '../components';
import { trackEvent } from '../utils/logger';

export const MyAccountPage = ({ history }) => {
    const { removeToken } = useContext(AuthContext);

    const { confirm } = useModal();

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
        return <Alert variant="info">Profile could not be found.</Alert>;
    }

    const onDeleteAccount = async () => {
        trackEvent('screen_view', {
            screen_name: 'delete-account-modal'
        });

        const confirmed = await confirm({
            title: 'Confirm',
            message: 'Are you sure you want to delete your account?',
            confirmText: 'Ok',
            cancelText: 'Cancel',
            cancelColor: 'secondary'
        });

        if (!confirmed) {
            return;
        }

        const result = await deleteAccount();

        if (result.ok) {
            toast.success(result.message);

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
                <title>My Account</title>
            </Helmet>

            <h1>My Account</h1>
            <hr />

            <div className="d-flex justify-content-between">
                <h3>Profile</h3>
                <Button to="/update-profile" as={Link} variant="primary">
                    Update
                </Button>
            </div>
            <hr />

            <p>
                <span className="text-muted">Email Address</span>
                <br />
                {data.emailAddress}
            </p>

            <p>
                <span className="text-muted">Password</span>
                <br />
                ********
                <br />
                <Link to="/change-password">Change your password...</Link>
            </p>

            <p>
                <span className="text-muted">Name</span>
                <br />
                {data.firstName} {data.lastName}
            </p>

            <p>
                <span className="text-muted">Date Of Birth</span>
                <br />
                {new Date(data.dateOfBirth).toLocaleDateString('en', {
                    day: '2-digit',
                    month: 'long',
                    year: 'numeric'
                })}
            </p>

            <h3>Settings</h3>
            <hr />
            <p>
                Once you delete your account all of your data and settings will
                be removed. Please be certain.
            </p>
            <p>
                <Button
                    variant="danger"
                    loading={isDeleting}
                    onClick={onDeleteAccount}
                >
                    Delete Account
                </Button>
            </p>
        </Container>
    );
};
