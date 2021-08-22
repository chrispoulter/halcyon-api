import React, { useState, useEffect, useContext } from 'react';
import { Link, useHistory } from 'react-router-dom';

import { AuthContext } from '../providers/AuthProvider';
import { isAuthorized, USER_ADMINISTRATOR_ROLES } from '../../utils/auth';
import { trackEvent } from '../../utils/logger';

export const Header = () => {
    const history = useHistory();

    const { currentUser, removeToken } = useContext(AuthContext);

    const [isOpen, setIsOpen] = useState(false);

    useEffect(() => {
        const listen = history.listen(() => setIsOpen(false));
        return () => listen();
    }, [history]);

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

    const toggle = () => setIsOpen(!isOpen);

    return (
        <header className="bg-dark mb-3">
            <div className="container">
                <nav className="navbar navbar-dark navbar-expand-lg">
                    <Link to="/" className="navbar-brand">
                        Halcyon
                    </Link>
                    <button
                        className="navbar-toggler"
                        type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#navbarNavAltMarkup"
                        aria-controls="navbarSupportedContent"
                        aria-expanded="false"
                        aria-label="Toggle navigation"
                    >
                        <span className="navbar-toggler-icon"></span>
                    </button>
                    <div
                        id="navbarNavAltMarkup"
                        className="collapse navbar-collapse"
                    >
                        {isUserAdministrator && (
                            <ul className="navbar-nav">
                                <li className="nav-item">
                                    <Link to="/user" className="nav-link">
                                        Users
                                    </Link>
                                </li>
                            </ul>
                        )}

                        <ul className="navbar-nav ms-auto">
                            {isAuthenticated ? (
                                <li className="nav-item dropdown">
                                    <Link
                                        // id="navbarDropdownMenuLink"
                                        role="button"
                                        to="/"
                                        className="nav-link dropdown-toggle show"
                                        // data-bs-toggle="dropdown"
                                        aria-expanded="false"
                                    >
                                        {currentUser.given_name}{' '}
                                        {currentUser.family_name}{' '}
                                    </Link>
                                    <ul
                                        className="dropdown-menu show"
                                        aria-labelledby="navbarDropdownMenuLink"
                                    >
                                        <li>
                                            <Link
                                                to="/my-account"
                                                className="dropdown-item"
                                            >
                                                My Account
                                            </Link>
                                        </li>
                                        <li>
                                            <Link
                                                to="/"
                                                className="dropdown-item"
                                                onClick={logout}
                                            >
                                                Log Out
                                            </Link>
                                        </li>
                                    </ul>
                                </li>
                            ) : (
                                <>
                                    <li className="nav-item">
                                        <Link to="/login" className="nav-link">
                                            Login
                                        </Link>
                                    </li>
                                    <li className="nav-item">
                                        <Link
                                            to="/register"
                                            className="nav-link"
                                        >
                                            Register
                                        </Link>
                                    </li>
                                </>
                            )}
                        </ul>
                    </div>
                </nav>
            </div>
        </header>
    );
};
