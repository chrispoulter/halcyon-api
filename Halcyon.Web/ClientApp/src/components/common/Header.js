import React from 'react';
import { Link, useHistory } from 'react-router-dom';
import Navbar from 'react-bootstrap/Navbar';
import Nav from 'react-bootstrap/Nav';
import NavDropdown from 'react-bootstrap/NavDropdown';
import Container from 'react-bootstrap/Container';
import { useAuth } from '../providers';
import { isAuthorized, USER_ADMINISTRATOR_ROLES } from '../../utils/auth';
import { trackEvent } from '../../utils/logger';

export const Header = () => {
    const history = useHistory();

    const { currentUser, removeToken } = useAuth();

    const isAuthenticated = isAuthorized(currentUser);

    const isUserAdministrator = isAuthorized(
        currentUser,
        USER_ADMINISTRATOR_ROLES
    );

    const logout = () => {
        removeToken();
        trackEvent('logout');
        history.push('/');
    };

    return (
        <Navbar
            collapseOnSelect
            expand="lg"
            bg="dark"
            variant="dark"
            className="mb-3"
        >
            <Container>
                <Navbar.Brand to="/" as={Link}>
                    Halcyon
                </Navbar.Brand>
                <Navbar.Toggle aria-controls="responsive-navbar-nav" />
                <Navbar.Collapse id="responsive-navbar-nav">
                    <Nav className="me-auto">
                        {isUserAdministrator && (
                            <Nav.Link to="/user" as={Link}>
                                Users
                            </Nav.Link>
                        )}
                    </Nav>
                    <Nav>
                        {isAuthenticated ? (
                            <NavDropdown
                                id="collasible-nav-authenticated"
                                title={`${currentUser.given_name} ${currentUser.family_name} `}
                            >
                                <NavDropdown.Item to="/my-account" as={Link}>
                                    My Account
                                </NavDropdown.Item>
                                <NavDropdown.Item onClick={logout}>
                                    Logout
                                </NavDropdown.Item>
                            </NavDropdown>
                        ) : (
                            <>
                                <Nav.Link to="/login" as={Link}>
                                    Login
                                </Nav.Link>
                                <Nav.Link to="/register" as={Link}>
                                    Register
                                </Nav.Link>
                            </>
                        )}
                    </Nav>
                </Navbar.Collapse>
            </Container>
        </Navbar>
    );
};
