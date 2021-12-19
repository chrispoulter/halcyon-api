import React, { useState, useContext } from 'react';
import jwtDecode from 'jwt-decode';
import { getItem, setItem, removeItem } from '../utils/storage';

const getInitialState = () => {
    const accessToken = getItem('accessToken');
    if (!accessToken) {
        return {
            accessToken: undefined,
            currentUser: undefined
        };
    }

    const currentUser = jwtDecode(accessToken);

    return {
        accessToken,
        currentUser
    };
};

export const AuthContext = React.createContext({});

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }) => {
    const initialState = getInitialState();

    const [state, setState] = useState(initialState);

    const setToken = (accessToken, persist) => {
        setItem('accessToken', accessToken, persist);

        const currentUser = jwtDecode(accessToken);

        setState({
            accessToken,
            currentUser
        });
    };

    const removeToken = () => {
        removeItem('accessToken');

        setState({
            accessToken: undefined,
            currentUser: undefined
        });
    };

    return (
        <AuthContext.Provider value={{ ...state, setToken, removeToken }}>
            {children}
        </AuthContext.Provider>
    );
};
