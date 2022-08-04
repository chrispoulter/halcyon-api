import React, { useState } from 'react';
import { useField } from 'formik';
import Form from 'react-bootstrap/Form';

const currentYear = new Date().getFullYear();

const monthNames = [
    'January',
    'February',
    'March',
    'April',
    'May',
    'June',
    'July',
    'August',
    'September',
    'October',
    'November',
    'December'
];

export const DateInput = ({ label, ...rest }) => {
    const [field, meta] = useField(rest);
    const { name, onChange, onBlur, value } = field;
    const { error, touched } = meta;

    const dateValue = {
        year: -1,
        month: -1,
        date: -1
    };

    if (value) {
        const date = new Date(value);
        dateValue.year = date.getFullYear();
        dateValue.month = date.getMonth();
        dateValue.date = date.getDate();
    }

    const [state, setState] = useState(dateValue);

    const handleYear = year =>
        handleChange({ ...state, year: parseInt(year, 10) });

    const handleMonth = month =>
        handleChange({ ...state, month: parseInt(month, 10) });

    const handleDay = day =>
        handleChange({ ...state, date: parseInt(day, 10) });

    const handleChange = input => {
        setState(input);

        const isDateSet =
            input.year > -1 && input.month > -1 && input.date > -1;

        const value = isDateSet
            ? new Date(input.year, input.month, input.date).toISOString() || ''
            : '';

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
        <Form.Group controlId={`${name}.date`} className="mb-3">
            <Form.Label>{label}</Form.Label>
            <div className="d-flex">
                <Form.Select
                    id={`${name}.date`}
                    value={state.date}
                    isInvalid={!!touched && !!error}
                    onChange={event => handleDay(event.target.value)}
                    onBlur={handleBlur}
                    className="me-1"
                >
                    <option value={-1}>Day...</option>
                    {Array.from({ length: 31 }).map((_, index) => (
                        <option key={index}>{index + 1}</option>
                    ))}
                </Form.Select>
                <Form.Select
                    id={`${name}.month`}
                    value={state.month}
                    isInvalid={!!touched && !!error}
                    onChange={event => handleMonth(event.target.value)}
                    onBlur={handleBlur}
                    className="me-1"
                >
                    <option value={-1}>Month...</option>
                    {Array.from({ length: 12 }).map((_, index) => (
                        <option key={index} value={index}>
                            {monthNames[index]}
                        </option>
                    ))}
                </Form.Select>
                <Form.Select
                    id={`${name}.year`}
                    value={state.year}
                    isInvalid={!!touched && !!error}
                    onChange={event => handleYear(event.target.value)}
                    onBlur={handleBlur}
                >
                    <option value={-1}>Year...</option>
                    {Array.from({ length: 120 }).map((_, index) => (
                        <option key={index}>{currentYear - index}</option>
                    ))}
                </Form.Select>
            </div>
            {touched && error && (
                <div className="invalid-feedback d-block">{error}</div>
            )}
        </Form.Group>
    );
};
