import { gql } from 'apollo-boost';

export const DELETE_USER = gql`
    mutation DeleteUser($id: ID!) {
        deleteUser(id: $id) {
            message
        }
    }
`;
