import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import BootstrapToast from 'react-bootstrap/Toast';
import ToastContainer from 'react-bootstrap/ToastContainer';
import { hideToast, selectToasts } from '../redux';

export const Toast = () => {
    const dispatch = useDispatch();

    const toasts = useSelector(selectToasts);

    const onClose = id => dispatch(hideToast(id));

    return (
        <ToastContainer
            position="bottom-end"
            className="p-3"
            containerPosition="fixed"
        >
            {toasts.map(({ id, variant, message }) => (
                <BootstrapToast
                    key={id}
                    bg={variant}
                    onClose={() => onClose(id)}
                    delay={5000}
                    autohide
                >
                    <BootstrapToast.Body className="d-flex">
                        <div className="text-white">{message}</div>
                        <button
                            type="button"
                            onClick={() => onClose(id)}
                            className="btn-close btn-close-white me-2 m-auto"
                        />
                    </BootstrapToast.Body>
                </BootstrapToast>
            ))}
        </ToastContainer>
    );
};
