import React from 'react';
import Form from 'react-bootstrap/Form';

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
        <Form.Group className="mb-3">
            <Form.Label>{label}</Form.Label>
            {Object.entries(options).map(([value, label]) => (
                <Form.Check
                    {...field}
                    key={`${name}.${value}`}
                    id={`${name}.${value}`}
                    name={`${name}.${value}`}
                    type="checkbox"
                    label={label}
                    checked={!!values.find(item => item === value)}
                    isInvalid={!!touch && !!error}
                    onChange={event =>
                        handleChange(value, event.target.checked)
                    }
                    onBlur={handleBlur}
                />
            ))}
            <Form.Control.Feedback type="invalid">
                {error}
            </Form.Control.Feedback>
        </Form.Group>
    );
};
