import { gql } from 'apollo-boost';

export const SEARCH_USERS = gql`
    query SearchUsers(
        $size: Int
        $search: String
        $sort: UserSortExpression
        $cursor: String
    ) {
        searchUsers(
            input: {
                size: $size
                search: $search
                sort: $sort
                cursor: $cursor
            }
        ) {
            items {
                id
                emailAddress
                firstName
                lastName
                dateOfBirth
                isLockedOut
                roles
            }
            before
            after
        }
    }
`;
