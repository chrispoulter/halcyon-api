import React, { useState } from 'react';

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

export const DateInput = ({ field, form, label }) => {
    const { name, onChange, onBlur, value } = field;
    const { errors, touched } = form;
    const error = errors[name];
    const touch = touched[name];

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
        <div className="mb-3">
            <label className="form-label">{label}</label>
            <div className="d-flex">
                <select
                    id={name}
                    value={state.date}
                    className={
                        !!touch && !!error
                            ? 'form-select me-1 is-invalid'
                            : 'form-select me-1'
                    }
                    onChange={event => handleDay(event.target.value)}
                    onBlur={handleBlur}
                >
                    <option value={-1}>Day...</option>
                    {Array.from({ length: 31 }).map((_, index) => (
                        <option key={index}>{index + 1}</option>
                    ))}
                </select>
                <select
                    value={state.month}
                    className={
                        !!touch && !!error
                            ? 'form-select me-1 is-invalid'
                            : 'form-select me-1'
                    }
                    onChange={event => handleMonth(event.target.value)}
                    onBlur={handleBlur}
                >
                    <option value={-1}>Month...</option>
                    {Array.from({ length: 12 }).map((_, index) => (
                        <option key={index} value={index}>
                            {monthNames[index]}
                        </option>
                    ))}
                </select>
                <select
                    value={state.year}
                    className={
                        !!touch && !!error
                            ? 'form-select is-invalid me-1'
                            : 'form-select me-1'
                    }
                    onChange={event => handleYear(event.target.value)}
                    onBlur={handleBlur}
                >
                    <option value={-1}>Year</option>
                    {Array.from({ length: 120 }).map((_, index) => (
                        <option key={index}>{currentYear - index}</option>
                    ))}
                </select>
            </div>
            {touch && error && <div className="invalid-feedback d-block">{error}</div>}
        </div>
    );
};
