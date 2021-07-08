import React, { Suspense } from 'react';
import { BrowserRouter, Switch, Route } from 'react-router-dom';
import { ToastContainer, Slide } from 'react-toastify';
import {
    AuthProvider,
    Header,
    Footer,
    PrivateRoute,
    Spinner,
    Meta,
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
import { USER_ADMINISTRATOR_ROLES } from './utils/auth';

export const App = () => (
    <Suspense fallback={<Spinner />}>
        <BrowserRouter>
            <AuthProvider>
                <Meta />
                <Header />
                <ErrorBoundary>
                    <Switch>
                        <Route path="/" component={HomePage} exact />
                        <Route
                            path="/register"
                            component={RegisterPage}
                            exact
                        />
                        <Route path="/login" component={LoginPage} exact />
                        <Route
                            path="/forgot-password"
                            component={ForgotPasswordPage}
                            exact
                        />
                        <Route
                            path="/reset-password/:token"
                            component={ResetPasswordPage}
                            exact
                        />
                        <PrivateRoute
                            path="/my-account"
                            component={MyAccountPage}
                            exact
                        />
                        <PrivateRoute
                            path="/update-profile"
                            component={UpdateProfilePage}
                            exact
                        />
                        <PrivateRoute
                            path="/change-password"
                            component={ChangePasswordPage}
                            exact
                        />
                        <PrivateRoute
                            path="/user"
                            requiredRoles={USER_ADMINISTRATOR_ROLES}
                            component={UserPage}
                            exact
                        />
                        <PrivateRoute
                            path="/user/create"
                            requiredRoles={USER_ADMINISTRATOR_ROLES}
                            component={CreateUserPage}
                            exact
                        />
                        <PrivateRoute
                            meta="pages.updateUser.meta"
                            path="/user/:id"
                            requiredRoles={USER_ADMINISTRATOR_ROLES}
                            component={UpdateUserPage}
                            exact
                        />
                        <Route component={NotFoundPage} />
                    </Switch>
                </ErrorBoundary>
                <Footer />
                <ToastContainer
                    position="bottom-right"
                    hideProgressBar
                    draggable={false}
                    transition={Slide}
                />
            </AuthProvider>
        </BrowserRouter>
    </Suspense>
);
