import { gql } from 'apollo-boost';

export const GENERATE_TOKEN = gql`
    mutation GenerateToken(
        $grantType: GrantType!
        $emailAddress: String!
        $password: String!
    ) {
        generateToken(
            input: {
                grantType: $grantType
                emailAddress: $emailAddress
                password: $password
            }
        ) {
            accessToken
            expiresIn
        }
    }
`;
