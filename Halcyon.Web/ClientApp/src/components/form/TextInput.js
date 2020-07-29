import React from 'react';
import { Input, FormGroup, Label, FormText } from 'reactstrap';

export const TextInput = ({ field, form, label, ...rest }) => {
    const { name } = field;
    const { errors, touched } = form;
    const error = errors[name];
    const touch = touched[name];

    return (
        <FormGroup>
            <Label for={name}>{label}</Label>
            <Input
                id={name}
                invalid={!!touch && !!error}
                {...field}
                {...rest}
            />
            {touch && error && <FormText color="danger">{error}</FormText>}
        </FormGroup>
    );
};
