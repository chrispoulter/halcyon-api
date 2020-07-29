import React from 'react';
import { BrowserRouter, Switch } from 'react-router-dom';
import { ToastContainer, Slide } from 'react-toastify';
import {
    AuthProvider,
    ApolloProvider,
    Header,
    Footer,
    PublicRoute,
    PrivateRoute,
    ErrorBoundary
} from './components';
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
import { USER_ADMINISTRATOR } from './utils/auth';

export const App = () => (
    <AuthProvider>
        <ApolloProvider>
            <BrowserRouter>
                <Header />
                <ErrorBoundary>
                    <Switch>
                        <PublicRoute path="/" component={HomePage} exact />
                        <PublicRoute
                            title="Register"
                            path="/register"
                            component={RegisterPage}
                            exact
                        />
                        <PublicRoute
                            title="Login"
                            path="/login"
                            component={LoginPage}
                            exact
                        />
                        <PublicRoute
                            title="Forgot Password"
                            path="/forgot-password"
                            component={ForgotPasswordPage}
                            exact
                        />
                        <PublicRoute
                            title="Reset Password"
                            path="/reset-password/:token"
                            component={ResetPasswordPage}
                            exact
                        />
                        <PrivateRoute
                            title="My Account"
                            path="/my-account"
                            component={MyAccountPage}
                            exact
                        />
                        <PrivateRoute
                            title="Update Profile"
                            path="/update-profile"
                            component={UpdateProfilePage}
                            exact
                        />
                        <PrivateRoute
                            title="Change Password"
                            path="/change-password"
                            component={ChangePasswordPage}
                            exact
                        />
                        <PrivateRoute
                            title="Users"
                            path="/user"
                            requiredRoles={USER_ADMINISTRATOR}
                            component={UserPage}
                            exact
                        />
                        <PrivateRoute
                            title="Create User"
                            path="/user/create"
                            requiredRoles={USER_ADMINISTRATOR}
                            component={CreateUserPage}
                            exact
                        />
                        <PrivateRoute
                            title="Update User"
                            path="/user/:id"
                            requiredRoles={USER_ADMINISTRATOR}
                            component={UpdateUserPage}
                            exact
                        />
                        <PublicRoute component={NotFoundPage} />
                    </Switch>
                </ErrorBoundary>
                <Footer />
            </BrowserRouter>

            <ToastContainer
                position="bottom-right"
                hideProgressBar
                draggable={false}
                transition={Slide}
            />
        </ApolloProvider>
    </AuthProvider>
);
