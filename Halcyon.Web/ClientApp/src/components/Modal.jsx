import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import BoostrapModal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import { selectModal, hideModal } from '../redux';

export const Modal = () => {
    const dispatch = useDispatch();

    const modal = useSelector(selectModal);

    const onCancel = () => dispatch(hideModal());

    const onConfirm = async () => {
        dispatch(hideModal());
        await modal.onOk();
    };

    if (!modal) {
        return null;
    }

    return (
        <BoostrapModal show onHide={onCancel}>
            <BoostrapModal.Header closeButton>
                <BoostrapModal.Title>{modal.title}</BoostrapModal.Title>
            </BoostrapModal.Header>
            <BoostrapModal.Body>{modal.body}</BoostrapModal.Body>
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
