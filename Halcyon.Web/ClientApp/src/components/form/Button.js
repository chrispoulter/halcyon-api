import React from 'react';
import { Button as BaseButton, Spinner } from 'reactstrap';

export const Button = ({ loading, disabled, children, ...rest }) => (
    <BaseButton disabled={loading || disabled} {...rest}>
        {loading ? (
            <>
                <Spinner type="grow" size="sm" />
                <Spinner type="grow" size="sm" />
                <Spinner type="grow" size="sm" />
            </>
        ) : (
            children
        )}
    </BaseButton>
);
