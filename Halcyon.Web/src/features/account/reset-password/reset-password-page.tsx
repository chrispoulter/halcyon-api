import { useNavigate, useParams } from 'react-router';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import { useResetPassword } from '@/features/account/hooks/use-reset-password';
import {
    ResetPasswordForm,
    ResetPasswordFormValues,
} from '@/features/account/reset-password/reset-password-form';

type ResetPasswordPageParams = { token: string };

export function ResetPasswordPage() {
    const { token } = useParams() as ResetPasswordPageParams;

    const navigate = useNavigate();

    const { mutate: resetPassword, isPending: isSaving } = useResetPassword();

    function onSubmit(data: ResetPasswordFormValues) {
        resetPassword(
            {
                token,
                ...data,
            },
            {
                onSuccess: () => {
                    toast.success('Your password has been reset.');
                    navigate('/account/login');
                },
                onError: (error) => toast.error(error.message),
            }
        );
    }

    return (
        <main className="mx-auto max-w-screen-sm space-y-6 p-6">
            <Metadata title="Reset Password" />

            <h1 className="scroll-m-20 text-4xl font-extrabold tracking-tight lg:text-5xl">
                Reset Password
            </h1>

            <p className="leading-7">
                Reset your password below. Choose a strong password and
                don&apos;t reuse it for other accounts. For security reasons,
                change your password on a regular basis.
            </p>

            <ResetPasswordForm loading={isSaving} onSubmit={onSubmit} />
        </main>
    );
}
