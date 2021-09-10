import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { Formik, Form } from 'formik';
import Container from 'react-bootstrap/Container';
import InputGroup from 'react-bootstrap/InputGroup';
import FormControl from 'react-bootstrap/FormControl';
import DropdownButton from 'react-bootstrap/DropdownButton';
import Dropdown from 'react-bootstrap/Dropdown';
import Alert from 'react-bootstrap/Alert';
import Card from 'react-bootstrap/Card';
import Badge from 'react-bootstrap/Badge';
import { Button, Spinner, Pager, useFetch } from '../components';
import { ALL_ROLES } from '../utils/auth';

const SORT_OPTIONS = {
    NAME_ASC: 'Name A-Z',
    NAME_DESC: 'Name Z-A',
    EMAIL_ADDRESS_ASC: 'Email Address A-Z',
    EMAIL_ADDRESS_DESC: 'Email Address Z-A'
};

export const UserPage = () => {
    const [state, setState] = useState({
        page: 1,
        size: 10,
        search: '',
        sort: 'NAME_ASC'
    });

    const { loading, data } = useFetch({
        method: 'GET',
        url: '/user',
        params: state
    });

    if (loading) {
        return <Spinner />;
    }

    const onSort = value => setState({ ...state, sort: value });

    const onPreviousPage = () => setState({ ...state, page: state.page - 1 });

    const onNextPage = () => setState({ ...state, page: state.page + 1 });

    const onSubmit = values =>
        setState({ ...state, page: 1, search: values.search });

    return (
        <Container>
            <Helmet>
                <title>Users</title>
            </Helmet>

            <div className="d-flex justify-content-between mb-3">
                <h1>Users</h1>
                <Button
                    to="/user/create"
                    as={Link}
                    variant="primary"
                    className="align-self-start"
                >
                    Create New
                </Button>
            </div>
            <hr />

            <Formik
                onSubmit={onSubmit}
                initialValues={{ search: state.search }}
            >
                {({ handleChange, handleBlur, values }) => (
                    <Form>
                        <InputGroup className="mb-3">
                            <FormControl
                                name="search"
                                type="text"
                                placeholder="Search..."
                                value={values.search}
                                onChange={handleChange}
                                onBlur={handleBlur}
                            />
                            <Button type="submit" variant="secondary">
                                Search
                            </Button>
                            <DropdownButton
                                title="Sort By"
                                variant="secondary"
                                align="end"
                            >
                                {Object.entries(SORT_OPTIONS).map(
                                    ([value, label]) => (
                                        <Dropdown.Item
                                            key={value}
                                            active={value === state.sort}
                                            onClick={() => onSort(value)}
                                        >
                                            {label}
                                        </Dropdown.Item>
                                    )
                                )}
                            </DropdownButton>
                        </InputGroup>
                    </Form>
                )}
            </Formik>

            {!data?.items.length ? (
                <Alert variant="info">No users could be found.</Alert>
            ) : (
                <>
                    {data.items.map(user => (
                        <Card
                            key={user.id}
                            to={`/user/${user.id}`}
                            as={Link}
                            body
                            className="text-decoration-none mb-2"
                        >
                            <h5>
                                {user.firstName} {user.lastName}
                                <br />
                                <small className="text-muted">
                                    {user.emailAddress}
                                </small>
                            </h5>
                            <div>
                                {user.isLockedOut && (
                                    <Badge bg="danger" className="me-1">
                                        Locked
                                    </Badge>
                                )}
                                {user.roles?.map(role => (
                                    <Badge
                                        key={role}
                                        bg="primary"
                                        className="me-1"
                                    >
                                        {ALL_ROLES[role]}
                                    </Badge>
                                ))}
                            </div>
                        </Card>
                    ))}

                    <Pager
                        hasNextPage={data.hasNextPage}
                        hasPreviousPage={data.hasPreviousPage}
                        onNextPage={onNextPage}
                        onPreviousPage={onPreviousPage}
                    />
                </>
            )}
        </Container>
    );
};
