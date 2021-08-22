import React from 'react';

export const Button = ({ loading, disabled, children, ...rest }) => (
    <button type="button" disabled={loading || disabled} {...rest}>
        {loading ? (
            <>
                <span className="spinner-border spinner-border-sm"></span>
                <span className="spinner-border spinner-border-sm"></span>
                <span className="spinner-border spinner-border-sm"></span>
            </>
        ) : (
            children
        )}
    </button>
);
