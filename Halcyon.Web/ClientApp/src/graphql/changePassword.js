import { gql } from 'apollo-boost';

export const CHANGE_PASSWORD = gql`
    mutation ChangePassword($currentPassword: String!, $newPassword: String!) {
        changePassword(
            input: {
                currentPassword: $currentPassword
                newPassword: $newPassword
            }
        ) {
            message
        }
    }
`;
