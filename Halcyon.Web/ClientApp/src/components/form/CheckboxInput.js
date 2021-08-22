import React from 'react';

export const CheckboxInput = ({ field, form, label }) => {
    const { name, value } = field;
    const { errors, touched } = form;
    const error = errors[name];
    const touch = touched[name];

    return (
        <div className="mb-3">
            <div className="form-check">
                <input
                    id={name}
                    type="checkbox"
                    className={
                        !!touch && !!error
                            ? 'form-check-input is-invalid'
                            : 'form-check-input'
                    }
                    checked={value}
                    {...field}
                />
                <label htmlFor={name} className="form-check-label">
                    {label}
                </label>
            </div>
            {touch && error && (
                <div className="invalid-feedback d-block">{error}</div>
            )}
        </div>
    );
};
