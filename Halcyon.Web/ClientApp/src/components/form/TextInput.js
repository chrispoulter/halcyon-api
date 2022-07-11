import React from 'react';
import { useField } from 'formik';
import Form from 'react-bootstrap/Form';

export const TextInput = ({ label, ...rest }) => {
    const [field, meta] = useField(rest);
    const { name } = field;
    const { error, touched } = meta;

    return (
        <Form.Group controlId={name} className="mb-3">
            <Form.Label>{label}</Form.Label>
            <Form.Control
                {...field}
                {...rest}
                isInvalid={!!touched && !!error}
            />
            <Form.Control.Feedback type="invalid">
                {error}
            </Form.Control.Feedback>
        </Form.Group>
    );
};
