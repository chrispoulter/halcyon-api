import { gql } from 'apollo-boost';

export const GET_PROFILE = gql`
    query GetProfile {
        getProfile {
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
