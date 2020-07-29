import { gql } from 'apollo-boost';

export const REGISTER = gql`
    mutation Register(
        $emailAddress: String!
        $password: String!
        $firstName: String!
        $lastName: String!
        $dateOfBirth: DateTime!
    ) {
        register(
            input: {
                emailAddress: $emailAddress
                password: $password
                firstName: $firstName
                lastName: $lastName
                dateOfBirth: $dateOfBirth
            }
        ) {
            message
            user {
                id
                emailAddress
                firstName
                lastName
                dateOfBirth
                isLockedOut
                roles
            }
        }
    }
`;
