import React from 'react';
import { Link } from 'react-router-dom';
import Navbar from 'react-bootstrap/Navbar';
import Nav from 'react-bootstrap/Nav';
import Container from 'react-bootstrap/Container';
import { HasPermission } from './HasPermission';
import { UserProfile } from './UserProfile';
import { USER_ADMINISTRATOR_ROLES } from '../../utils/auth';

export const Header = () => (
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
                    <HasPermission requiredRoles={USER_ADMINISTRATOR_ROLES}>
                        <Nav.Link to="/user" as={Link}>
                            Users
                        </Nav.Link>
                    </HasPermission>
                </Nav>
                <UserProfile />
            </Navbar.Collapse>
        </Container>
    </Navbar>
);
