import { createSlice } from '@reduxjs/toolkit';

const initialState = null;

const slice = createSlice({
    name: 'modal',
    initialState,
    reducers: {
        showModal: (_, { payload }) => ({ ...payload }),
        hideModal: () => null
    }
});

export const { showModal, hideModal } = slice.actions;

export const modalReducer = slice.reducer;

export const selectModal = state => state.modal;
