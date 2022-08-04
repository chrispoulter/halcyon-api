import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import BootstrapToast from 'react-bootstrap/Toast';
import ToastContainer from 'react-bootstrap/ToastContainer';
import { hideToast, selectToast } from '../redux';

export const Toast = () => {
    const dispatch = useDispatch();

    const { variant, message } = useSelector(selectToast);

    const onClose = () => dispatch(hideToast());

    return (
        <ToastContainer position="bottom-end" className="p-3">
            <BootstrapToast
                bg={variant}
                onClose={onClose}
                show={!!message}
                delay={5000}
                autohide
            >
                <BootstrapToast.Body className="d-flex">
                    <div className="text-white">{message}</div>
                    <button
                        type="button"
                        onClick={onClose}
                        className="btn-close btn-close-white me-2 m-auto"
                    />
                </BootstrapToast.Body>
            </BootstrapToast>
        </ToastContainer>
    );
};
