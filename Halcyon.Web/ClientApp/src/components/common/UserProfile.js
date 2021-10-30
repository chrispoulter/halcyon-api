import React from 'react';
import { Link, useHistory } from 'react-router-dom';
import Nav from 'react-bootstrap/Nav';
import NavDropdown from 'react-bootstrap/NavDropdown';
import { useAuth } from '../providers';
import { HasPermission } from './HasPermission';
import { trackEvent } from '../../utils/logger';

export const UserProfile = () => {
    const history = useHistory();

    const { currentUser, removeToken } = useAuth();

    const logout = () => {
        removeToken();
        trackEvent('logout');
        history.push('/');
    };

    return (
        <Nav>
            <HasPermission
                fallback={
                    <>
                        <Nav.Link to="/login" as={Link}>
                            Login
                        </Nav.Link>
                        <Nav.Link to="/register" as={Link}>
                            Register
                        </Nav.Link>
                    </>
                }
            >
                <NavDropdown
                    id="collasible-nav-authenticated"
                    title={`${currentUser?.given_name} ${currentUser?.family_name} `}
                >
                    <NavDropdown.Item to="/my-account" as={Link}>
                        My Account
                    </NavDropdown.Item>
                    <NavDropdown.Item onClick={logout}>Logout</NavDropdown.Item>
                </NavDropdown>
            </HasPermission>
        </Nav>
    );
};
