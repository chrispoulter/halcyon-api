import { gql } from 'apollo-boost';

export const UPDATE_PROFILE = gql`
    mutation UpdateProfile(
        $emailAddress: String!
        $firstName: String!
        $lastName: String!
        $dateOfBirth: DateTime!
    ) {
        updateProfile(
            input: {
                emailAddress: $emailAddress
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
            }
        }
    }
`;
