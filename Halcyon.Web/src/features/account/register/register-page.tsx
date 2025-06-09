import { Link, useNavigate } from 'react-router';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import { useRegister } from '@/features/account/hooks/use-register';
import {
    RegisterForm,
    type RegisterFormValues,
} from '@/features/account/register/register-form';

export function RegisterPage() {
    const navigate = useNavigate();

    const { mutate: register, isPending: isSaving } = useRegister();

    function onSubmit(data: RegisterFormValues) {
        register(data, {
            onSuccess: () => {
                toast.success('User successfully registered.');
                navigate('/account/login');
            },
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <main className="mx-auto max-w-screen-sm space-y-6 p-6">
            <Metadata title="Register" />

            <h1 className="scroll-m-20 text-4xl font-extrabold tracking-tight lg:text-5xl">
                Register
            </h1>

            <p className="leading-7">
                Register for a new account to access the full range of features
                available on this site.
            </p>

            <RegisterForm loading={isSaving} onSubmit={onSubmit} />

            <p className="text-muted-foreground text-sm">
                Already have an account?{' '}
                <Link
                    to="/account/login"
                    className="underline underline-offset-4"
                >
                    Log in now
                </Link>
            </p>
        </main>
    );
}
