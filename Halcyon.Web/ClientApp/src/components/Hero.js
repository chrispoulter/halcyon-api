import React from 'react';
import Container from 'react-bootstrap/Container';

export const Hero = ({ children }) => (
    <div className="bg-secondary pt-5 pb-5 mb-3">
        <Container className="pt-5 pb-5">{children}</Container>
    </div>
);
