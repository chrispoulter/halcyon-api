import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import BoostrapModal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import { selectModal, hideModal } from '../../redux';

export const Modal = () => {
    const dispatch = useDispatch();

    const { title, body, onOk } = useSelector(selectModal);

    const onCancel = () => dispatch(hideModal());

    const onConfirm = async () => {
        dispatch(hideModal());
        await onOk();
    };

    return (
        <BoostrapModal show={!!title} onHide={onCancel}>
            <BoostrapModal.Header closeButton>
                <BoostrapModal.Title>{title}</BoostrapModal.Title>
            </BoostrapModal.Header>
            <BoostrapModal.Body>{body}</BoostrapModal.Body>
            <BoostrapModal.Footer>
                <Button variant="secondary" onClick={onCancel}>
                    Cancel
                </Button>
                <Button variant="primary" onClick={onConfirm}>
                    Ok
                </Button>
            </BoostrapModal.Footer>
        </BoostrapModal>
    );
};
