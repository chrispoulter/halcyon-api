import { RouteObject } from 'react-router';
import { ProtectedRoute } from '@/components/protected-route';
import { ProfilePage } from '@/features/profile/profile/profile-page';
import { UpdateProfilePage } from '@/features/profile/update-profile/update-profile-page';
import { ChangePasswordPage } from '@/features/profile/change-password/change-password-page';

export const profileRoutes: RouteObject[] = [
    {
        path: 'profile',
        element: <ProtectedRoute />,
        children: [
            { index: true, element: <ProfilePage /> },
            { path: 'update-profile', element: <UpdateProfilePage /> },
            { path: 'change-password', element: <ChangePasswordPage /> },
        ],
    },
];
