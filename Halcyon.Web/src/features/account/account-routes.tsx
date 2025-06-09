import { RouteObject } from 'react-router';
import { LoginPage } from '@/features/account/login/login-page';
import { RegisterPage } from '@/features/account/register/register-page';
import { ForgotPasswordPage } from '@/features/account/forgot-password/forgot-password-page';
import { ResetPasswordPage } from '@/features/account/reset-password/reset-password-page';

export const accountRoutes: RouteObject[] = [
    {
        path: 'account',
        children: [
            { path: 'login', element: <LoginPage /> },
            { path: 'register', element: <RegisterPage /> },
            { path: 'forgot-password', element: <ForgotPasswordPage /> },
            { path: 'reset-password/:token', element: <ResetPasswordPage /> },
        ],
    },
];
