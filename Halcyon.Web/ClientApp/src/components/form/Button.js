import React from 'react';

export const Button = ({ loading, disabled, children, ...rest }) => (
    <button type="button" disabled={loading || disabled} {...rest}>
        {loading ? (
            <>
                <span className="spinner-grow spinner-grow-sm"></span>
                <span className="spinner-grow spinner-grow-sm"></span>
                <span className="spinner-grow spinner-grow-sm"></span>
            </>
        ) : (
            children
        )}
    </button>
);
