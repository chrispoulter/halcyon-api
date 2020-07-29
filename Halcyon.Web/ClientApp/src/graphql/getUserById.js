import { gql } from 'apollo-boost';

export const GET_USER_BY_ID = gql`
    query GetUserById($id: ID!) {
        getUserById(id: $id) {
            id
            emailAddress
            firstName
            lastName
            dateOfBirth
            isLockedOut
            roles
        }
    }
`;
