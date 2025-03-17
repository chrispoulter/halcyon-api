import { Link, useNavigate } from 'react-router';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Metadata } from '@/components/metadata';
import {
    CreateUserForm,
    type CreateUserFormValues,
} from '@/features/user/create-user/create-user-form';
import { useCreateUser } from '@/features/user/hooks/use-create-user';

export function CreateUserPage() {
    const navigate = useNavigate();

    const { mutate: createUser, isPending: isSaving } = useCreateUser();

    function onSubmit(data: CreateUserFormValues) {
        createUser(data, {
            onSuccess: () => {
                toast.success('User successfully created.');
                navigate('/user');
            },
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <main className="mx-auto max-w-screen-sm space-y-6 p-6">
            <Metadata title="Create User" />

            <h1 className="scroll-m-20 text-4xl font-extrabold tracking-tight lg:text-5xl">
                Users
            </h1>
            <h2 className="scroll-m-20 border-b pb-2 text-3xl font-semibold tracking-tight">
                Create
            </h2>

            <p className="leading-7">
                Create a new account for a user to access the full range of
                features available on this site.
            </p>

            <CreateUserForm onSubmit={onSubmit} loading={isSaving}>
                <Button asChild variant="outline">
                    <Link to="/user">Cancel</Link>
                </Button>
            </CreateUserForm>
        </main>
    );
}
