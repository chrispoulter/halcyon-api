import { createSlice } from '@reduxjs/toolkit';

const initialState = [];

const slice = createSlice({
    name: 'toast',
    initialState,
    reducers: {
        showToast: (state, { payload }) => [
            ...state,
            { ...payload, id: new Date().getTime() }
        ],
        hideToast: (state, { payload }) => [
            ...state.filter(toast => toast.id !== payload)
        ]
    }
});

export const { showToast, hideToast } = slice.actions;

export const toastReducer = slice.reducer;

export const selectToasts = state => state.toast;
