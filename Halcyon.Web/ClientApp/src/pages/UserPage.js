import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { Formik, Form } from 'formik';
import {
    Container,
    Button,
    FormGroup,
    InputGroup,
    Input,
    InputGroupAddon,
    UncontrolledDropdown,
    DropdownToggle,
    DropdownMenu,
    DropdownItem,
    Alert,
    Card,
    Badge
} from 'reactstrap';
import { Spinner, Pager, useFetch } from '../components';
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
                    color="primary"
                    className="align-self-start"
                    tag={Link}
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
                        <FormGroup>
                            <InputGroup>
                                <Input
                                    name="search"
                                    type="text"
                                    placeholder="Search..."
                                    value={values.search}
                                    onChange={handleChange}
                                    onBlur={handleBlur}
                                />
                                <InputGroupAddon addonType="append">
                                    <Button type="submit" color="secondary">
                                        Search
                                    </Button>
                                    <UncontrolledDropdown>
                                        <DropdownToggle caret color="secondary">
                                            Sort By{' '}
                                        </DropdownToggle>
                                        <DropdownMenu right>
                                            {Object.entries(SORT_OPTIONS).map(
                                                ([value, label]) => (
                                                    <DropdownItem
                                                        key={value}
                                                        active={
                                                            value === state.sort
                                                        }
                                                        onClick={() =>
                                                            onSort(value)
                                                        }
                                                    >
                                                        {label}
                                                    </DropdownItem>
                                                )
                                            )}
                                        </DropdownMenu>
                                    </UncontrolledDropdown>
                                </InputGroupAddon>
                            </InputGroup>
                        </FormGroup>
                    </Form>
                )}
            </Formik>

            {!data?.items.length ? (
                <Alert color="info" className="container p-3 mb-3">
                    No users could be found.
                </Alert>
            ) : (
                <>
                    {data.items.map(user => (
                        <Card
                            key={user.id}
                            to={`/user/${user.id}`}
                            className="mb-2"
                            body
                            tag={Link}
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
                                    <Badge color="danger" className="mr-1">
                                        Locked
                                    </Badge>
                                )}
                                {user.roles?.map(role => (
                                    <Badge
                                        key={role}
                                        color="primary"
                                        className="mr-1"
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
