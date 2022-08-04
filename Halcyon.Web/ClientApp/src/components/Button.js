import React from 'react';
import BaseButton from 'react-bootstrap/Button';
import Spinner from 'react-bootstrap/Spinner';

export const Button = ({ loading, disabled, children, ...rest }) => (
    <BaseButton disabled={loading || disabled} {...rest}>
        {loading ? (
            <>
                <Spinner animation="grow" size="sm" />
                <Spinner animation="grow" size="sm" />
                <Spinner animation="grow" size="sm" />
            </>
        ) : (
            children
        )}
    </BaseButton>
);
