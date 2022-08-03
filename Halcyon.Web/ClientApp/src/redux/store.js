import { configureStore, combineReducers } from '@reduxjs/toolkit';
import { setupListeners } from '@reduxjs/toolkit/query';
import { halcyonApi, rtkQueryErrorLogger } from './services';
import { authReducer, modalReducer, toastReducer } from './features';

const combinedReducer = combineReducers({
    [halcyonApi.reducerPath]: halcyonApi.reducer,
    auth: authReducer,
    modal: modalReducer,
    toast: toastReducer
});

const rootReducer = (state, action) => {
    if (action.type === 'auth/removeToken') {
        state = undefined;
    }

    return combinedReducer(state, action);
};

export const store = configureStore({
    reducer: rootReducer,
    middleware: getDefaultMiddleware =>
        getDefaultMiddleware({
            serializableCheck: false
        }).concat(halcyonApi.middleware, rtkQueryErrorLogger)
});

setupListeners(store.dispatch);
