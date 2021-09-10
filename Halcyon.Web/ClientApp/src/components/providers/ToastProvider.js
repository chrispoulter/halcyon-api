import React, { useState, useContext } from 'react';
import Toast from 'react-bootstrap/Toast';
import ToastContainer from 'react-bootstrap/ToastContainer';

const initialState = {
    variant: undefined,
    message: undefined
};

export const ToastContext = React.createContext({});

export const useToast = () => useContext(ToastContext);

export const ToastProvider = ({ children }) => {
    const [state, setState] = useState(initialState);

    const hideToast = () => setState(initialState);

    const warn = async message =>
        setState({
            variant: 'warning',
            message
        });

    const error = async message =>
        setState({
            variant: 'danger',
            message
        });

    const success = async message =>
        setState({
            variant: 'success',
            message
        });

    return (
        <ToastContext.Provider value={{ warn, error, success }}>
            {children}
            <ToastContainer position="bottom-end" className="p-3">
                <Toast
                    bg={state.variant}
                    onClose={() => hideToast()}
                    show={!!state.message}
                    delay={5000}
                    autohide
                >
                    <Toast.Body className="d-flex">
                        <div className="text-white">{state.message}</div>
                        <button
                            type="button"
                            onClick={() => hideToast()}
                            className="btn-close btn-close-white me-2 m-auto"
                        />
                    </Toast.Body>
                </Toast>
            </ToastContainer>
        </ToastContext.Provider>
    );
};
