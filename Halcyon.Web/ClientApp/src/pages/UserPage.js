import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useQuery } from '@apollo/react-hooks';
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
import { SEARCH_USERS } from '../graphql';
import { Spinner, Pager } from '../components';

const sortOptions = [
    { label: 'Name A-Z', value: 'NAME_ASC' },
    { label: 'Name Z-A', value: 'NAME_DESC' },
    { label: 'Email Address A-Z', value: 'EMAIL_ADDRESS_ASC' },
    { label: 'Email Address Z-A', value: 'EMAIL_ADDRESS_DESC' }
];

export const UserPage = () => {
    const [state, setState] = useState({
        size: 10,
        search: '',
        sort: sortOptions[0].value,
        cursor: undefined
    });

    const { loading, data } = useQuery(SEARCH_USERS, {
        variables: state
    });

    if (loading) {
        return <Spinner />;
    }

    const onSort = value =>
        setState({ ...state, cursor: undefined, sort: value });

    const onPreviousPage = () =>
        setState({ ...state, cursor: data.searchUsers.before });

    const onNextPage = () =>
        setState({ ...state, cursor: data.searchUsers.after });

    const onSubmit = values =>
        setState({ ...state, cursor: undefined, search: values.search });

    return (
        <Container>
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
                                            {sortOptions.map(option => (
                                                <DropdownItem
                                                    key={option.value}
                                                    active={
                                                        option.value ===
                                                        state.sort
                                                    }
                                                    onClick={() =>
                                                        onSort(option.value)
                                                    }
                                                >
                                                    {option.label}
                                                </DropdownItem>
                                            ))}
                                        </DropdownMenu>
                                    </UncontrolledDropdown>
                                </InputGroupAddon>
                            </InputGroup>
                        </FormGroup>
                    </Form>
                )}
            </Formik>

            {!data?.searchUsers.items.length ? (
                <Alert color="info" className="container p-3 mb-3">
                    No users could be found.
                </Alert>
            ) : (
                <>
                    {data.searchUsers.items?.map(user => (
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
                                {user.roles.map(role => (
                                    <Badge
                                        key={role}
                                        color="primary"
                                        className="mr-1"
                                    >
                                        {role}
                                    </Badge>
                                ))}
                            </div>
                        </Card>
                    ))}

                    <Pager
                        hasNextPage={!!data.searchUsers.after}
                        hasPreviousPage={!!data.searchUsers.before}
                        onNextPage={onNextPage}
                        onPreviousPage={onPreviousPage}
                    />
                </>
            )}
        </Container>
    );
};
