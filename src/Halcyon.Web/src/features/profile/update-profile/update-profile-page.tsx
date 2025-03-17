import { Link, useNavigate } from 'react-router';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Metadata } from '@/components/metadata';
import { QueryError } from '@/components/query-error';
import {
    UpdateProfileForm,
    type UpdateProfileFormValues,
} from '@/features/profile/update-profile/update-profile-form';
import { UpdateProfileLoading } from '@/features/profile/update-profile/update-profile-loading';
import { useGetProfile } from '@/features/profile/hooks/use-get-profile';
import { useUpdateProfile } from '@/features/profile/hooks/use-update-profile';

export function UpdateProfilePage() {
    const navigate = useNavigate();

    const {
        data: profile,
        isPending,
        isFetching,
        isSuccess,
        error,
    } = useGetProfile();

    const { mutate: updateProfile, isPending: isSaving } = useUpdateProfile();

    if (isPending) {
        return <UpdateProfileLoading />;
    }

    if (!isSuccess) {
        return <QueryError error={error} />;
    }

    const { version } = profile;

    function onSubmit(data: UpdateProfileFormValues) {
        updateProfile(
            {
                ...data,
                version,
            },
            {
                onSuccess: () => {
                    toast.success('Your profile has been updated.');
                    navigate('/profile');
                },
                onError: (error) => toast.error(error.message),
            }
        );
    }

    return (
        <main className="mx-auto max-w-screen-sm space-y-6 p-6">
            <Metadata title="Update Profile" />

            <h1 className="scroll-m-20 text-4xl font-extrabold tracking-tight lg:text-5xl">
                Update Profile
            </h1>

            <p className="leading-7">
                Update your personal details below. Your email address is used
                to login to your account.
            </p>

            <UpdateProfileForm
                profile={profile}
                onSubmit={onSubmit}
                disabled={isFetching || isSaving}
                loading={isSaving}
            >
                <Button asChild variant="outline">
                    <Link to="/profile">Cancel</Link>
                </Button>
            </UpdateProfileForm>
        </main>
    );
}
