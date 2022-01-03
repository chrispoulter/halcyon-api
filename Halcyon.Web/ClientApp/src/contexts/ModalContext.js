import React, { useState, useContext } from 'react';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';

const initialState = {
    title: undefined,
    body: undefined,
    onOk: undefined
};

export const ModalContext = React.createContext({});

export const useModal = () => useContext(ModalContext);

export const ModalProvider = ({ children }) => {
    const [state, setState] = useState(initialState);

    const showModal = async ({ title, body, onOk }) =>
        setState({
            title,
            body,
            onOk
        });

    const hideModal = () => setState(initialState);

    const onOk = async () => {
        setState(initialState);
        await state.onOk();
    };

    return (
        <ModalContext.Provider value={{ showModal }}>
            {children}
            <Modal show={!!state.title} onHide={hideModal}>
                <Modal.Header closeButton>
                    <Modal.Title>{state.title}</Modal.Title>
                </Modal.Header>
                <Modal.Body>{state.body}</Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={hideModal}>
                        Cancel
                    </Button>
                    <Button variant="primary" onClick={onOk}>
                        Ok
                    </Button>
                </Modal.Footer>
            </Modal>
        </ModalContext.Provider>
    );
};
