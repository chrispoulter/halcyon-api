import React from 'react';
import { useField } from 'formik';
import Form from 'react-bootstrap/Form';

export const CheckboxInput = ({ label, ...rest }) => {
    const [field, meta] = useField(rest);
    const { name, value } = field;
    const { error, touched } = meta;

    return (
        <Form.Group controlId={name} className="mb-3">
            <Form.Check
                {...field}
                {...rest}
                id={name}
                type="checkbox"
                label={label}
                checked={value}
                isInvalid={!!touched && !!error}
            />
            <Form.Control.Feedback type="invalid">
                {error}
            </Form.Control.Feedback>
        </Form.Group>
    );
};
