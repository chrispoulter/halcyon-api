import React from 'react';
import { BrowserRouter } from 'react-router-dom';
import { Provider } from 'react-redux';
import {
    Header,
    Footer,
    Meta,
    ErrorBoundary,
    Modal,
    Toast
} from './components';
import { store } from './redux';
import { Router } from './Router';

export const App = () => (
    <Provider store={store}>
        <BrowserRouter>
            <Meta />
            <Header />
            <ErrorBoundary>
                <Router />
            </ErrorBoundary>
            <Footer />
            <Modal />
            <Toast />
        </BrowserRouter>
    </Provider>
);
