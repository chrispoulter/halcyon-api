import { gql } from 'apollo-boost';

export const RESET_PASSWORD = gql`
    mutation ResetPassword(
        $token: String!
        $emailAddress: String!
        $newPassword: String!
    ) {
        resetPassword(
            input: {
                token: $token
                emailAddress: $emailAddress
                newPassword: $newPassword
            }
        ) {
            message
        }
    }
`;
