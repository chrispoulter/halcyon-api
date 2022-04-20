import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Nav from 'react-bootstrap/Nav';
import NavDropdown from 'react-bootstrap/NavDropdown';
import { useAuth } from '../../contexts';
import { HasPermission } from './HasPermission';

export const UserProfile = () => {
    const navigate = useNavigate();

    const { currentUser, removeToken } = useAuth();

    const logout = () => {
        removeToken();
        navigate('/');
    };

    return (
        <Nav>
            <HasPermission
                fallback={
                    <>
                        <Nav.Link eventKey="login" to="/login" as={Link}>
                            Login
                        </Nav.Link>
                        <Nav.Link eventKey="register" to="/register" as={Link}>
                            Register
                        </Nav.Link>
                    </>
                }
            >
                <NavDropdown
                    id="collasible-nav-authenticated"
                    title={`${currentUser?.given_name} ${currentUser?.family_name} `}
                >
                    <NavDropdown.Item
                        eventKey="my-account"
                        to="/my-account"
                        as={Link}
                    >
                        My Account
                    </NavDropdown.Item>
                    <NavDropdown.Item eventKey="logout" onClick={logout}>
                        Logout
                    </NavDropdown.Item>
                </NavDropdown>
            </HasPermission>
        </Nav>
    );
};
