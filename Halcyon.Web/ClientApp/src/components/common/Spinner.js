import React from 'react';
import { Container, Spinner as BaseSpinner } from 'reactstrap';

export const Spinner = () => (
    <Container className="spinner text-center text-nowrap">
        <BaseSpinner type="grow" color="light" />
        <BaseSpinner type="grow" color="light" />
        <BaseSpinner type="grow" color="light" />
    </Container>
);
