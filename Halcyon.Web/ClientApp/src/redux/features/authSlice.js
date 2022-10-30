import { createSlice } from '@reduxjs/toolkit';
import jwtDecode from 'jwt-decode';
import { getItem, setItem, removeItem } from '@/utils/storage';

const initialState = () => {
    const accessToken = getItem('accessToken');

    if (!accessToken) {
        return null;
    }

    const currentUser = jwtDecode(accessToken);

    return {
        accessToken,
        currentUser
    };
};

const slice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        setToken: (_, { payload: { accessToken, persist } }) => {
            setItem('accessToken', accessToken, persist);

            const currentUser = jwtDecode(accessToken);

            return {
                accessToken,
                currentUser
            };
        },
        removeToken: () => {
            removeItem('accessToken');
            return null;
        }
    }
});

export const { setToken, removeToken } = slice.actions;

export const authReducer = slice.reducer;

export const selectCurrentUser = state => state.auth?.currentUser;

export const selectAccessToken = state => state.auth?.accessToken;
