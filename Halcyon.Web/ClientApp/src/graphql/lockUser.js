import { gql } from 'apollo-boost';

export const LOCK_USER = gql`
    mutation LockUser($id: ID!) {
        lockUser(id: $id) {
            message
            user {
                id
                isLockedOut
            }
        }
    }
`;
