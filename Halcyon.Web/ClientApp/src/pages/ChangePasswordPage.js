import React from 'react';
import { Link } from 'react-router-dom';
import { useMutation } from '@apollo/react-hooks';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Container, FormGroup } from 'reactstrap';
import { toast } from 'react-toastify';
import { CHANGE_PASSWORD } from '../graphql';
import { TextInput, Button } from '../components';

const initialValues = {
    currentPassword: '',
    newPassword: '',
    confirmNewPassword: ''
};

const validationSchema = Yup.object().shape({
    currentPassword: Yup.string().label('Current Password').required(),
    newPassword: Yup.string().label('New Password').min(8).max(50).required(),
    confirmNewPassword: Yup.string()
        .label('Confirm New Password')
        .required()
        .oneOf(
            [Yup.ref('newPassword')],
            d => `The "${d.label}" field does not match.`
        )
});

export const ChangePasswordPage = ({ history }) => {
    const [changePassword] = useMutation(CHANGE_PASSWORD);

    const onSubmit = async variables => {
        try {
            const result = await changePassword({ variables });
            toast.success(result.data.changePassword.message);
            history.push('/my-account');
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <Container>
            <h1>Change Password</h1>
            <hr />

            <Formik
                initialValues={initialValues}
                validationSchema={validationSchema}
                onSubmit={onSubmit}
            >
                {({ isSubmitting }) => (
                    <Form noValidate>
                        <Field
                            name="currentPassword"
                            type="password"
                            label="Current Password"
                            required
                            maxLength={50}
                            autoComplete="current-password"
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
                                to="/my-account"
                                className="mr-1"
                                tag={Link}
                            >
                                Cancel
                            </Button>
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

            <p>
                Forgotten your password?{' '}
                <Link to="/forgot-password">Request reset</Link>
            </p>
        </Container>
    );
};
