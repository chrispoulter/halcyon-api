import React from 'react';
import { Routes, Route } from 'react-router-dom';
import { PrivateRoute } from './components';
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
        <Route element={<PrivateRoute />}>
            <Route path="/my-account" element={<MyAccountPage />} />
            <Route path="/update-profile" element={<UpdateProfilePage />} />
            <Route path="/change-password" element={<ChangePasswordPage />} />
        </Route>
        <Route
            element={<PrivateRoute requiredRoles={USER_ADMINISTRATOR_ROLES} />}
        >
            <Route path="/user" element={<UserPage />} />
            <Route path="/user/create" element={<CreateUserPage />} />
            <Route path="/user/:id" element={<UpdateUserPage />} />
        </Route>
        <Route path="*" element={<NotFoundPage />} />
    </Routes>
);
