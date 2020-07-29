import React from 'react';
import { useMutation } from '@apollo/react-hooks';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, FormGroup } from 'reactstrap';
import { toast } from 'react-toastify';
import { FORGOT_PASSWORD } from '../graphql';
import { TextInput, Button } from '../components';

const initialValues = {
    emailAddress: ''
};

const validationSchema = Yup.object().shape({
    emailAddress: Yup.string().label('Email Address').email().required()
});

export const ForgotPasswordPage = ({ history }) => {
    const [forgotPassword] = useMutation(FORGOT_PASSWORD);

    const onSubmit = async variables => {
        try {
            const result = await forgotPassword({ variables });
            toast.success(result.data.forgotPassword.message);
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
