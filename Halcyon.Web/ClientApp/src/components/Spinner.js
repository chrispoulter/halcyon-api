import React from 'react';
import BaseSpinner from 'react-bootstrap/Spinner';

export const Spinner = () => (
    <div className="p-5">
        <div className="d-flex justify-content-center p-5">
            <BaseSpinner animation="grow" variant="secondary" />
            <BaseSpinner animation="grow" variant="secondary" />
            <BaseSpinner animation="grow" variant="secondary" />
        </div>
    </div>
);
