import React from 'react';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, FormGroup } from 'reactstrap';
import { toast } from 'react-toastify';
import { TextInput, Button, useFetch } from '../components';

const initialValues = {
    emailAddress: ''
};

const validationSchema = Yup.object().shape({
    emailAddress: Yup.string().label('Email Address').email().required()
});

export const ForgotPasswordPage = ({ history }) => {
    const { refetch: forgotPassword } = useFetch({
        method: 'PUT',
        url: '/account/forgotpassword',
        manual: true
    });

    const onSubmit = async data => {
        try {
            const result = await forgotPassword(data);
            toast.success(result.messages);
            history.push('/login');
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <Container>
            <h1>Forgotten Password</h1>
            <hr />

            <Formik
                initialValues={initialValues}
                validationSchema={validationSchema}
                onSubmit={onSubmit}
            >
                {({ isSubmitting }) => (
                    <Form noValidate>
                        <Field
                            name="emailAddress"
                            type="email"
                            label="Email Address"
                            required
                            maxLength={254}
                            autoComplete="username"
                            component={TextInput}
                        />

                        <FormGroup className="text-right">
                            <Button
                                type="submit"
                                color="primary"
                                loading={isSubmitting}
                            >
                                Submit
                            </Button>
                        </FormGroup>
                    </Form>
                )}
            </Formik>
        </Container>
    );
};
