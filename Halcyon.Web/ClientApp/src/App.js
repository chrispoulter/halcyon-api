import React, { Suspense } from 'react';
import { BrowserRouter } from 'react-router-dom';
import {
    AuthProvider,
    ModalProvider,
    ToastProvider,
    Header,
    Footer,
    Spinner,
    Meta,
    ErrorBoundary
} from './components';
import { Routes } from './Routes';

export const App = () => (
    <Suspense fallback={<Spinner />}>
        <BrowserRouter>
            <AuthProvider>
                <ModalProvider>
                    <ToastProvider>
                        <Meta />
                        <Header />
                        <ErrorBoundary>
                            <Routes />
                        </ErrorBoundary>
                        <Footer />
                    </ToastProvider>
                </ModalProvider>
            </AuthProvider>
        </BrowserRouter>
    </Suspense>
);
