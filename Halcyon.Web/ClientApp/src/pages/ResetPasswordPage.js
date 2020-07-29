import React from 'react';
import { useMutation } from '@apollo/react-hooks';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, FormGroup } from 'reactstrap';
import { toast } from 'react-toastify';
import { RESET_PASSWORD } from '../graphql';
import { TextInput, Button } from '../components';

const initialValues = {
    emailAddress: '',
    newPassword: '',
    confirmNewPassword: ''
};

const validationSchema = Yup.object().shape({
    emailAddress: Yup.string().label('Email Address').email().required(),
    newPassword: Yup.string().label('New Password').min(8).max(50).required(),
    confirmNewPassword: Yup.string()
        .label('Confirm New Password')
        .required()
        .oneOf(
            [Yup.ref('newPassword')],
            d => `The "${d.label}" field does not match.`
        )
});

export const ResetPasswordPage = ({ match, history }) => {
    const [resetPassword] = useMutation(RESET_PASSWORD);

    const onSubmit = async variables => {
        try {
            const result = await resetPassword({
                variables: { token: match.params.token, ...variables }
            });

            toast.success(result.data.resetPassword.message);
            history.push('/login');
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <Container>
            <h1>Reset Password</h1>
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
                        <Field
                            name="newPassword"
                            type="password"
                            label="New Password"
                            required
                            maxLength={50}
                            autoComplete="new-password"
                            component={TextInput}
                        />
                        <Field
                            name="confirmNewPassword"
                            type="password"
                            label="Confirm New Password"
                            required
                            maxLength={50}
                            autoComplete="new-password"
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
