import React from 'react';
import Form from 'react-bootstrap/Form';

export const CheckboxInput = ({ field, form, label }) => {
    const { name, value } = field;
    const { errors, touched } = form;
    const error = errors[name];
    const touch = touched[name];

    return (
        <Form.Group controlId={name} className="mb-3">
            <Form.Check
                {...field}
                id={name}
                type="checkbox"
                label={label}
                checked={value}
                isInvalid={!!touch && !!error}
            />
            <Form.Control.Feedback type="invalid">
                {error}
            </Form.Control.Feedback>
        </Form.Group>
    );
};
