import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { Helmet } from 'react-helmet';
import { Formik, Form } from 'formik';
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
        <div className="container">
            <Helmet>
                <title>Users</title>
            </Helmet>

            <div className="d-flex justify-content-between mb-3">
                <h1>Users</h1>
                <Link
                    to="/user/create"
                    className="btn btn-primary align-self-start"
                >
                    Create New
                </Link>
            </div>
            <hr />

            <Formik
                onSubmit={onSubmit}
                initialValues={{ search: state.search }}
            >
                {({ handleChange, handleBlur, values }) => (
                    <Form>
                        <div className="input-group mb-3">
                            <input
                                name="search"
                                type="text"
                                className="form-control"
                                placeholder="Search..."
                                value={values.search}
                                onChange={handleChange}
                                onBlur={handleBlur}
                            />
                            <button type="submit" className="btn btn-secondary">
                                Search
                            </button>
                            <button
                                className="btn btn-outline-secondary dropdown-toggle"
                                type="button"
                                data-bs-toggle="dropdown"
                                aria-expanded="false"
                            >
                                Sort By
                            </button>
                            <ul className="dropdown-menu dropdown-menu-end">
                                {Object.entries(SORT_OPTIONS).map(
                                    ([value, label]) => (
                                        <Link
                                            key={value}
                                            className="dropdown-item"
                                            active={value === state.sort}
                                            onClick={() => onSort(value)}
                                        >
                                            {label}
                                        </Link>
                                    )
                                )}
                            </ul>
                        </div>
                    </Form>
                )}
            </Formik>

            {!data?.items.length ? (
                <div className="container alert alert-info p-3 mb-3">
                    No users could be found.
                </div>
            ) : (
                <>
                    {data.items.map(user => (
                        <Link
                            key={user.id}
                            to={`/user/${user.id}`}
                            className="card mb-2"
                        >
                            <div className="card-body">
                                <h5>
                                    {user.firstName} {user.lastName}
                                    <br />
                                    <small className="text-muted">
                                        {user.emailAddress}
                                    </small>
                                </h5>
                                <div>
                                    {user.isLockedOut && (
                                        <div className="badge bg-danger me-1">
                                            Locked
                                        </div>
                                    )}
                                    {user.roles?.map(role => (
                                        <div
                                            key={role}
                                            className="badge bg-primary me-1"
                                        >
                                            {ALL_ROLES[role]}
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </Link>
                    ))}

                    <Pager
                        hasNextPage={data.hasNextPage}
                        hasPreviousPage={data.hasPreviousPage}
                        onNextPage={onNextPage}
                        onPreviousPage={onPreviousPage}
                    />
                </>
            )}
        </div>
    );
};
