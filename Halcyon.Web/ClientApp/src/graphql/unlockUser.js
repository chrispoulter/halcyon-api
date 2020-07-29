import { gql } from 'apollo-boost';

export const UNLOCK_USER = gql`
    mutation UnlockUser($id: ID!) {
        unlockUser(id: $id) {
            message
            user {
                id
                isLockedOut
            }
        }
    }
`;
