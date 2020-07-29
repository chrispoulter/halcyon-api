import { gql } from 'apollo-boost';

export const FORGOT_PASSWORD = gql`
    mutation ForgotPassword($emailAddress: String!) {
        forgotPassword(emailAddress: $emailAddress) {
            message
        }
    }
`;
