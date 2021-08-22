import React from 'react';

export const CheckboxGroupInput = ({ field, form, label, options }) => {
    const { name, onChange, onBlur, value } = field;
    const { errors, touched } = form;
    const error = errors[name];
    const touch = touched[name];
    const values = value || [];

    const handleChange = (option, checked) => {
        const value = [...values];
        const index = values.indexOf(option);

        if (checked && index === -1) {
            value.push(option);
        }

        if (!checked && index > -1) {
            value.splice(index, 1);
        }

        onChange({
            target: {
                name,
                value
            }
        });
    };

    const handleBlur = () =>
        onBlur({
            target: {
                name
            }
        });

    return (
        <div className="mb-3">
            <label className="form-label">{label}</label>
            <ul className="list-unstyled">
                {Object.entries(options).map(([value, label]) => (
                    <li key={`${name}.${value}`}>
                        <div className="form-check">
                            <input
                                id={`${name}.${value}`}
                                name={`${name}.${value}`}
                                type="checkbox"
                                checked={!!values.find(item => item === value)}
                                className={
                                    !!touch && !!error
                                        ? 'form-check-input is-invalid'
                                        : 'form-check-input'
                                }
                                onChange={event =>
                                    handleChange(value, event.target.checked)
                                }
                                onBlur={handleBlur}
                            />
                            <label
                                htmlFor={`${name}.${value}`}
                                className="form-check-label"
                            >
                                {label}
                            </label>
                        </div>
                    </li>
                ))}
            </ul>
            {touch && error && <div className="invalid-feedback d-block">{error}</div>}
        </div>
    );
};
