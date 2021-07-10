import React from 'react';
import { FormGroup, Label, FormText, Input } from 'reactstrap';

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
        <FormGroup>
            <Label>{label}</Label>
            <ul>
                {Object.entries(options).map(([value, label]) => (
                    <li key={`${name}.${value}`}>
                        <Input
                            id={`${name}.${value}`}
                            name={`${name}.${value}`}
                            type="checkbox"
                            checked={!!values.find(item => item === value)}
                            invalid={!!touch && !!error}
                            onChange={event =>
                                handleChange(value, event.target.checked)
                            }
                            onBlur={handleBlur}
                        />
                        <Label for={`${name}.${value}`}>{label}</Label>
                    </li>
                ))}
            </ul>
            {touch && error && <FormText color="danger">{errors}</FormText>}
        </FormGroup>
    );
};
