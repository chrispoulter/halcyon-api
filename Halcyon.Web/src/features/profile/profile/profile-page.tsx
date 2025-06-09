import { Link, useNavigate } from 'react-router';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { useAuth } from '@/components/auth-provider';
import { Metadata } from '@/components/metadata';
import { QueryError } from '@/components/query-error';
import { useGetProfile } from '@/features/profile/hooks/use-get-profile';
import { useDeleteAccount } from '@/features/profile/hooks/use-delete-account';
import { DeleteAccountButton } from '@/features/profile/profile/delete-account-button';
import { ProfileLoading } from '@/features/profile/profile/profile-loading';
import { toLocaleString } from '@/lib/dates';

export function ProfilePage() {
    const navigate = useNavigate();

    const { clearAuth } = useAuth();

    const {
        data: profile,
        isPending,
        isFetching,
        isSuccess,
        error,
    } = useGetProfile();

    const { mutate: deleteAccount, isPending: isDeleting } = useDeleteAccount();

    if (isPending) {
        return <ProfileLoading />;
    }

    if (!isSuccess) {
        return <QueryError error={error} />;
    }

    const { version } = profile;

    function onDelete() {
        deleteAccount(
            {
                version,
            },
            {
                onSuccess: () => {
                    toast.success('Your account has been deleted.');
                    clearAuth();
                    navigate('/');
                },
                onError: (error) => toast.error(error.message),
            }
        );
    }

    return (
        <main className="mx-auto max-w-screen-sm space-y-6 p-6">
            <Metadata title="My Account" />

            <h1 className="scroll-m-20 text-4xl font-extrabold tracking-tight lg:text-5xl">
                My Account
            </h1>

            <h2 className="scroll-m-20 border-b pb-2 text-3xl font-semibold tracking-tight">
                Personal Details
            </h2>

            <dl className="space-y-2">
                <dt className="text-sm leading-none font-medium">
                    Email Address
                </dt>
                <dd className="text-muted-foreground truncate text-sm">
                    {profile.emailAddress}
                </dd>
                <dt className="text-sm leading-none font-medium">Name</dt>
                <dd className="text-muted-foreground truncate text-sm">
                    {profile.firstName} {profile.lastName}
                </dd>
                <dt className="text-sm leading-none font-medium">
                    Date Of Birth
                </dt>
                <dd className="text-muted-foreground truncate text-sm">
                    {toLocaleString(profile.dateOfBirth)}
                </dd>
            </dl>

            <Button asChild className="w-full sm:w-auto">
                <Link to="/profile/update-profile">Update Profile</Link>
            </Button>

            <h2 className="scroll-m-20 border-b pb-2 text-3xl font-semibold tracking-tight">
                Login Details
            </h2>

            <p className="leading-7">
                Choose a strong password and don&apos;t reuse it for other
                accounts. For security reasons, change your password on a
                regular basis.
            </p>

            <Button asChild className="w-full sm:w-auto">
                <Link to="/profile/change-password">Change Password</Link>
            </Button>

            <h2 className="scroll-m-20 border-b pb-2 text-3xl font-semibold tracking-tight">
                Settings
            </h2>

            <p className="leading-7">
                Once you delete your account all of your data and settings will
                be removed. Please be certain.
            </p>

            <DeleteAccountButton
                onClick={onDelete}
                loading={isDeleting}
                disabled={isFetching}
                className="w-full sm:w-auto"
            />
        </main>
    );
}
