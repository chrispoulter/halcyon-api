import { gql } from 'apollo-boost';

export const UPDATE_USER = gql`
    mutation UpdateUser(
        $id: ID!
        $emailAddress: String!
        $firstName: String!
        $lastName: String!
        $dateOfBirth: DateTime!
        $roles: [String!]
    ) {
        updateUser(
            id: $id
            input: {
                emailAddress: $emailAddress
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
                roles
            }
        }
    }
`;
