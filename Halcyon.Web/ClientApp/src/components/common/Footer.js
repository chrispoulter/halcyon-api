import React from 'react';
import { config } from '../../utils/config';

const currentYear = new Date().getFullYear();

export const Footer = () => (
    <footer className="container">
        <hr />
        <div className="d-flex justify-content-between">
            <p className="text-muted">
                &copy; <a href="https://www.chrispoulter.com">Chris Poulter</a>{' '}
                {currentYear}
            </p>
            <p className="text-muted">v{config.VERSION}</p>
        </div>
    </footer>
);
