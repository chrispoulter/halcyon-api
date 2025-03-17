import { Link, useNavigate } from 'react-router';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Metadata } from '@/components/metadata';
import { QueryError } from '@/components/query-error';
import {
    ChangePasswordForm,
    type ChangePasswordFormValues,
} from '@/features/profile/change-password/change-password-form';
import { ChangePasswordLoading } from '@/features/profile/change-password/change-password-loading';
import { useGetProfile } from '@/features/profile/hooks/use-get-profile';
import { useChangePassword } from '@/features/profile/hooks/use-change-password';

export function ChangePasswordPage() {
    const navigate = useNavigate();

    const {
        data: profile,
        isPending,
        isFetching,
        isSuccess,
        error,
    } = useGetProfile();

    const { mutate: changePassword, isPending: isSaving } = useChangePassword();

    if (isPending) {
        return <ChangePasswordLoading />;
    }

    if (!isSuccess) {
        return <QueryError error={error} />;
    }

    const { version } = profile;

    function onSubmit(data: ChangePasswordFormValues) {
        changePassword(
            {
                ...data,
                version,
            },
            {
                onSuccess: () => {
                    toast.success('Your password has been changed.');
                    navigate('/profile');
                },
                onError: (error) => toast.error(error.message),
            }
        );
    }

    return (
        <main className="mx-auto max-w-screen-sm space-y-6 p-6">
            <Metadata title="Change Password" />

            <h1 className="scroll-m-20 text-4xl font-extrabold tracking-tight lg:text-5xl">
                Change Password
            </h1>

            <p className="leading-7">
                Change your password below. Choose a strong password and
                don&apos;t reuse it for other accounts. For security reasons,
                change your password on a regular basis.
            </p>

            <ChangePasswordForm
                onSubmit={onSubmit}
                loading={isSaving}
                disabled={isFetching || isSaving}
            >
                <Button asChild variant="outline">
                    <Link to="/profile">Cancel</Link>
                </Button>
            </ChangePasswordForm>

            <p className="text-muted-foreground text-sm">
                Forgotten your password?{' '}
                <Link
                    to="/account/forgot-password"
                    className="underline underline-offset-4"
                >
                    Request reset
                </Link>
            </p>
        </main>
    );
}
