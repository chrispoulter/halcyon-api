import { gql } from 'apollo-boost';

export const CREATE_USER = gql`
    mutation CreateUser(
        $emailAddress: String!
        $password: String!
        $firstName: String!
        $lastName: String!
        $dateOfBirth: DateTime!
        $roles: [String!]
    ) {
        createUser(
            input: {
                emailAddress: $emailAddress
                password: $password
                firstName: $firstName
                lastName: $lastName
                dateOfBirth: $dateOfBirth
                roles: $roles
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
