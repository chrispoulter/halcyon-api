import React from 'react';

export const TextInput = ({ field, form, label, ...rest }) => {
    const { name } = field;
    const { errors, touched } = form;
    const error = errors[name];
    const touch = touched[name];

    return (
        <div className="mb-3">
            <label htmlFor={name} className="form-label">
                {label}
            </label>
            <input
                id={name}
                className={
                    !!touch && !!error
                        ? 'form-control is-invalid'
                        : 'form-control'
                }
                {...field}
                {...rest}
            />
            {touch && error && <div className="invalid-feedback d-block">{error}</div>}
        </div>
    );
};
