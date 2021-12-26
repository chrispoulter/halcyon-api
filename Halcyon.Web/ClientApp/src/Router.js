import React from 'react';
import { Routes, Route } from 'react-router-dom';
import { RequireAuth } from './components';
import {
    HomePage,
    NotFoundPage,
    LoginPage,
    RegisterPage,
    ForgotPasswordPage,
    ResetPasswordPage,
    MyAccountPage,
    UpdateProfilePage,
    ChangePasswordPage,
    UserPage,
    CreateUserPage,
    UpdateUserPage
} from './pages';
import { USER_ADMINISTRATOR_ROLES } from './utils/auth';

export const Router = () => (
    <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/forgot-password" element={<ForgotPasswordPage />} />
        <Route path="/reset-password/:token" element={<ResetPasswordPage />} />
        <Route
            path="/my-account"
            element={
                <RequireAuth>
                    <MyAccountPage />
                </RequireAuth>
            }
        />

        <Route
            path="/update-profile"
            element={
                <RequireAuth>
                    <UpdateProfilePage />
                </RequireAuth>
            }
        />

        <Route
            path="/change-password"
            element={
                <RequireAuth>
                    <ChangePasswordPage />
                </RequireAuth>
            }
        />

        <Route
            path="/user"
            element={
                <RequireAuth requiredRoles={USER_ADMINISTRATOR_ROLES}>
                    <UserPage />
                </RequireAuth>
            }
        />

        <Route
            path="/user/create"
            element={
                <RequireAuth requiredRoles={USER_ADMINISTRATOR_ROLES}>
                    <CreateUserPage />
                </RequireAuth>
            }
        />

        <Route
            path="/user/:id"
            element={
                <RequireAuth requiredRoles={USER_ADMINISTRATOR_ROLES}>
                    <UpdateUserPage />
                </RequireAuth>
            }
        />

        <Route path="*" element={<NotFoundPage />} />
    </Routes>
);
