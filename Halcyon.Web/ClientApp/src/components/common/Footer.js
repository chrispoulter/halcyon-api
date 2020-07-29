import React from 'react';
import moment from 'moment';
import { Container } from 'reactstrap';

const currentYear = moment().year();

export const Footer = () => (
    <footer>
        <Container className="pt-3">
            <hr />
            <p>
                &copy; <a href="https://www.chrispoulter.com">Chris Poulter</a>{' '}
                {currentYear}
            </p>
        </Container>
    </footer>
);
