import React, { useState } from 'react';
import moment from 'moment';
import { FormGroup, Label, FormText, Input } from 'reactstrap';

const months = moment.months();
const currentYear = moment().year();

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
        const date = moment(value);
        dateValue.year = date.year();
        dateValue.month = date.month();
        dateValue.date = date.date();
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

        const value = isDateSet ? moment(input).toISOString() || '' : '';

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
            <Label for={name}>{label}</Label>
            <div className="d-flex">
                <Input
                    id={name}
                    type="select"
                    value={state.date}
                    invalid={!!touch && !!error}
                    onChange={event => handleDay(event.target.value)}
                    onBlur={handleBlur}
                    className="mr-1"
                >
                    <option value={-1}>Day...</option>
                    {Array.from({ length: 31 }).map((_, index) => (
                        <option key={index}>{index + 1}</option>
                    ))}
                </Input>
                <Input
                    type="select"
                    value={state.month}
                    invalid={!!touch && !!error}
                    onChange={event => handleMonth(event.target.value)}
                    onBlur={handleBlur}
                    className="mr-1"
                >
                    <option value={-1}>Month...</option>
                    {months.map((month, index) => (
                        <option key={index} value={index}>
                            {month}
                        </option>
                    ))}
                </Input>
                <Input
                    type="select"
                    value={state.year}
                    invalid={!!touch && !!error}
                    onChange={event => handleYear(event.target.value)}
                    onBlur={handleBlur}
                >
                    <option value={-1}>Year...</option>
                    {Array.from({ length: 150 }).map((_, index) => (
                        <option key={index}>{currentYear - index}</option>
                    ))}
                </Input>
            </div>
            {touch && error && <FormText color="danger">{error}</FormText>}
        </FormGroup>
    );
};
