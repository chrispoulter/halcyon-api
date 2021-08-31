import React from 'react';
import Form from 'react-bootstrap/Form';

export const TextInput = ({ field, form, label, ...rest }) => {
    const { name } = field;
    const { errors, touched } = form;
    const error = errors[name];
    const touch = touched[name];

    return (
        <Form.Group controlId={name} className="mb-3">
            <Form.Label>{label}</Form.Label>
            <Form.Control {...field} {...rest} isInvalid={!!touch && !!error} />
            <Form.Control.Feedback type="invalid">
                {error}
            </Form.Control.Feedback>
        </Form.Group>
    );
};
