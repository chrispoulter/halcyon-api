import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
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

const sortOptions = [
    'NAME_ASC',
    'NAME_DESC',
    'EMAIL_ADDRESS_ASC',
    'EMAIL_ADDRESS_DESC'
];

export const UserPage = () => {
    const { t } = useTranslation();

    const [state, setState] = useState({
        page: 1,
        size: 10,
        search: '',
        sort: sortOptions[0]
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
                <title>{t('pages.user.meta.title')}</title>
            </Helmet>

            <div className="d-flex justify-content-between mb-3">
                <h1>{t('pages.user.title')}</h1>
                <Button
                    to="/user/create"
                    color="primary"
                    className="align-self-start"
                    tag={Link}
                >
                    {t('pages.user.createNewButton')}
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
                                    placeholder={t(
                                        'pages.user.form.searchPlaceholder'
                                    )}
                                    value={values.search}
                                    onChange={handleChange}
                                    onBlur={handleBlur}
                                />
                                <InputGroupAddon addonType="append">
                                    <Button type="submit" color="secondary">
                                        {t('pages.user.form.searchButton')}
                                    </Button>
                                    <UncontrolledDropdown>
                                        <DropdownToggle caret color="secondary">
                                            {t('pages.user.form.sortByButton')}{' '}
                                        </DropdownToggle>
                                        <DropdownMenu right>
                                            {sortOptions.map(option => (
                                                <DropdownItem
                                                    key={option}
                                                    active={
                                                        option === state.sort
                                                    }
                                                    onClick={() =>
                                                        onSort(option)
                                                    }
                                                >
                                                    {t(
                                                        `api.userSortExpressions.${option}`
                                                    )}
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

            {!data?.items.length ? (
                <Alert color="info" className="container p-3 mb-3">
                    {t('pages.user.usersNotFound')}
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
                                        {t('pages.user.lockedBadge')}
                                    </Badge>
                                )}
                                {user.roles?.map(role => (
                                    <Badge
                                        key={role}
                                        color="primary"
                                        className="mr-1"
                                    >
                                        {t(`api.userRoles.${role}`)}
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
