import React from 'react';
import Container from 'react-bootstrap/Container';
import { config } from '../utils/config';

const currentYear = new Date().getFullYear();

export const Footer = () => (
    <Container>
        <hr />
        <div className="d-flex justify-content-between">
            <p className="text-muted">
                &copy; <a href="https://www.chrispoulter.com">Chris Poulter</a>{' '}
                {currentYear}
            </p>
            <p className="text-muted">v{config.VERSION}</p>
        </div>
    </Container>
);
